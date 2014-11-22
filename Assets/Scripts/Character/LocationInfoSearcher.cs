using System;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Character
{
    /// <summary>
    ///     Naive implementation for displaying information about current position 
    ///     I think I have ideas how to improve that (IB)
    /// </summary>
    public class LocationInfoSearcher : MonoBehaviour
    {
        private float _scanFrequency = 3.0f;

        private GameObject _nearestObject;

        private void Start()
        {
            // set up repeating scan for new targets:
            InvokeRepeating("ScanForTarget", 0, _scanFrequency);
        }

        private void OnGUI()
        {
            if (_nearestObject != null)
            {
                var locationInfoHolder = _nearestObject.GetComponent<LocationInfoHolder>();
                var locationInfo = locationInfoHolder.Address;
                GUI.Box(new Rect(0, 0, 300, 30), locationInfo.Street);
            }
        }

        private void ScanForTarget()
        {
            // this should be called less often, because it could be an expensive
            // process if there are lots of objects to check against
            _nearestObject = GetNearestTaggedObject(this.gameObject, Consts.OsmTag);
        }

        /// <summary>
        /// Gets the nearest object to target with given tag
        /// </summary>
        public static GameObject GetNearestTaggedObject(GameObject target, string tag)
        {
            var nearestDistanceSqr = Mathf.Infinity;
            var taggedGameObjects = GameObject.FindGameObjectsWithTag(tag);

            GameObject nearestObj = null;
            foreach (var obj in taggedGameObjects)
            {
                // TODO add checking of collision for large areas/ways

                var position = obj.GetComponent<LocationInfoHolder>().Center;
                var distanceSqr = (position - target.transform.position).sqrMagnitude;

                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestObj = obj;
                    nearestDistanceSqr = distanceSqr;
                }

                /*var meshFilter = obj.GetComponentInChildren<MeshFilter>();
                // TODO investigate such cases
                // first: for empty models
                if (meshFilter == null || meshFilter.mesh == null) continue;
                
                // TODO hits performance a lot!!!
                foreach (var vertex in meshFilter.mesh.vertices)
                {
                    var position = new Vector3(vertex.x, vertex.y, vertex.z);

                    var distanceSqr = (position - target.transform.position).sqrMagnitude;

                    if (distanceSqr < nearestDistanceSqr)
                    {
                        nearestObj = obj;
                        nearestDistanceSqr = distanceSqr;
                    }
                }*/
            }

            return nearestObj;
        }
    }
}
