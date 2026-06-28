namespace Ecommerce.AuthService.Application.Interfaces.IService
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
