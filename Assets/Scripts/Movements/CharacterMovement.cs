using UnityEngine;

namespace Assets.Scripts.Movements
{
    public class CharacterMovement : MonoBehaviour
    {
        public float speed = 5.0f;
        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

            transform.Translate(x, 0, z);
        }
    }

}