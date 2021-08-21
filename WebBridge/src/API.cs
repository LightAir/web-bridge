using System.Collections.Generic;
using System.IO;
using System.Text;
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

        private bool _isSendUpdateEvent;

        public void InitMod()
        {
            ConfigTool configTool = new ConfigTool(ConfigFilePath);
            HttpTool httpTool = new HttpTool(configTool.GetApiUrl(), configTool.GetWebToken());

            _isSendUpdateEvent = configTool.IsSendUpdateEvent;
            _eventHooks = new EventHooks(httpTool, configTool);

            ModEvents.GameAwake.RegisterHandler(GameAwake);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameUpdate.RegisterHandler(GameUpdate);
            ModEvents.GameMessage.RegisterHandler(GameMessage);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);

            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);

            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
            ModEvents.EntityKilled.RegisterHandler(EntityKilled);
        }

        /**
         * Runs once, when the server is ready to communicate and the world instance (GameManager.Instance.World) is set
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
         * Runs once when the server is about to shut down
         */
        private void GameShutdown()
        {
            _eventHooks.HookGame(Enum.EnumGameState.Shutdown);
        }

        /**
         * Executed when a game event occurs
         */
        private bool GameMessage(
            ClientInfo clientInfo,
            EnumGameMessages enumGameMessages,
            string message,
            string mainName,
            bool localizeMain,
            string secondaryName,
            bool localizeSecondary
        )
        {
            _eventHooks.HookPlayer(clientInfo, enumGameMessages, message, mainName, localizeMain, secondaryName, localizeSecondary);

            return true;
        }

        private void GameUpdate()
        {
            if (_isSendUpdateEvent)
            {
                _eventHooks.HookUpdate();
            }
        }

        /**
         * Executed when the user tries to login.
         * If the method returns false then the user will be Denied to logon to the server
         */
        private bool PlayerLogin(ClientInfo clientInfo, string compatibilityVersion, StringBuilder stringBuilder)
        {
            return _eventHooks.HookPlayerLogin(clientInfo, compatibilityVersion, stringBuilder);
        }

        /**
         * Runs every time a player is spawned, logged in, revived after death or teleportation
         */
        private void PlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnReason, Vector3i position)
        {
            _eventHooks.HookPlayer(clientInfo, respawnReason, position);
        }

        private void PlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            _eventHooks.HookPlayerDisconnected(clientInfo, shutdown);
        }

        /**
         * Executed when the user or the system sends a message to the chat room.
         * Return True to pass the message on to the next mod, or if no other mods then it will output to chat.
         * Return False to prevent the message from being passed on or output to chat
         */
        private bool ChatMessage(
            ClientInfo clientInfo,
            EChatType eChatType,
            int senderId,
            string message,
            string mainName,
            bool localizeMain,
            List<int> recipientEntityIds
        )
        {
            if (clientInfo == null)
            {
                return _eventHooks.HookChat(eChatType, message);
            }

            return _eventHooks.HookChat(
                clientInfo,
                eChatType,
                senderId,
                message,
                mainName,
                localizeMain,
                recipientEntityIds ?? new List<int>()
            );
        }

        /**
         * Executed when the killed event occurred
         */
        private void EntityKilled(Entity entity1, Entity entity2)
        {
            _eventHooks.HookKill(entity1, entity2);
        }
    }
}