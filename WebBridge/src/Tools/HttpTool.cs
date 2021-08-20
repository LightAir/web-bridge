using System.Collections.Specialized;
using System.Net;

namespace WebBridge.Tools
{
    public class HttpTool
    {
        private readonly string _apiBaseUrl;
        private readonly string _apiToken;

        public HttpTool(string apiBaseUrl, string apiToken)
        {
            this._apiBaseUrl = apiBaseUrl;
            this._apiToken = apiToken;
        }

        public void Post(NameValueCollection pairs)
        {
            pairs.Add("Token", _apiToken);

            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
                    client.UploadValues(_apiBaseUrl, pairs);
                }
            } catch (WebException webEx) {
                Log.Out(webEx.ToString());
                if (webEx.Status == WebExceptionStatus.ConnectFailure) {
                    Log.Out("Server running?");
                }
            }
        }
    }
}