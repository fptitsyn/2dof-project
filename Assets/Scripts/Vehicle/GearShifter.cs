using UnityEngine;

namespace Vehicle
{
    public class GearShifter : MonoBehaviour
    {
        private bool _isInNeutral;
        private bool _isInRear;

        private int _currentGear;

        public int CurrentGear
        {
            set
            {
                _currentGear = value;
                maxSpeedInKms = value * 20;
                if (value == 0)
                {
                    _isInNeutral = true;
                }
                else if (value == -1)
                {
                    _isInRear = true;
                }
            }
            get => _currentGear;
        }
        
        public float maxSpeedInKms;
        
        public float ShiftGear(int gear)
        {
            float maxSpeedKm = 20 * gear;
            return maxSpeedKm;
        }
    }
}