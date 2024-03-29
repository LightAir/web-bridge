using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using JetBrains.Annotations;
using WebBridge.Enum;
using WebBridge.Tools;

namespace WebBridge
{
    public class EventHooks
    {
        private readonly HttpTool _httpTool;

        private readonly ConfigTool _configTool;

        public EventHooks(HttpTool httpTool, ConfigTool configTool)
        {
            _httpTool = httpTool;
            _configTool = configTool;
        }

        private NameValueCollection ClientInfoAsNameValueCollection([CanBeNull] ClientInfo clientInfo)
        {
            if (clientInfo == null)
            {
                return new NameValueCollection();
            }

            return new NameValueCollection()
            {
                {"EntityID", clientInfo.entityId.ToString()},
                {"PlayerName", clientInfo.playerName ?? string.Empty},
                {"CompatibilityVersion", clientInfo.compatibilityVersion ?? string.Empty},
                {"SteamID", clientInfo.playerId ?? string.Empty},
                {"SteamOwnerID", clientInfo.ownerId ?? string.Empty},
                {"IP", clientInfo.ip ?? string.Empty},
                {"Ping", clientInfo.ping.ToString()},
            };
        }

        private NameValueCollection HookTypeNameValueCollection(EnumHookType enumHookType)
        {
            return new NameValueCollection()
            {
                {"HookType", enumHookType.ToString()},
            };
        }

        public void HookGame(Enum.EnumGameState gameState)
        {
            _httpTool.Post(new NameValueCollection()
                {
                    HookTypeNameValueCollection(EnumHookType.Game),
                    {"GameStateType", gameState.ToString()},
                }
            );
        }

        public void HookUpdate()
        {
            _httpTool.Post(new NameValueCollection()
                {
                    HookTypeNameValueCollection(EnumHookType.Update),
                }
            );
        }

        public void HookPlayer(
            ClientInfo clientInfo,
            EnumGameMessages enumGameMessages,
            string message,
            string mainName,
            bool localizeMain,
            string secondaryName,
            bool localizeSecondary
        )
        {
            _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(clientInfo),
                    HookTypeNameValueCollection(EnumHookType.Player),
                    {"MessageType", enumGameMessages.ToString()},
                    {"Message", message ?? string.Empty},
                    {"MainName", mainName ?? string.Empty},
                    {"LocalizeMain", localizeMain.ToString()},
                    {"SecondaryName", secondaryName ?? string.Empty},
                    {"LocalizeSecondary", localizeSecondary.ToString()},
                }
            );
        }

        public void HookPlayer(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(clientInfo),
                    HookTypeNameValueCollection(EnumHookType.PlayerRespawn),
                    {"RespawnType", respawnType.ToString()},
                    {"Position", position.ToStringNoBlanks()},
                }
            );
        }

        public bool HookChat(
            ClientInfo clientInfo,
            EChatType eChatType,
            int senderId,
            string message,
            string mainName = null,
            bool localizeMain = false,
            List<int> recipientEntityIds = null
        )
        {
            var recipientEntityIdsString = string.Empty;

            if (recipientEntityIds != null)
            {
                recipientEntityIdsString = recipientEntityIds.ToString();
            }

            var response = _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(clientInfo),
                    HookTypeNameValueCollection(EnumHookType.Chat),
                    {"Message", message ?? string.Empty},
                    {"EChatType", eChatType.ToString()},
                    {"SenderId", senderId.ToString()},
                    {"MainName", mainName ?? string.Empty},
                    {"LocalizeMain", localizeMain.ToString()},
                    {"RecipientEntityIds", recipientEntityIdsString},
                }
            );
            
            return IsMessageModerateAndOk(response);
        }

        public bool HookChat(EChatType eChatType, string message)
        {
            var response = _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(null),
                    HookTypeNameValueCollection(EnumHookType.SystemChat),
                    {"Message", message ?? string.Empty},
                    {"EChatType", eChatType.ToString()},
                    {"SenderId", string.Empty},
                    {"MainName", string.Empty},
                    {"LocalizeMain", string.Empty},
                    {"RecipientEntityIds", string.Empty},
                }
            );

            return IsMessageModerateAndOk(response);
        }

        private bool IsMessageModerateAndOk(string response)
        {
            if (!_configTool.IsMessageModerate)
            {
                return true;
            }

            return response == "ok";
        }

        public void HookKill(Entity entity, Entity entitySecond)
        {
            string victimType = string.Empty;
            string assailantType = string.Empty;
            string victimId = string.Empty;
            string assailantId = string.Empty;

            if (entity != null)
            {
                victimType = entity.entityType.ToString();
                victimId = entity.entityId.ToString();
            }

            if (entitySecond != null)
            {
                assailantType = entitySecond.entityType.ToString();
                assailantId = entitySecond.entityId.ToString();
            }

            _httpTool.Post(new NameValueCollection()
                {
                    HookTypeNameValueCollection(EnumHookType.Kill),
                    {"VictimId", victimId},
                    {"VictimType", victimType},
                    {"AssailantId", assailantId},
                    {"AssailantType", assailantType},
                }
            );
        }

        public void HookPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(clientInfo),
                    HookTypeNameValueCollection(EnumHookType.Disconnect),
                    {"Shutdown", shutdown.ToString()},
                }
            );
        }

        public bool HookPlayerLogin(ClientInfo clientInfo, string compatibilityVersion, StringBuilder stringBuilder)
        {
            var response = _httpTool.Post(new NameValueCollection()
                {
                    ClientInfoAsNameValueCollection(clientInfo),
                    HookTypeNameValueCollection(EnumHookType.PlayerLogin),
                    {"CoVersion", compatibilityVersion},
                    {"StringBuilder", stringBuilder.ToString()},
                }
            );
            
            if (!_configTool.IsLoginControl)
            {
                return true;
            }

            return response == "ok";
        }
    }
}