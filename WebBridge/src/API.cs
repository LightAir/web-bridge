using System.Collections.Generic;
using System.IO;
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
            ConfigTool configTool = new ConfigTool(ConfigFilePath);
            HttpTool httpTool = new HttpTool(configTool.GetApiUrl(), configTool.GetWebToken());

            _eventHooks = new EventHooks(httpTool);

            ModEvents.GameAwake.RegisterHandler(GameAwake);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.GameMessage.RegisterHandler(GameMessage);
            ModEvents.GameUpdate.RegisterHandler(GameUpdate);

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
            _eventHooks.HookGame(Enum.EnumGameState.GameUpdate);
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
         * return true to pass the message on to the next mod, or if no other mods then it will output to chat.
         * return false to prevent the message from being passed on or output to chat
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
                _eventHooks.HookChat(eChatType, message);

                return true;
            }

            _eventHooks.HookChat(
                clientInfo,
                eChatType,
                senderId,
                message,
                mainName,
                localizeMain,
                recipientEntityIds ?? new List<int>()
            );

            return true;
        }

        private void EntityKilled(Entity entity1, Entity entity2)
        {
            _eventHooks.HookKill(entity1, entity2);
        }
    }
}