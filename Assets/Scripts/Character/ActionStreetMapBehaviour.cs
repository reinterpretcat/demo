using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Maps.GeoCoding;
using UnityEngine;
using RenderMode = ActionStreetMap.Core.RenderMode;

namespace Assets.Scripts.Character
{
    /// <summary> Performs some initialization and listens for position changes of character.  </summary>
    public class ActionStreetMapBehaviour : MonoBehaviour
    {
        private ApplicationManager _appManager;

        private float _initialGravity;
        private IMessageBus _messageBus;

        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        /// <summary>
        ///     Place name. Will be resolved to the certain GeoCoordinate via performing reverse
        ///     geocoding request to reverse geocoding server.
        /// </summary>
        public string PlaceName;

        public RenderMode RenderMode = RenderMode.Scene;

        /// <summary> Start latitude. Used if PlaceName is empty. </summary>
        public double StartLatitude = 52.5209;

        /// <summary> Start longitude. Used if PlaceName is empty. </summary>
        public double StartLongitude = 13.40793;

        /// <summary> Size of tile. </summary>
        public float TileSize = 180;

        private void Start()
        {
            // set gravity to zero to prevent free fall as terrain loading takes some time.
            var thirdPersonControll = gameObject.GetComponent<ThirdPersonController>();
            _initialGravity = thirdPersonControll.gravity;
            thirdPersonControll.gravity = 0;

            _appManager = ApplicationManager.Instance;
            _appManager.InitializeFramework(GetConfigBuilder());

            SetStartGeoCoordinate();

            _messageBus = _appManager.GetService<IMessageBus>();

            // restore gravity and adjust character y-position once first scene tile is loaded
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

        /// <summary>
        ///     Sets start geocoordinate using desired method: direct latitude/longitude or
        ///     via reverse geocoding request for given place name.
        /// </summary>
        private void SetStartGeoCoordinate()
        {
            var coordinate = new GeoCoordinate(StartLatitude, StartLongitude);
            if (!String.IsNullOrEmpty(PlaceName))
            {
                // NOTE this will freeze UI thread as we're making web request and should wait for its result
                var place = _appManager.GetService<IGeocoder>().Search(PlaceName)
                    .Wait();

                if (place != null)
                    coordinate = place.Coordinate;
                else
                    _appManager.GetService<ITrace>()
                        .Warn("init", "Cannot resolve '{0}', will use default latitude/longitude", PlaceName);
            }
            _appManager.Coordinate = coordinate;
        }

        private void Update()
        {
            if (_appManager.IsInitialized && _position != transform.position)
            {
                _position = transform.position;
                _appManager.Move(new Vector2d(_position.x, _position.z));
            }
        }

        /// <summary> Returns config builder initialized with user defined settings. </summary>
        private ConfigBuilder GetConfigBuilder()
        {
            return ConfigBuilder.GetDefault()
                .SetTileSettings(TileSize, 40)
                .SetRenderOptions(RenderMode, new Rectangle2d(0, 0, TileSize*3, TileSize*3));
        }
    }
}