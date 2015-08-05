using System;
using ActionStreetMap.Explorer.Scene;
using Assets.Scripts.MapEditor.Behaviors;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class WeaponBehavior: MonoBehaviour
    {
        private Transform _grenadeSpawnPoint;

        void Start()
        {
            _grenadeSpawnPoint = gameObject.transform.GetChild(0).transform;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                const float sphereRadius = 0.5f;
                var grenade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                grenade.transform.position = _grenadeSpawnPoint.position + new Vector3(0, 4, 0);
                grenade.transform.rotation = _grenadeSpawnPoint.rotation;
                grenade.AddComponent<GrenadeBehavior>();
 
                grenade.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                var sphereCollider = grenade.AddComponent<SphereCollider>();
                sphereCollider.center = Vector3.zero;
                sphereCollider.radius = sphereRadius * 10;
                
                var rigidBody = grenade.AddComponent<Rigidbody>();
                rigidBody.mass = 2;

                Physics.IgnoreCollision(sphereCollider, GetComponent<Collider>());
            }
        }

        #region Nested class

        private class GrenadeBehavior : MonoBehaviour
        {
            public float XPower = 100;
            public float YPower = -100;
            public float ZPower = 100;

            private const float LifeTime = 10.0f;
            private const float ExplosionRadius = 5;

            private Vector3 _direction;
            private bool _isDestroyed;
            void Start()
            {
                _direction = Camera.main.transform.TransformDirection(Vector3.forward).normalized;
                GetComponent<Rigidbody>().AddForce(
                    _direction.x * XPower, 
                    _direction.y * YPower, 
                    _direction.z * ZPower,
                    ForceMode.Impulse);

                Destroy(gameObject, LifeTime);
            }

            void Update()
            {
                if (_isDestroyed)
                    GetComponent<MeshRenderer>().enabled = false;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (_isDestroyed) return;

                ContactPoint contact = collision.contacts[0];
                BehaviourHelper.Modify(new MeshQuery()
                    {
                        Epicenter = contact.point,
                        Radius = ExplosionRadius,
                        ForceDirection = _direction,
                        OffsetThreshold = 2f,
                        GetForceChange = distance => 2 / ((float)Math.Pow(distance + 1, 1.67))
                    });
                Destroy(gameObject);
                _isDestroyed = true;
            }
        }

        #endregion
    }
}
