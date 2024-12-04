using System;
using LogitechG29.Sample.Input;
using UnityEngine;

namespace Vehicle
{
    public class SteeringWheel : MonoBehaviour
    {
        [SerializeField] float rotateBackSpeed = 10f; // degrees per second
        [SerializeField] float rotateSpeed = 50f;    // degrees per second
        [SerializeField] float angle = 0f;           // degrees
        [SerializeField] float minAngle = -120f;     // degrees
        [SerializeField] float maxAngle = 120f;      // degrees
        [SerializeField] float neutralAngle = 0f;    // degrees

        [SerializeField] private InputControllerReader controller;
        
        private void Update()
        {
            angle = Mathf.Clamp(angle + -controller.Steering * rotateSpeed * Time.deltaTime, minAngle, maxAngle);

            if (Mathf.Approximately(0f, -controller.Steering)) 
            {
                angle = Mathf.MoveTowardsAngle(angle, neutralAngle, 
                    rotateBackSpeed * Time.deltaTime);
            }
            
            transform.localEulerAngles = angle * Vector3.forward;
        }
    }
}