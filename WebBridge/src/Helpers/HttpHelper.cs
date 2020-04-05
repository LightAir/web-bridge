using System.Collections.Specialized;
using System.Net;

namespace WebBridge.Helpers
{
    public class HttpHelper
    {
        private readonly string _apiBaseUrl;
        private readonly string _apiToken;

        public HttpHelper(string _apiBaseUrl, string _apiToken)
        {
            this._apiBaseUrl = _apiBaseUrl;
            this._apiToken = _apiToken;
        }

        public void Post(NameValueCollection _pairs)
        {
            // byte[] response;

            _pairs.Add("Token", _apiToken);

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
                client.UploadValues(_apiBaseUrl, _pairs);
            }

            // @todo сделать проверки на ошибки
        }
    }
}