using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private List<Sound> music, sounds;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void PlayMusic(string soundName)
        {
            Sound s = music.Find(x => x.name == soundName);

            if (s == null)
            {
                Debug.LogWarning("Music: " + soundName + " not found!");
                return;
            }
            
            musicSource.clip = s.clip;
            musicSource.Play();
        }

        public void PlaySfx(string soundName)
        {
            Sound s = sounds.Find(x => x.name == soundName);

            if (s == null)
            {
                Debug.LogWarning("Sound " + soundName + " not found!");
                return;
            }
            
            sfxSource.PlayOneShot(s.clip);
        }
    }
}