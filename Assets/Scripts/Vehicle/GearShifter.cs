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
                    1 => 25,
                    2 => 50,
                    3 => 75,
                    4 => 110,
                    5 => 150,
                    6 => 160,
                    -1 => 15, // doesn't work
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