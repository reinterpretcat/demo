using UnityEngine;

namespace Assets.Scripts.TouchKit.Controls
{
    /// <summary>
    /// this demo will create a virtual d-pad and two buttons. the virtual d-pad will have a left, right and up button. the up
    /// button will overlap the left and right buttons. the reason for this is to show one way to handle 5 directional d-pad
    /// (left, up-left, up, up-right, right) letting the player slide their finger around. It alsow demonstrates how the TKAnyTouchRecognizer
    /// does not "eat" touches and allows them to bleed down through to recognizers below them.
    ///
    /// note that we are working with a 16:9 design time resolution since that is the most popular on mobile and a good base. Note also
    /// that we have set shouldAutoUpdateTouches to false and we are manually calling updateTouches. This lets us process the touches
    /// exactly when we want: right before we use them.
    /// </summary>
    public class TouchController : MonoBehaviour
    {
        private VirtualControls _controls;
        //private CharacterController _characterController;

        private float speed = 50.0f;
        private float  jumpSpeed = 0.0f;
        private float  inAirMultiplier = 0.0f;
        private float  gravity = 20.0f;

        private Vector3 velocity;

        private void Start()
        {
            _controls = new VirtualControls();
           // _characterController = GetComponent<CharacterController>();
        }


        private void Update()
        {
            _controls.update();

            float x = 0;
            float z = 0;
            if (_controls.leftDown)
                x = -1;
            if (_controls.rightDown)
                x = 1;

            if (_controls.upDown)
                z = 1;


            var directionVector = new Vector3(x, 0, z);
            if (directionVector != Vector3.zero)
            {
                // Get the length of the directon vector and then normalize it
                // Dividing by the length is cheaper than normalizing when we already have the length anyway
                var directionLength = directionVector.magnitude;
                directionVector = directionVector / directionLength;

                // Make sure the length is no bigger than 1
                directionLength = Mathf.Min(1, directionLength);

                // Make the input vector more sensitive towards the extremes and less sensitive in the middle
                // This makes it easier to control slow speeds when using analog sticks
                directionLength = directionLength * directionLength;

                // Multiply the normalized direction vector by the modified length
                directionVector = directionVector * directionLength;
            }

            // Apply the direction to the CharacterMotor
            var moveDirection = transform.rotation * directionVector;

            var position = moveDirection*Time.deltaTime*speed;

            //_characterController.Move(moveDirection * Time.deltaTime);
            transform.Translate(position.x, 0, position.z);
        }


        void OnGUI()
        {
            showLabelAndValue( "Left: ", _controls.leftDown.ToString() );
            showLabelAndValue( "Right: ", _controls.rightDown.ToString() );
            showLabelAndValue( "Up: ", _controls.upDown.ToString() );

            GUILayout.Space( 4 );

            showLabelAndValue( "Attack: ", _controls.attackDown.ToString() );
            showLabelAndValue( "Jump: ", _controls.jumpDown.ToString() );
        }


        void showLabelAndValue( string label, string value )
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( label, GUILayout.Width( 50 ) );
                GUILayout.Label( value );
            }
            GUILayout.EndHorizontal();
        }

    }
}
