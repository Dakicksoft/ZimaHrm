namespace ZimaHrm.Core.Infrastructure.Security
{
    public interface ICriptography
    {
        string Decrypt(string value);

        string Encrypt(string value);
    }
}
