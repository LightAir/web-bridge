using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace WebBridge.Tools
{
    public class HttpTool
    {
        private readonly string _apiBaseUrl;

        private readonly string _apiToken;

        public HttpTool(string apiBaseUrl, string apiToken)
        {
            _apiBaseUrl = apiBaseUrl;
            _apiToken = apiToken;
        }

        public string Post(NameValueCollection pairs)
        {
            pairs.Add("Token", _apiToken);

            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
                    client.Headers.Add("Content-Type","application/x-www-form-urlencoded");
                    byte[] response = client.UploadValues(_apiBaseUrl, pairs);

                    return Encoding.ASCII.GetString(response);
                }
            } catch (WebException webEx) {
                Log.Out(webEx.ToString());
                if (webEx.Status == WebExceptionStatus.ConnectFailure) {
                    Log.Out("Server running?");
                }
            }

            return null;
        }
    }
}