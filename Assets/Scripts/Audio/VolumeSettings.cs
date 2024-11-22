using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("SFXVolume"))
            {
                LoadVolume();
            }
            else
            {
                SetMusicVolume();
                SetSfxVolume();
            }
            
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
            sfxSlider.onValueChanged.AddListener(delegate { SetSfxVolume(); });
        }

        public void SetMusicVolume()
        {
            float volume = musicSlider.value;
            if (!audioMixer.SetFloat("Music", MathF.Log10(volume) * 20))
            {
                Debug.LogWarning("Could not set music volume.");
            }
            PlayerPrefs.SetFloat("musicVolume", volume);
        }

        public void SetSfxVolume()
        {
            float volume = sfxSlider.value;
            if (!audioMixer.SetFloat("SFX", MathF.Log10(volume) * 20))
            {
                Debug.LogWarning("Could not set sfx volume.");
            }
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        private void LoadVolume()
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            if (!audioMixer.SetFloat("Music", MathF.Log10(musicVolume) * 20))
            {
                Debug.LogWarning("Could not set music volume.");
            }
            musicSlider.value = musicVolume;

            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            if (!audioMixer.SetFloat("SFX", MathF.Log10(sfxVolume) * 20))
            {
                Debug.LogWarning("Could not set sfx volume.");
            }
            sfxSlider.value = sfxVolume;
        }
    }
}