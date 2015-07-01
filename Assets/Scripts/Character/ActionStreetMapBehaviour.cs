using System;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer;
using UnityEngine;
using UnityEngine.Networking;
using RenderMode = ActionStreetMap.Core.RenderMode;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehaviour : NetworkBehaviour
    {
        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private ApplicationManager _appManager;
        private IMessageBus _messageBus;

        private float _initialGravity;

        // Use this for initialization
        void Start()
        {
            if (isLocalPlayer)
                Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer && _appManager.IsInitialized && _position != transform.position)
            {
                _position = transform.position;
                _appManager.Move(new MapPoint(_position.x, _position.z, _position.y));
            }
        }

        #region Initialization

        private void Initialize()
        {
            // wait for loading..
            //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            var thirdPersonControll = gameObject.GetComponent<ThirdPersonController>();
            _initialGravity = thirdPersonControll.gravity;
            thirdPersonControll.gravity = 0;

            _appManager = ApplicationManager.Instance;

            _messageBus = _appManager.GetService<IMessageBus>();

            _appManager.CreateConsole(true);

            _messageBus.AsObservable<GameRunner.GameStartedMessage>()
                .Where(msg => msg.Tile.RenderMode == RenderMode.Scene)
                .Take(1)
                .Delay(TimeSpan.FromSeconds(2)) // give extra seconds..
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var position = transform.position;
                    var elevation = _appManager.GetService<IElevationProvider>()
                        .GetElevation(new MapPoint(position.x, position.z));
                    transform.position = new Vector3(position.x, elevation + 90, position.z);
                    thirdPersonControll.gravity = _initialGravity;
                });

            // ASM should be started from non-UI thread
           Observable.Start(() => _appManager.RunGame(), Scheduler.ThreadPool);
        }

        #endregion
    }
}
