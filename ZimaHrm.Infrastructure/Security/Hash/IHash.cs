namespace ZimaHrm.Core.Infrastructure.Security
{
    public interface IHash
    {
        string Create(string value);

        byte[] Create(byte[] value);
    }
}
