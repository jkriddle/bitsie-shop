using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;

namespace Bitsie.Shop.Api
{
    public class TwilioApi : IMessageApi
    {
        private readonly string _fromNumber;
        private readonly TwilioRestClient _client;

        public TwilioApi(string fromNumber, string sid, string authToken)
        {
            _client = new TwilioRestClient(sid, authToken);
            _fromNumber = fromNumber;
        }

        public void SendSms(string phoneNumber, string text)
        {
            _client.SendMessage(
                _fromNumber,
                phoneNumber,
                text
            );
        }
    }
}
