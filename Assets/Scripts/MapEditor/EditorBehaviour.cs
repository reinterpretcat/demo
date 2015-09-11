using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    internal class EditorBehaviour : MonoBehaviour
    {
        private EditorController _editorController;

        void Start()
        {
            var appManager = ApplicationManager.Instance;
            _editorController = new EditorController(appManager.GetService<ITileModelEditor>());

            var messageBus = appManager.GetService<IMessageBus>();
            messageBus.AsObservable<TerrainPointMessage>().Subscribe(message =>
            {
                // tree
                if (message.ActionMode == EditorActionMode.AddTree)
                    _editorController.AddTree(message.Point);
                // terrain
                else if (message.ActionMode == EditorActionMode.TerrainUp ||
                         message.ActionMode == EditorActionMode.TerrainDown)
                    _editorController.ModifyTerrain(message.Point,
                        message.ActionMode == EditorActionMode.TerrainUp);
            });
            messageBus.AsObservable<TerrainPolylineMessage>().Subscribe(message =>
            {
                // building
                if (message.ActionMode == EditorActionMode.AddBuilding)
                    _editorController.AddBuilding(message.Polyline);
                // barrier
                else if (message.ActionMode == EditorActionMode.AddBarrier)
                    _editorController.AddBarrier(message.Polyline);
            });
        }
    }
}