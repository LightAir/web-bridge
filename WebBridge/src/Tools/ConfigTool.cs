using System.Xml;

namespace WebBridge.Tools
{
    public class ConfigTool
    {
        private readonly string _webToken;
        private readonly string _apiUrl;

        public ConfigTool(string configFilePath)
        {
            Log.Out("---------------------------");
            Log.Out("WebBridge Mod read settings");
            Log.Out("---------------------------");

            if (!Utils.FileExists(configFilePath))
            {
                GenerateConfig(configFilePath);
                Log.Error($"WebBridge Mod. Please, configure settings file {configFilePath}");
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(configFilePath);
            }
            catch (XmlException e)
            {
                Log.Error($"WebBridge Mod failed loading {configFilePath}: {e.Message}");

                return;
            }

            if (doc.DocumentElement != null)
            {
                _apiUrl = doc.DocumentElement.SelectSingleNode("/Settings/ApiUrl")?.InnerText;
                _webToken = doc.DocumentElement.SelectSingleNode("/Settings/WebToken")?.InnerText;

                return;
            }

            Log.Error($"WebBridge Mod failed read settings from {configFilePath}");
        }

        private void GenerateConfig(string configFilePath)
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
                XmlWriter writer = XmlWriter.Create(configFilePath, settings);

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