using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Vehicle
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private InputController inputCtrl;
        [SerializeField] private WheelControl[] wheels;
        [SerializeField] private Transform centerOfMass;

        [SerializeField] private float motorTorque;
        [SerializeField] private float brakeTorque;
        [SerializeField] private float steeringRange;
        [SerializeField] private float steeringRangeAtMaxSpeed;

        [Header("Car GUI")]
        [SerializeField] private GameObject tabletGUI;
        [SerializeField] private TMP_Text speedometerText;
        [SerializeField] private TMP_Text rpmText;
        [SerializeField] private TMP_Text gearText;

        [SerializeField] private GameObject steeringWheel;
        
        // Input actions
        private InputAction _setRearAction;
        private InputAction _shiftGearAction;
        private InputAction _setNeutralAction;
        private InputAction _startEngineAction;
        
        private GearShifter _gearShifter;
        private Rigidbody _rb;
    
        private bool _engineRunning;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gearShifter = GetComponent<GearShifter>();
            _gearShifter.CurrentGear = 0; // neutral
            
            _setRearAction = InputSystem.actions.FindAction("SetRear");
            _shiftGearAction = InputSystem.actions.FindAction("ShiftGear");
            _setNeutralAction = InputSystem.actions.FindAction("SetNeutral");
            _startEngineAction = InputSystem.actions.FindAction("StartEngine");
        }

        private void FixedUpdate()
        {
            _rb.centerOfMass = centerOfMass.localPosition;
            
            if (!_engineRunning)
            {
                return;
            }
            
            // Calculate current speed in relation to the forward direction of the car
            // (this returns a negative number when traveling backwards)
            float forwardSpeed = Mathf.Abs(Vector3.Dot(transform.forward, _rb.linearVelocity));
            
            // Calculate how close the car is to top speed
            // as a number from zero to one
            float maxSpeed = _gearShifter.maxSpeedInKms / 3.6f;
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);
            
            // Use that to calculate how much torque is available 
            // (zero torque at top speed)
            float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
            
            // …and to calculate how much to steer 
            // (the car steers more gently at top speed)
            float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            Drive(currentSteerRange, currentMotorTorque);
            
            float rpm = motorTorque * speedFactor;
            float kilosPerHour = Mathf.Clamp(forwardSpeed * 3.6f, 0, _gearShifter.maxSpeedInKms);
            UpdateUi(kilosPerHour, rpm);
        }

        private void Update()
        {
            ControlInput();
            AnimateSteeringWheel();
        }

        private void Drive(float currentSteerRange, float currentMotorTorque)
        {
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

            if (_startEngineAction.triggered)
            {
                _engineRunning = true;
                tabletGUI.SetActive(!tabletGUI.activeSelf);
            }
        }

        private void UpdateUi(float kilosPerHour, float rpm)
        {
            speedometerText.text = Mathf.Round(kilosPerHour).ToString("00");
            rpmText.text = Mathf.Round(rpm).ToString();
            if (_gearShifter.CurrentGear == 0)
            {
                gearText.text = "N";
            }
            else if (_gearShifter.CurrentGear == -1)
            {
                Debug.Log(_gearShifter.maxSpeedInKms);
                gearText.text = "R";
            }
            else
            {
                gearText.text = _gearShifter.CurrentGear.ToString();
            }
        }

        private void AnimateSteeringWheel()
        {
            // float angle = Mathf.InverseLerp(-450, 450, inputCtrl.Horizontal);
            // need to clamp rotation and reset the wheel gradually if there is no input
            Vector3 rotationZ = new Vector3(0, 0, -inputCtrl.Horizontal);
            steeringWheel.transform.Rotate(rotationZ);
        }
    }
}