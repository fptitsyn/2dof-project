using UnityEngine;

namespace Vehicle
{
    public class AntiRollBar : MonoBehaviour
    {
        [SerializeField] private WheelCollider wheelFrontLeft;
        [SerializeField] private WheelCollider wheelFrontRight;
        [SerializeField] private WheelCollider wheelRearLeft;
        [SerializeField] private WheelCollider wheelRearRright;
        [SerializeField] private float antiRoll = 5000.0f;

        private Rigidbody _rb;

        void Start(){
            _rb = GetComponent<Rigidbody> ();
        }

        void FixedUpdate()
        {
            StabilizeAxle(wheelFrontLeft, wheelFrontRight);
            StabilizeAxle(wheelRearLeft, wheelRearRright);
        }

        private void StabilizeAxle(WheelCollider leftWheel, WheelCollider rightWheel)
        {
            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;
            
            bool groundedL = leftWheel.GetGroundHit (out hit);
            if (groundedL) {
                travelL = (-leftWheel.transform.InverseTransformPoint (hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
            }

            bool groundedR = rightWheel.GetGroundHit (out hit);
            if (groundedR) {
                travelR = (-rightWheel.transform.InverseTransformPoint (hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
            }

            float antiRollForce = (travelL - travelR) * antiRoll;

            if (groundedL)
                _rb.AddForceAtPosition (leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);

            if (groundedR)
                _rb.AddForceAtPosition (rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }
}