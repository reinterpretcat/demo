using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Core.Utils;
using ActionStreetMap.Explorer.Scene.Builders;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scenes.Customization
{
    public class WaypointModelBuilder: IModelBuilder
    {
        private readonly IElevationProvider _elevationProvider;
        private readonly IGameObjectFactory _gameObjectFactory;

        public string Name { get { return "waypoint"; } }

        [Dependency]
        public WaypointModelBuilder(IElevationProvider elevationProvider, 
            IGameObjectFactory gameObjectFactory)
        {
            _elevationProvider = elevationProvider;
            _gameObjectFactory = gameObjectFactory;
        }

        // NOTE we don't expect to use this model builder for area
        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            throw new NotImplementedException();
        }

        public IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            // create parent object for waypoint. Use factory to handle object creation on proper thread.
            var parent = _gameObjectFactory.CreateNew("waypoints " + way.Id, tile.GameObject);
            foreach (var coordinate in way.Points)
            {
                // detect position
                var position = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, coordinate);
                var elevation = _elevationProvider.GetElevation(coordinate);

                // create new gameobject on main thread.
                Observable.Start(() =>
                {
                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = new Vector3((float) position.X, elevation + 1, (float) position.Y);
                    sphere.transform.parent = parent.GetComponent<GameObject>().transform;
                }, Scheduler.MainThread);
            }
            // return null as we don't need to attach behaviours
            return null;
        }

        // NOTE we don't expect to use this model builder for node
        public IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            throw new NotImplementedException();
        }
    }
}
