using System;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Commands;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;
using RenderMode = ActionStreetMap.Core.RenderMode;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        public Camera CameraScene;
        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private ApplicationManager _appManager;
        private IMessageBus _messageBus;
        private ITrace _trace;

        private bool _isInitialized = false;
        private bool _isStarted = false;
        private float _initialGravity;
        private Address _currentAddress;

        // Use this for initialization
        private void Start()
        {
            Initialize();
            CameraScene.enabled = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isInitialized && _position != transform.position)
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

            _trace = _appManager.GetService<ITrace>();
            _messageBus = _appManager.GetService<IMessageBus>();

            _appManager.CreateConsole(true);

            _messageBus.AsObservable<GameRunner.GameStartedMessage>()
                .Where(msg => msg.Tile.RenderMode == RenderMode.Scene)
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var position = transform.position;
                    var elevation = _appManager.GetService<IElevationProvider>()
                        .GetElevation(new MapPoint(position.x, position.z));
                    transform.position = new Vector3(position.x, elevation + 30, position.z);
                    thirdPersonControll.gravity = _initialGravity;
                });

            // ASM should be started from non-UI thread
            Scheduler.ThreadPool.Schedule(() =>
            {
                try
                {
                    // Attach address locator which provides the way to get current address
                    AttachAddressLocator();
                    _appManager.RunGame();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    _trace.Error("FATAL", ex, "Error running game:");
                    throw;
                }
            });
        }

        private void AttachAddressLocator()
        {
            var commandController = ApplicationManager.Instance
                .GetService<CommandController>();

            _messageBus
                .AsObservable<GameRunner.GameStartedMessage>()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    _isStarted = true;
                    gameObject.AddComponent<AddressLocatorBehaviour>()
                        .SetCommandController(commandController)
                        .GetObservable()
                        .Subscribe(address => { _currentAddress = address; });
                });
        }

        #endregion

        private void OnGUI()
        {
            DrawAddressInfo();
            DrawOverviewButton();
        }

        #region UI controls

        private void DrawAddressInfo()
        {
            var address = _currentAddress;
            if (address != null)
            {
                String addressString = address.Street;

                if (!String.IsNullOrEmpty(address.Name))
                    addressString += String.Format(", {0}", address.Name);

                if (!String.IsNullOrEmpty(address.Code))
                    addressString += String.Format(", {0}", address.Code);

                GUI.Box(new Rect(0, 0, 400, 30), addressString);
            }
        }

        private void DrawOverviewButton()
        {
            if (!_isStarted) return;

            const int width = 200;
            bool isToOverview = !CameraScene.orthographic;
            var buttonLabel = isToOverview ? "3D Scene" : "2D Overview";
            if (GUI.Button(new Rect(Screen.width - width, 0, width, 30), buttonLabel))
            {
                CameraScene.orthographic = isToOverview;

                // disable MouseOrbit/ThirdPersonController script to prevent interference with animation
                if (isToOverview)
                {
                    CameraScene.GetComponent<MouseOrbit>().enabled = false;
                    gameObject.GetComponent<ThirdPersonController>().enabled = false;
                }

                // NOTE workarounds to keep overview north oriented
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                CameraScene.transform.rotation = Quaternion.Euler(90, 0, 0);

                // setup animation
                var cameraAnimation = CameraScene.GetComponent<CameraAnimation>();
                cameraAnimation.Play(2, isToOverview);
                Observable.FromEvent<EventHandler>(
                    g => OnFinishAnimation,
                    h => cameraAnimation.Finished += h,
                    h => cameraAnimation.Finished -= h)
                    .Take(1)
                    .Subscribe();
            }
        }

        private void OnFinishAnimation(object sender, EventArgs args)
        {
            var viewportHeight = 1200f;
            var viewportWidth = 1200f;
            var isToOverview = CameraScene.orthographic;
            if (isToOverview)
            {
                viewportHeight = CameraScene.orthographicSize * 2;
                viewportWidth = CameraScene.aspect * viewportHeight;
            }
            CameraScene.GetComponent<OverviewMousePan>().enabled = isToOverview;
            CameraScene.GetComponent<MouseOrbit>().enabled = !isToOverview;
            gameObject.GetComponent<ThirdPersonController>().enabled = !isToOverview;
            
            _appManager.SwitchMode(isToOverview ? RenderMode.Overview : RenderMode.Scene,
                new MapRectangle(0, 0, viewportWidth, viewportHeight));
           _appManager.Move(new MapPoint(_position.x, _position.z, _position.y));
        }

        #endregion
    }
}
