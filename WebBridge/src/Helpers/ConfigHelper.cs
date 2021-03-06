using System.Xml;

namespace WebBridge.Helpers
{
    public class ConfigHelper
    {
        private readonly string _webToken;
        private readonly string _apiUrl;

        public ConfigHelper(string _configFilePath)
        {
            Log.Out("---------------------------");
            Log.Out("WebBridge Mod read settings");
            Log.Out("---------------------------");

            if (!Utils.FileExists(_configFilePath))
            {
                GenerateConfig(_configFilePath);
                Log.Error($"WebBridge Mod. Please, configure settings file {_configFilePath}");
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(_configFilePath);
            }
            catch (XmlException e)
            {
                Log.Error($"WebBridge Mod failed loading {_configFilePath}: {e.Message}");

                return;
            }

            if (doc.DocumentElement != null)
            {
                _apiUrl = doc.DocumentElement.SelectSingleNode("/Settings/ApiUrl")?.InnerText;
                _webToken = doc.DocumentElement.SelectSingleNode("/Settings/WebToken")?.InnerText;

                return;
            }

            Log.Error($"WebBridge Mod failed read settings from {_configFilePath}");
        }

        private void GenerateConfig(string _configFilePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml("<Settings></Settings>");

                XmlElement newElem = doc.CreateElement("ApiUrl");
                newElem.InnerText = "https://localhost/api/v1/hooks";

                doc.DocumentElement?.AppendChild(newElem);
                
                XmlElement newElemToken = doc.CreateElement("WebToken");
                newElemToken.InnerText = "Don'tForgetReplaceThisToken";

                doc.DocumentElement?.AppendChild(newElemToken);

                XmlWriterSettings settings = new XmlWriterSettings {Indent = true};
                XmlWriter writer = XmlWriter.Create(_configFilePath, settings);

                doc.Save(writer);
            }
            catch (XmlException e)
            {
                Log.Error($"WebBridge Mod Exception: {e.Message}");
            }
        }

        public string GetWebToken()
        {
            return _webToken;
        }

        public string GetApiUrl()
        {
            return _apiUrl;
        }
    }
}