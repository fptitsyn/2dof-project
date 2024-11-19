using UnityEngine;

namespace Vehicle
{
    public class GearShifter : MonoBehaviour
    {
        // private bool _isInNeutral;
        // private bool _isInRear;

        private int _currentGear;

        public float maxSpeedInKms;
        
        public int CurrentGear
        {
            set
            {
                _currentGear = value;
                maxSpeedInKms = value switch
                {
                    1 => 20,
                    2 => 40,
                    3 => 60,
                    4 => 90,
                    5 => 125,
                    6 => 160,
                    -1 => 15,
                    _ => 0
                };
                
                // switch (value)
                // {
                //     case 0:
                //         _isInNeutral = true;
                //         break;
                //     case -1:
                //         _isInRear = true;
                //         break;
                // }
            }
            get => _currentGear;
        }
    }
}