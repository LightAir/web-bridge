using System.Collections.Generic;
using System.Collections.Specialized;
using WebBridge.Enum;
using WebBridge.Helpers;

namespace WebBridge.Tools
{
    public class EventHooks
    {
        private readonly HttpHelper _httpHelper;

        public EventHooks(HttpHelper _httpHelper)
        {
            this._httpHelper = _httpHelper;
        }

        public void HookGame(Enum.EnumGameState _gameState)
        {
            NameValueCollection collection = new NameValueCollection()
            {
                {"HookType", EnumHookType.GameHook.ToString()},
                {"MessageType", _gameState.ToString()},
            };

            _httpHelper.Post(collection);
        }

        public void HookPlayer(ClientInfo _clientInfo,
            EnumGameMessages _type,
            string _msg,
            string _mainName,
            bool _localizeMain,
            string _secondaryName,
            bool _localizeSecondary)
        {
            NameValueCollection collection = new NameValueCollection()
            {
                {"HookType", EnumHookType.PlayerHook.ToString()},
                {"Message", _msg ?? string.Empty},
                {"MessageType", _type.ToString()},
                {"PlayerName", _clientInfo.playerName ?? string.Empty},
                {"SteamID", _clientInfo.playerId ?? string.Empty},
                {"SteamOwnerID", _clientInfo.ownerId ?? string.Empty},
                {"IP", _clientInfo.ip ?? string.Empty},
                {"Ping", _clientInfo.ping.ToString()},
                {"EntityID", _clientInfo.entityId.ToString()},
                {"MainName", _mainName ?? string.Empty},
                {"LocalizeMain", _localizeMain.ToString()},
                {"SecondaryName", _secondaryName ?? string.Empty},
                {"LocalizeSecondary", _localizeSecondary.ToString()},
            };

            _httpHelper.Post(collection);
        }

        public void HookPlayer(ClientInfo _clientInfo, RespawnType _type, Vector3i _position)
        {
            NameValueCollection collection = new NameValueCollection()
            {
                {"HookType", EnumHookType.PlayerRespawnHook.ToString()},
                {"RespawnType", _type.ToString()},
                {"Position", _position.ToStringNoBlanks()},
                {"PlayerName", _clientInfo.playerName ?? string.Empty},
                {"SteamID", _clientInfo.playerId ?? string.Empty},
                {"SteamOwnerID", _clientInfo.ownerId ?? string.Empty},
                {"IP", _clientInfo.ip ?? string.Empty},
                {"Ping", _clientInfo.ping.ToString()},
                {"EntityID", _clientInfo.entityId.ToString()},
            };

            _httpHelper.Post(collection);
        }

        public void HookChat(
            ClientInfo _clientInfo,
            EChatType _type,
            int _senderId,
            string _msg,
            string _mainName = null,
            bool _localizeMain = false,
            List<int> _recipientEntityIds = null
        )
        {
            NameValueCollection collection = new NameValueCollection()
            {
                {"Message", _msg?? string.Empty},
                {"HookType", EnumHookType.ChatHook.ToString()},
                {"EChatType", _type.ToString()},
                {"PlayerName", _clientInfo.playerName ?? string.Empty},
                {"SteamID", _clientInfo.playerId ?? string.Empty},
                {"SteamOwnerID", _clientInfo.ownerId ?? string.Empty},
                {"IP", _clientInfo.ip ?? string.Empty},
                {"Ping", _clientInfo.ping.ToString()},
                {"EntityID", _clientInfo.entityId.ToString()},
                {"SenderId", _senderId.ToString()},
                {"MainName", _mainName?? string.Empty},
                {"LocalizeMain", _localizeMain.ToString()},
                {"RecipientEntityIds", _recipientEntityIds.ToString()},
            };

            _httpHelper.Post(collection);
        }

        public void HookChat(EChatType _type, string _msg)
        {
            NameValueCollection collection = new NameValueCollection()
            {
                {"HookType", EnumHookType.SystemChatHook.ToString()},
                {"Message", _msg ?? string.Empty},
                {"EChatType", _type.ToString()},
                {"PlayerName", string.Empty},
                {"SteamID", string.Empty},
                {"SteamOwnerID", string.Empty},
                {"IP", string.Empty},
                {"Ping", string.Empty},
                {"EntityID", string.Empty},
                {"SenderId", string.Empty},
                {"MainName", string.Empty},
                {"LocalizeMain", string.Empty},
                {"RecipientEntityIds", string.Empty},
            };

            _httpHelper.Post(collection);
        }

        public void HookKill(Entity _entity, Entity _entitySecond)
        {
            string whoType = string.Empty;
            string whomType = string.Empty;
            string whoId = string.Empty;
            string whomId = string.Empty;
            
            if (_entity != null)
            {
                whoType = _entity.entityType.ToString();
                whoId = _entity.entityId.ToString();
            }
            
            if (_entitySecond != null)
            {
                whomType = _entitySecond.entityType.ToString();
                whomId = _entitySecond.entityId.ToString();
            }

            NameValueCollection collection = new NameValueCollection()
            {
                {"HookType", EnumHookType.KillHook.ToString()},
                {"WhoType", whoType},
                {"WhomType", whomType},
                {"WhoId", whoId},
                {"WhomId", whomId},
            };

            _httpHelper.Post(collection);
        }
    }
}