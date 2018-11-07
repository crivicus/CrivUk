using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CrivServer.Infrastructure.ControllerHelpers
{
    public class VideoStreamHelper
    {

        private readonly string _filename;

        public VideoStreamHelper(string filename, string ext)
        {
            _filename = Path.Combine(AppDomain.CurrentDomain.GetData("PublicDirectory").ToString(),"videos", filename+"."+ext).ToString();
        }

        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read))
                {
                    var length = (int)video.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }

        public Stream StreamFile()
        {
            return File.OpenRead(_filename);
        }
    }
}
