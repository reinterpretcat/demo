using System.IO;
using System.Threading;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Positioning;
using ActionStreetMap.Core.Positioning.Nmea;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Utils;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Scenes.GpsTracking
{
    /// <summary>
    ///     Demonstrates how to react on location changes. Location signal is
    ///     simulated by "playing" nmea file. This can be useful to test related 
    ///     functionality without using real API. 
    /// </summary>
    public class GpsTrackBehaviour : MonoBehaviour
    {
        private const string CategoryName = "gps.trace";

        private IPositionObserver<GeoCoordinate> _geoPositionObserver;
        private IMessageBus _messageBus;
        private ITrace _trace;
        private NmeaPositionMocker _mocker;

        public string GpsTrackFile = @"Assets\Resources\Data\berlin_invalidenstr_chausseestr_tieckstr_borsigstr.nme";

        void Awake()
        {
            var appManager = ApplicationManager.Instance;
            _geoPositionObserver = appManager.GetService<ITileController>();
            _messageBus = appManager.GetService<IMessageBus>();
            _trace = appManager.GetService<ITrace>();

            var elevationProvider = appManager.GetService<IElevationProvider>();

            _messageBus.AsObservable<GeoPosition>()
                .SubscribeOn(Scheduler.ThreadPool)
                .Do(position =>
                {
                    _trace.Info(CategoryName, "GeoPosition: {0}", position.ToString());
                    // notify ASM about position change to process tiles
                    _geoPositionObserver.OnNext(position.Coordinate);
                    // calculate new world position
                    var mapPoint = GeoProjection.ToMapCoordinate(appManager.Coordinate, position.Coordinate);
                    var elevation = elevationProvider.GetElevation(position.Coordinate);
                    var worldPosition = new Vector3((float)mapPoint.X, elevation, (float)mapPoint.Y);
                    // set transform on UI thread
                    Observable.Start(() => transform.position = worldPosition, Scheduler.MainThread);

                }).Subscribe();

            _messageBus.AsObservable<TileLoadFinishMessage>()
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    Observable.Start(() =>
                    {
                        // read nmea file with gps data
                        using (Stream stream = new FileStream(GpsTrackFile, FileMode.Open))
                        {
                            _trace.Info(CategoryName, "start to read geopositions from {0}", GpsTrackFile);

                            _mocker = new NmeaPositionMocker(stream, _messageBus);
                            _mocker.OnDone += (s, e) => _trace.Info(CategoryName, "trace is finished");
                            _mocker.Start(Thread.Sleep);
                        }
                    }, Scheduler.ThreadPool);
                });
        }

        void OnDestroy()
        {
            if (_mocker != null && _mocker.IsStarted)
                _mocker.Stop();
        }
    }
}