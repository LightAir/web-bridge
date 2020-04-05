using System.Collections.Generic;
using System.IO;
using WebBridge.Helpers;
using WebBridge.Tools;

namespace WebBridge
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedType.Global
    public class API : IModApi
    {
        private EventHooks _eventHooks;

        private static readonly string GamePath = Directory.GetCurrentDirectory();
        private static readonly string ConfigFilePath = $"{GamePath}/Mods/WebBridge/WebBridge.xml";

        public void InitMod()
        {
            ConfigHelper configHelper = new ConfigHelper(ConfigFilePath);
            HttpHelper httpHelper = new HttpHelper(configHelper.GetApiUrl(), configHelper.GetWebToken());

            _eventHooks = new EventHooks(httpHelper);

            ModEvents.GameAwake.RegisterHandler(GameAwake);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.GameMessage.RegisterHandler(GameMessage);

            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
            ModEvents.EntityKilled.RegisterHandler(EntityKilled);
        }

        /**
         * Runs once when the server is ready for interaction and GameManager.Instance.World is set
         */
        private void GameAwake()
        {
            _eventHooks.HookGame(Enum.EnumGameState.GameAwake);
        }

        /**
         * Runs once when the server is ready for players to join
         */
        private void GameStartDone()
        {
            _eventHooks.HookGame(Enum.EnumGameState.StartDone);
        }

        /**
         * runs once when the server is about to shut down
         */
        private void GameShutdown()
        {
            _eventHooks.HookGame(Enum.EnumGameState.Shutdown);
        }

        private bool GameMessage(
            ClientInfo _clientInfo,
            EnumGameMessages _type,
            string _msg,
            string _mainName,
            bool _localizeMain,
            string _secondaryName,
            bool _localizeSecondary
        )
        {
            _eventHooks.HookPlayer(
                _clientInfo,
                _type,
                _msg,
                _mainName,
                _localizeMain,
                _secondaryName,
                _localizeSecondary
            );

            return true;
        }

        /**
         * runs each time a player spawns, including on login, respawn from death, and teleport
         */
        private void PlayerSpawnedInWorld(ClientInfo _clientInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            _eventHooks.HookPlayer(_clientInfo, _respawnReason, _pos);
        }

        /**
         * return true to pass the message on to the next mod, or if no other mods then it will output to chat.
         * return false to prevent the message from being passed on or output to chat
         */
        private bool ChatMessage(
            ClientInfo _clientInfo,
            EChatType _type,
            int _senderId,
            string _msg,
            string _mainName,
            bool _localizeMain,
            List<int> _recipientEntityIds
        )
        {
            if (_clientInfo == null)
            {
                _eventHooks.HookChat(_type, _msg);

                return true;
            }

            if (_recipientEntityIds == null)
            {
                _recipientEntityIds = new List<int>();
            }

            _eventHooks.HookChat(_clientInfo, _type, _senderId, _msg, _mainName, _localizeMain, _recipientEntityIds);

            return true;
        }

        private void EntityKilled(Entity _entity1, Entity _entity2)
        {
            _eventHooks.HookKill(_entity1, _entity2);
        }
    }
}