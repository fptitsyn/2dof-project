using Audio;
using UnityEngine;

namespace Vehicle
{
    public class CollisionAudio : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("crash");
            int r = Random.Range(1, 4);
            AudioManager.Instance.PlaySfx($"Crash{r}");
        }
    }
}