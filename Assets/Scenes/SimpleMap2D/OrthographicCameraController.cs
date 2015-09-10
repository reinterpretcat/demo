using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Scenes.SimpleMap2D
{
    /// <summary> Sets camera settings needed by 2D mode. </summary>
    class OrthographicCameraController: MonoBehaviour
    {
        public Camera CameraScene;
        
        void Start()
        {
            // make camera to be north oriented.
            CameraScene.transform.rotation = Quaternion.Euler(90, 0, 0);

            // tweak settings once any tile is loaded
            ApplicationManager.Instance.GetService<IMessageBus>()
                .AsObservable<TileLoadFinishMessage>()
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    // adjust camera position
                    var position = transform.position;
                    var elevation = ApplicationManager.Instance.GetService<IElevationProvider>()
                        .GetElevation(new Vector2d(position.x, position.z));
                    transform.position = new Vector3(position.x, elevation + 300, position.z);

                    CameraScene.orthographicSize = transform.position.y;

                    // adjust viewport
                    var viewportHeight = CameraScene.orthographicSize * 2;
                    var viewportWidth = CameraScene.aspect * viewportHeight;

                    ApplicationManager.Instance.GetService<ITileController>().Viewport = 
                        new Rectangle2d(0, 0, viewportWidth, viewportHeight);
                });
        }
    }
}
