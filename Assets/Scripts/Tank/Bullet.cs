using UnityEngine;

namespace Assets.Scripts.TankDemo
{
    public class Bullet : MonoBehaviour
    {
        //Explosion Effect
        public GameObject Explosion;

        public float Speed = 60.0f;
        public float LifeTime = 3.0f;
        public int damage = 50;

        void Start()
        {
            Destroy(gameObject, LifeTime);
        }

        void Update()
        {
            transform.position += 
                transform.forward * Speed * Time.deltaTime;       
        }

        void OnCollisionEnter(Collision collision)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(Explosion, contact.point, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}