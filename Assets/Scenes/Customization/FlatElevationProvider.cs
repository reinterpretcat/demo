using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Infrastructure.Reactive;

namespace Assets.Scenes.Customization
{
    /// <summary>
    ///     Implements elevation provider which return zero
    ///     elevation for any coordinate.
    /// </summary>
    public class FlatElevationProvider : IElevationProvider
    {
        public bool HasElevation(BoundingBox bbox)
        {
            return true;
        }

        public IObservable<Unit> Download(BoundingBox bbox)
        {
            throw new NotImplementedException();
        }

        public float GetElevation(GeoCoordinate coordinate)
        {
            return 0;
        }

        public float GetElevation(Vector2d point)
        {
            return 0;
        }

        public float GetElevation(float x, float y)
        {
            return 0;
        }

        public void SetNullPoint(GeoCoordinate coordinate)
        {
        }
    }
}