using UnityEngine;

namespace Vehicle
{
    public class VehicleController : MonoBehaviour
    {
        
        // [Tooltip("Set ref in order of FL, FR, RL, RR")]
        // [SerializeField] private WheelCollider[] wheelColliders;

        // [Tooltip("Set ref of wheel meshes in order of  FL, FR, RL, RR")]
        
        [SerializeField] private InputController inputCtrl;
        [SerializeField] private WheelControl[] wheels;
        [SerializeField] private Transform centerOfMass;

        // public int force;
        // public int angle;
        // public int brakeForce;

        [SerializeField] private float motorTorque = 2000;
        [SerializeField] private float brakeTorque = 2000;
        [SerializeField] private float steeringRange = 30;
        [SerializeField] private float steeringRangeAtMaxSpeed = 10;
        [SerializeField] private float maxSpeed = 20;
        
        private Rigidbody _rb;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rb.centerOfMass = centerOfMass.localPosition;
            // Calculate current speed in relation to the forward direction of the car
            // (this returns a negative number when traveling backwards)
            float forwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);


            // Calculate how close the car is to top speed
            // as a number from zero to one
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

            // Use that to calculate how much torque is available 
            // (zero torque at top speed)
            float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

            // …and to calculate how much to steer 
            // (the car steers more gently at top speed)
            float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            // Check whether the user input is in the same direction 
            // as the car's velocity
            bool isAccelerating = Mathf.Sign(inputCtrl.Vertical) == Mathf.Sign(forwardSpeed);
            
            // Steer();
            // Drive(currentMotorTorque);
            // Brake(brakeTorque);
            // UpdateWheelMovements();

            foreach (WheelControl wheel in wheels)
            {
                // Apply steering to Wheel colliders that have "Steerable" enabled
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = inputCtrl.Horizontal * currentSteerRange;
                }

                if (isAccelerating)
                {
                    // Apply torque to Wheel colliders that have "Motorized" enabled
                    if (wheel.motorized)
                    {
                        wheel.WheelCollider.motorTorque = inputCtrl.Vertical * currentMotorTorque;
                    }

                    wheel.WheelCollider.brakeTorque = 0;
                }
                else
                {
                    // If the user is trying to go in the opposite direction
                    // apply brakes to all wheels
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(inputCtrl.Vertical) * brakeTorque;
                    wheel.WheelCollider.motorTorque = 0;
                }
            }
        }
	
        //Drive forward/backward
        // private void Drive(float torque)
        // {
        //     wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = inputCtrl.Vertical * torque;
        //     wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = 0;
        // }
    
        //Steer left/right
        // private void Steer()
        // {
        //     wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = inputCtrl.Horizontal * angle;
        // }

        //Apply brakes
        // private void Brake(float torque)
        // {
        //     wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = inputCtrl.Brake * torque;
        // }
        //
        // //imitate the wheelcollider movements onto the wheel-meshes
        // private void UpdateWheelMovements()
        // {
        //     for (var i = 0; i < wheels.Length; i++)
        //     {
        //         Vector3 pos;
        //         Quaternion rot;
        //         wheelColliders[i].GetWorldPose(out pos, out rot);
        //         wheels[i].transform.position = pos;
        //         wheels[i].transform.rotation = rot;
        //     }
        // }
    }
}