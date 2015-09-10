using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.MapEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Character
{
    /// <summary> 
    ///     This behaviour shows main idea how to use unity multiplayer feature with ASM
    ///     framework. So far is not in any scene, but kept as reference implementation
    /// </summary>
    public class MultiplayerBehaviour : NetworkBehaviour
    {
        private NetworkClient _client;
        private EditorController _editorController;
        private IMessageBus _messageBus;

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

            _editorController = new EditorController(appManager.GetService<ITileModelEditor>());

            SubscribeToLocalEvents();
            SubscribeToNetworkEvents();
        }

        #region Event processing

        private void SubscribeToLocalEvents()
        {
            _messageBus.AsObservable<TerrainPointMessage>().Subscribe(msg => _client.Send(msg.Id, msg));
            _messageBus.AsObservable<TerrainPolylineMessage>().Subscribe(msg => _client.Send(msg.Id, msg));
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

                _client.RegisterHandler(TerrainPolylineMessage.MsgId, msg =>
                {
                    var message = msg.ReadMessage<TerrainPolylineMessage>();
                    // building
                    if (message.ActionMode == EditorActionMode.AddBuilding)
                        _editorController.AddBuilding(message.Polyline);
                    // barrier
                    else if (message.ActionMode == EditorActionMode.AddBarrier)
                        _editorController.AddBarrier(message.Polyline);
                });
            }

            if (isServer)
            {
                NetworkServer.RegisterHandler(TerrainPointMessage.MsgId, msg =>
                    NetworkServer.SendToAll(TerrainPointMessage.MsgId, msg.ReadMessage<TerrainPointMessage>()));

                NetworkServer.RegisterHandler(TerrainPolylineMessage.MsgId, msg =>
                    NetworkServer.SendToAll(TerrainPolylineMessage.MsgId, msg.ReadMessage<TerrainPolylineMessage>()));
            }
        }

        #endregion
    }
}