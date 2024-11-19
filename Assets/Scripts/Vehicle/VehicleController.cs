using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Vehicle
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private InputController inputCtrl;
        [SerializeField] private WheelControl[] wheels;
        [SerializeField] private Transform centerOfMass;

        [SerializeField] private float motorTorque = 2000;
        [SerializeField] private float brakeTorque = 2000;
        [SerializeField] private float steeringRange = 30;
        [SerializeField] private float steeringRangeAtMaxSpeed = 10;
        [SerializeField] private float maxSpeed = 20;

        private InputAction _setRearAction;
        private InputAction _shiftGearAction;
        private InputAction _setNeutralAction;
        
        private GearShifter _gearShifter;
        private Rigidbody _rb;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gearShifter = GetComponent<GearShifter>();
            _gearShifter.CurrentGear = 0; // neutral
            
            _setRearAction = InputSystem.actions.FindAction("SetRear");
            _shiftGearAction = InputSystem.actions.FindAction("ShiftGear");
            _setNeutralAction = InputSystem.actions.FindAction("SetNeutral");
        }

        private void FixedUpdate()
        {
            _rb.centerOfMass = centerOfMass.localPosition;
            // Calculate current speed in relation to the forward direction of the car
            // (this returns a negative number when traveling backwards)
            float forwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);
            
            float maxSpeedInKms = maxSpeed * 3.6f;
            float kilosPerHour = Mathf.Clamp(forwardSpeed * 3.6f, 0, maxSpeedInKms);
            // Calculate how close the car is to top speed
            // as a number from zero to one
            maxSpeed = _gearShifter.maxSpeedInKms / 3.6f;
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);
            // Use that to calculate how much torque is available 
            // (zero torque at top speed)
            float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
            // Debug.Log(currentMotorTorque);
            // …and to calculate how much to steer 
            // (the car steers more gently at top speed)
            float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            // Check whether the user input is in the same direction 
            // as the car's velocity
            // bool isAccelerating = Mathf.Sign(inputCtrl.Vertical) == Mathf.Sign(forwardSpeed);
            
            foreach (WheelControl wheel in wheels)
            {
                // Apply steering to Wheel colliders that have "Steerable" enabled
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = inputCtrl.Horizontal * currentSteerRange;
                }

                if (_gearShifter.CurrentGear == 0) // neutral
                {
                    return;
                }
                
                if (_gearShifter.CurrentGear > 0)
                {
                    // Apply torque to Wheel colliders that have "Motorized" enabled
                    if (wheel.motorized)
                    {
                        wheel.WheelCollider.motorTorque = Mathf.Abs(inputCtrl.Vertical) * currentMotorTorque;
                    }

                    if (inputCtrl.Vertical < 0)
                    {
                        wheel.WheelCollider.brakeTorque = Mathf.Abs(inputCtrl.Vertical) * brakeTorque;
                    }
                    else
                    {
                        wheel.WheelCollider.brakeTorque = 0;
                    }
                }
                else // rear
                {
                    // If the user is trying to go in the opposite direction
                    // apply brakes to all wheels
                    if (inputCtrl.Vertical > 0)
                    {
                        wheel.WheelCollider.motorTorque = Mathf.Abs(inputCtrl.Vertical) * -currentMotorTorque;
                        wheel.WheelCollider.brakeTorque = 0;
                    }
                    else
                    {
                        wheel.WheelCollider.brakeTorque = Mathf.Abs(inputCtrl.Vertical) * brakeTorque;
                        wheel.WheelCollider.motorTorque = 0;
                    }
                }
            }
        }

        private void Update()
        {
            ControlInput();
        }
        
        private void ControlInput()
        {
            if (_setRearAction.triggered)
            {
                _gearShifter.CurrentGear = -1;
            }

            if (_shiftGearAction.triggered)
            {
                InputControl inputControl = _shiftGearAction.activeControl;
                _gearShifter.CurrentGear = Convert.ToInt32(inputControl.displayName);
            }

            if (_setNeutralAction.triggered)
            {
                _gearShifter.CurrentGear = 0;
            }
        }
    }
}