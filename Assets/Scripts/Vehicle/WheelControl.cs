using UnityEngine;

namespace Vehicle
{
    public class WheelControl : MonoBehaviour
    {
        public Transform wheelModel;

        [HideInInspector] public WheelCollider WheelCollider;

        // Create properties for the CarControl script
        // (You should enable/disable these via the 
        // Editor Inspector window)
        public bool steerable;
        public bool motorized;

        private Vector3 _position;
        private Quaternion _rotation;

        // Start is called before the first frame update
        private void Start()
        {
            WheelCollider = GetComponent<WheelCollider>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Get the Wheel collider's world pose values and
            // use them to set the wheel model's position and rotation
            WheelCollider.GetWorldPose(out _position, out _rotation);
            wheelModel.transform.position = _position;
            wheelModel.transform.rotation = _rotation;
        }
    }
}