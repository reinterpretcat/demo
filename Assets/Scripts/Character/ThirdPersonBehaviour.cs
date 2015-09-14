using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class ThirdPersonBehaviour: MonoBehaviour
    {
        private float _initialGravity;

        void Start()
        {
            // set gravity to zero on start to prevent free fall as terrain loading takes some time.
            // restore it afterwards.

            var appManager = ApplicationManager.Instance;
            var thirdPersonController = gameObject.GetComponent<ThirdPersonController>();
            _initialGravity = thirdPersonController.gravity;
            thirdPersonController.gravity = 0;

            // restore gravity and adjust character y-position once first scene tile is loaded
            appManager.GetService<IMessageBus>().AsObservable<TileLoadFinishMessage>()
                .Where(msg => msg.Tile.RenderMode == ActionStreetMap.Core.RenderMode.Scene)
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var position = transform.position;
                    var elevation = appManager.GetService<IElevationProvider>()
                        .GetElevation(new Vector2d(position.x, position.z));
                    transform.position = new Vector3(position.x, elevation + 90, position.z);
                    thirdPersonController.gravity = _initialGravity;
                });
        }
    }
}
