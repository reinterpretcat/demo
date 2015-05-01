using System;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Commands;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        /// <summary> This is start coordinate corresponds to World (0,0,0). </summary>
        private GeoCoordinate StartPosition = new GeoCoordinate(52.53208,13.38775);//(55.75282, 37.62259);
        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private IPositionObserver<MapPoint> _positionObserver;
        private IMessageBus _messageBus;
        private ITrace _trace;

        private bool _isInitialized = false;

        private Address _currentAddress;

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isInitialized && _position != transform.position)
            {
                _position = transform.position;
                Scheduler.ThreadPool.Schedule(() => 
                    _positionObserver.OnNext(new MapPoint(_position.x, _position.z, _position.y)));
            }
        }

        #region Initialization

        private void Initialize()
        {
            var appManager = ApplicationManager.Instance;

            _trace = appManager.GetService<ITrace>();
            _messageBus = appManager.GetService<IMessageBus>();

            // ASM should be started from non-UI thread
            Scheduler.ThreadPool.Schedule(() =>
            {
                try
                {
                    // Attach address locator which provides the way to get current address
                    AttachAddressLocator();

                    _positionObserver = appManager.GetService<ITilePositionObserver>();
                    appManager.Coordinate = StartPosition;
                    appManager.RunGame();

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
                    gameObject.AddComponent<AddressLocatorBehaviour>()
                        .SetCommandController(commandController)
                        .GetObservable()
                        .Subscribe(address => { _currentAddress = address; });
                });
        }

        #endregion

        private void OnGUI()
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
    }
}
