using ActionStreetMap.Explorer.Scene;
using Assets.Scripts.MapEditor.Behaviors;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class WeaponBehavior: MonoBehaviour
    {
        private const float Radius = 0.2f;
        private Transform _grenadeSpawnPoint;

        void Start()
        {
            _grenadeSpawnPoint = gameObject.transform.GetChild(0).transform;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var grenade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                grenade.transform.position = _grenadeSpawnPoint.position + new Vector3(0, 3, 0);
                grenade.transform.rotation = _grenadeSpawnPoint.rotation;
                grenade.AddComponent<GrenadeBehavior>();
                grenade.transform.localScale = new Vector3(Radius, Radius, Radius);
                grenade.AddComponent<Rigidbody>();
            }
        }

        #region Nested class

        private class GrenadeBehavior : MonoBehaviour
        {
            private const float Speed = 20.0f;
            private const float LifeTime = 10.0f;
            private const float ExplosionRadius = 5;
            
            private bool _isDestroyed;
            void Start()
            {
                Destroy(gameObject, LifeTime);
                GetComponent<Rigidbody>().AddForce(5, 5, 0, ForceMode.Impulse);
            }

            void Update()
            {
                transform.Translate(-Speed * Time.deltaTime, 0, 0);
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
                        ForceDirection = transform.forward,
                        ForcePower = 1,
                        OffsetThreshold = 1,
                    });
                Destroy(gameObject);
                _isDestroyed = true;
            }
        }

        #endregion
    }
}
