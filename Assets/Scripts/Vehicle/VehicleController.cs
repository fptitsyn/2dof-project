using System;
using System.Globalization;
using LogitechG29.Sample.Input;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Vehicle
{
    public class VehicleController : MonoBehaviour
    {
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
        [SerializeField] private TMP_Text handbrakeText;

        [SerializeField] private GameObject steeringWheel;
        [SerializeField] private GameObject headLights;
        
        [SerializeField] private InputActionReference shifter1Action;
        [SerializeField] private InputActionReference shifter2Action;
        [SerializeField] private InputActionReference shifter3Action;
        [SerializeField] private InputActionReference shifter4Action;
        [SerializeField] private InputActionReference shifter5Action;
        [SerializeField] private InputActionReference shifter6Action;
        [SerializeField] private InputActionReference shifter7Action;
        
        private GearShifter _gearShifter;
        private Rigidbody _rb;
    
        private bool _engineRunning;
        private bool _handBrakeApplied;

        [SerializeField] private InputControllerReader controller;

        // private bool _isPaused;

        [SerializeField] private GameObject pauseUI;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gearShifter = GetComponent<GearShifter>();
            _gearShifter.CurrentGear = 0; // neutral
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
            AnimateSteeringWheel();
        }

        private void Drive(float currentSteerRange, float currentMotorTorque)
        {
            foreach (WheelControl wheel in wheels)
            {
                // Apply steering to Wheel colliders that have "Steerable" enabled
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = controller.Steering * currentSteerRange;
                }

                if (!_engineRunning)
                {
                    return;
                }
                
                if (wheel.motorized)
                {
                    if (_handBrakeApplied) // handbrake
                    {
                        wheel.WheelCollider.brakeTorque = brakeTorque;
                        return;
                    }

                    if (_gearShifter.CurrentGear > 0)
                    {
                        wheel.WheelCollider.motorTorque = controller.Throttle * currentMotorTorque;
                        wheel.WheelCollider.brakeTorque = controller.Brake * brakeTorque;
                    }
                    else if (_gearShifter.CurrentGear == -1) // rear
                    {
                        wheel.WheelCollider.motorTorque = -controller.Throttle * currentMotorTorque;
                        wheel.WheelCollider.brakeTorque = controller.Brake * brakeTorque;
                    }
                }
            }
        }

        private void UpdateUi(float kilosPerHour, float rpm)
        {
            speedometerText.text = $"{Mathf.Round(kilosPerHour)} KM/H";
            rpmText.text = $"RPM: {Mathf.Round(rpm)}";
            if (_gearShifter.CurrentGear == 0)
            {
                gearText.text = "N";
            }
            else if (_gearShifter.CurrentGear == -1)
            {
                gearText.text = "R";
            }
            else
            {
                gearText.text = _gearShifter.CurrentGear.ToString();
            }

            handbrakeText.text = _handBrakeApplied ? "Handbrake: ON" : "Handbrake: OFF";
        }

        private void AnimateSteeringWheel()
        {
            float turnSpeed = 75f;
            
            if (-controller.Steering != 0)
            {
                // need to clamp rotation and reset the wheel gradually if there is no input
                steeringWheel.transform.Rotate(Vector3.forward, -controller.Steering * turnSpeed * Time.deltaTime);
            }
            else
            {
                // reset rotation
            }
        }
        
        private void OnEnable()
        {
            // Shifter
            shifter1Action.action.performed += HandleShifterCallback;
            shifter2Action.action.performed += HandleShifterCallback;
            shifter3Action.action.performed += HandleShifterCallback;
            shifter4Action.action.performed += HandleShifterCallback;
            shifter5Action.action.performed += HandleShifterCallback;
            shifter6Action.action.performed += HandleShifterCallback;
            shifter7Action.action.performed += HandleShifterCallback;
         
            // Buttons
            controller.OnReturnCallback += HandleOnReturnCallback;
            controller.HandbrakeCallback += HandleHandbrakeCallback;
            controller.OnWestButtonCallback += HandleWestCallback;
            controller.OnSouthButtonCallback += HandleSouthCallback;
            
            // UI
            controller.OnOptionsCallback += HandleOptionsCallback;
        }

        private void OnDisable()
        {
            // shifter
            shifter1Action.action.performed -= HandleShifterCallback;
            shifter2Action.action.performed -= HandleShifterCallback;
            shifter3Action.action.performed -= HandleShifterCallback;
            shifter4Action.action.performed -= HandleShifterCallback;
            shifter5Action.action.performed -= HandleShifterCallback;
            shifter6Action.action.performed -= HandleShifterCallback;
            shifter7Action.action.performed -= HandleShifterCallback;
            
            // Buttons
            controller.HandbrakeCallback -= HandleHandbrakeCallback;
            controller.OnWestButtonCallback -= HandleWestCallback;
            controller.OnSouthButtonCallback -= HandleSouthCallback;
            controller.OnReturnCallback -= HandleOnReturnCallback;
            
            // UI
            controller.OnOptionsCallback -= HandleOptionsCallback;
        }

        private void HandleOnReturnCallback(bool value) // start engine
        {
            if (value)
            {
                _engineRunning = true;
                tabletGUI.SetActive(!tabletGUI.activeSelf);
                Debug.Log("wheel enter");
            }
        }

        private void HandleShifterCallback(InputAction.CallbackContext context) // stupid shifter
        {
            string actionName = context.action.name;
            char lastCharacter = actionName[^1];
            if (lastCharacter == '7')
            {
                _gearShifter.CurrentGear = -1; // rear (can be handled better but legacy code what can you do)
            }
            else
            {
                _gearShifter.CurrentGear = Convert.ToInt32(lastCharacter) - 48;
            }
        }

        private void HandleHandbrakeCallback(float value) // confusing handbrake
        {
            _handBrakeApplied = !_handBrakeApplied;
        }

        private void HandleWestCallback(bool value) // headlights
        {
            if (value)
            {
                if (_engineRunning)
                {
                    headLights.SetActive(!headLights.activeSelf);
                }
            }
        }

        private void HandleSouthCallback(bool value) // neutral (for now)
        {
            if (value)
            {
                _gearShifter.CurrentGear = 0;
            }
        }

        private void HandleOptionsCallback(bool value)
        {
            if (value)
            {
                // pauseUI.SetActive(!pauseUI.activeSelf);
                PauseUI.TogglePause();
            }
        }
    }
}