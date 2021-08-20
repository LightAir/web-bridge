using System.Xml;

namespace WebBridge.Tools
{
    public class ConfigTool
    {
        private readonly string _webToken;
        
        private readonly string _apiUrl;

        public bool IsSendUpdateEvent { get; set; } = false;
        public bool IsMessageModerate { get; set; } = false;

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

                var sendUpdateEventOption = doc.DocumentElement.SelectSingleNode("/Settings/IsSendUpdateEvent")?.InnerText;
                var messageModerateOption = doc.DocumentElement.SelectSingleNode("/Settings/IsMessageModerate")?.InnerText;

                IsSendUpdateEvent =  isOptionIsTrue(sendUpdateEventOption);

                IsMessageModerate = isOptionIsTrue(messageModerateOption);
                
                return;
            }

            Log.Error($"WebBridge Mod failed read settings from {configFilePath}");
        }

        private bool isOptionIsTrue(string option)
        {
            return option?.ToLower() == "true" || option == "1";
        }

        private void AddElement(XmlDocument doc, string name, string text)
        {
            XmlElement newElem = doc.CreateElement(name);
            newElem.InnerText = text;
            doc.DocumentElement?.AppendChild(newElem);
        }

        private void GenerateConfig(string configFilePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml("<Settings></Settings>");

                AddElement(doc, "ApiUrl", "https://localhost/api/v1/hooks");
                AddElement(doc, "WebToken", "Don't forget replace this token");
                AddElement(doc, "IsSendUpdateEvent", "false");
                AddElement(doc, "IsMessageModerate", "false");

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