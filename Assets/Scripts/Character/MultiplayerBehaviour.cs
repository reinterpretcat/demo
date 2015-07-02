using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.MapEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Character
{
    public class MultiplayerBehaviour: NetworkBehaviour
    {
        private IMessageBus _messageBus;
        private EditorController _editorController;
        private NetworkClient _client;

        public override void OnStartLocalPlayer()
        {
            GetComponent<ThirdPersonController>().enabled = true;

            var mouseOrbit = Camera.main.GetComponent<MouseOrbit>();
            mouseOrbit.target = transform;
            mouseOrbit.enabled = true;
        }

        private void Start()
        {
            var appManager = ApplicationManager.Instance;
            _messageBus = appManager.GetService<IMessageBus>();
            _client = NetworkManager.singleton.client;

            _editorController = new EditorController(
                appManager.GetService<ITileController>(),
                appManager.GetService<ITileModelEditor>(),
                appManager.GetService<ITrace>());

            SubscribeToLocalEvents();
            SubscribeToNetworkEvents();
        }

        #region Event processing

        private void SubscribeToLocalEvents()
        {
            _messageBus.AsObservable<TerrainPointMessage>().Subscribe(msg => _client.Send(msg.Id, msg));
        }

        private void SubscribeToNetworkEvents()
        {           
            if (isClient)
            {
                _client.RegisterHandler(TerrainPointMessage.MsgId, msg =>
                {
                    var message = msg.ReadMessage<TerrainPointMessage>();
                    // tree
                    if (message.ActionMode == EditorActionMode.AddTree)
                        _editorController.AddTree(message.Point);
                    // terrain
                    else if (message.ActionMode == EditorActionMode.TerrainUp ||
                        message.ActionMode == EditorActionMode.TerrainDown)
                        _editorController.ModifyTerrain(message.Point, 
                            message.ActionMode == EditorActionMode.TerrainUp);
                });
            }

            if (isServer)
            {
                NetworkServer.RegisterHandler(TerrainPointMessage.MsgId, msg =>
                {
                    NetworkServer.SendToAll(TerrainPointMessage.MsgId, msg.ReadMessage<TerrainPointMessage>());
                });
            }
        }

        #endregion
    }
}
