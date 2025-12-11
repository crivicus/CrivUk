using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrivServer.Data.Contexts;
using CrivServer.Data.Models;
using CrivServer.Infrastructure.ControllerHelpers;
using CrivServer.Infrastructure.Services;
using CrivServer.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using Microsoft.AspNetCore.Http.Features;
using CrivServer.CrivUk.Models;

namespace CrivServer.CrivUk.Controllers
{
    //[ApiController]
    [Route("api/file")]
    public class FileTransferController : ControllerHeart
    {
        private readonly IDataProtector _encryptor;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public FileTransferController(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger, IDataProtectionProvider provider, IWebHostEnvironment env, UserManager<ApplicationUser> userManager) : base(configuration, context, logger) {
            _encryptor = provider.CreateProtector(_crivConfig.GetValue<string>("Protector"));
            _environment = env;
            _userManager = userManager;
        }

        [Route("get-video/{ext}/{filename}")]
        public async Task<IActionResult> GetVideoAsync([FromRoute] string filename, [FromRoute] string ext)
        {
            var video = new VideoStreamHelper(filename, ext);

            var stream = new FileStreamResult(video.StreamFile(), new System.Net.Http.Headers.MediaTypeHeaderValue("video/" + ext).MediaType);

            return stream;
        }

        [Authorize]
        [HttpPost("upload-files")]
        public async Task<IActionResult> UploadSmallFiles(List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(User);
            var userFolder = !string.IsNullOrWhiteSpace(user.UserFolder) ? user.UserFolder : "anon";
            long size = files.Sum(f => f.Length);
            var filePath = "public_data/" + userFolder;
            using (var fileprovider = new FileService(base._crivConfig, _encryptor, _environment))
            {
                await fileprovider.SaveFiles(files, userFolder);
            }
            return Ok(new { count = files.Count, size, filePath });
        }

        [Authorize]
        [DisableRequestSizeLimit]
        [HttpPost("upload-large-files")]
        public async Task<IActionResult> UploadLargeFiles(List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(User);
            var userFolder = !string.IsNullOrWhiteSpace(user.UserFolder) ? user.UserFolder : "anon";
            long size = files.Sum(f => f.Length);
            var filePath = "public_data/" + userFolder;
            using (var fileprovider = new FileService(base._crivConfig, _encryptor, _environment))
            {
                await fileprovider.SaveFiles(files, userFolder);
            }
            return Ok(new { count = files.Count, size, filePath });
        }

        [HttpGet("transfer")]
        [GenerateAntiforgeryTokenCookieForAjax]
        public IActionResult Index()
        {
            return PartialView();
        }

        #region uploadStream
        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [HttpPost("upload")]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload()
        {
            if (!FileUploadHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();
            string targetFilePath = null;

            var boundary = FileUploadHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            
            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (FileUploadHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);

                            _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                    }
                    else if (FileUploadHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key.ToString(), value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            var model = new FileModel();
            // Bind form data to a model
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(model, prefix: "",
                valueProvider: formValueProvider);
            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var uploadedData = targetFilePath;// ToDo: safely save to a non temporary location
            
            var user = await _userManager.GetUserAsync(User);
            var userFolder = !string.IsNullOrWhiteSpace(user.UserFolder) ? user.UserFolder : "anon";
            var path = Path.Combine(AppDomain.CurrentDomain.GetData("PublicDirectory").ToString(), userFolder).ToString();
            using (var fs = new FileService(_crivConfig,_encryptor,_environment))
            {
                await fs.CopyFile(path, new Guid().ToString(), uploadedData, uploadedData);
            }

                return Json(uploadedData);
        }
        #endregion

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}
