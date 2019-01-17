namespace Bitsie.Shop.Api
{
    public interface IMessageApi
    {
        void SendSms(string phoneNumber, string text);
    }
}
