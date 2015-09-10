using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using UnityEngine;
using RenderMode = ActionStreetMap.Core.RenderMode;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehaviour : MonoBehaviour
    {
        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private ApplicationManager _appManager;
        private IMessageBus _messageBus;

        private float _initialGravity;

        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (_appManager.IsInitialized && _position != transform.position)
            {
                _position = transform.position;
                _appManager.Move(new Vector2d(_position.x, _position.z));
            }
        }

        #region Initialization

        private void Initialize()
        {
            // wait for loading..
            var thirdPersonControll = gameObject.GetComponent<ThirdPersonController>();
            _initialGravity = thirdPersonControll.gravity;
            thirdPersonControll.gravity = 0;

            _appManager = ApplicationManager.Instance;

            _messageBus = _appManager.GetService<IMessageBus>();

            _appManager.CreateConsole(true);

            _messageBus.AsObservable<TileLoadFinishMessage>()
                .Where(msg => msg.Tile.RenderMode == RenderMode.Scene)
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var position = transform.position;
                    var elevation = _appManager.GetService<IElevationProvider>()
                        .GetElevation(new Vector2d(position.x, position.z));
                    transform.position = new Vector3(position.x, elevation + 90, position.z);
                    thirdPersonControll.gravity = _initialGravity;
                });

            // ASM should be started from non-UI thread
           Observable.Start(() => _appManager.RunGame(), Scheduler.ThreadPool);
        }

        #endregion
    }
}
