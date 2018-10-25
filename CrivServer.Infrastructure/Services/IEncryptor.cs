using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public interface IEncryptor
    {
        string Encrypt(string input);
        string Decrypt(string cipherText);
        string ManualEncrypt(string input, string password);
        string ManualDecrypt(string cipherText, string password);
        Task FileEncrypt(string filename, string password);
        Task FileDecrypt(string filename, string password);
        X509Certificate2 GenerateCertificate(string certName, string exportpassword);
    }
}
