using System;
using Audio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseUI : MonoBehaviour
    {
        public static bool IsPaused;
        
        public static PauseUI Instance;

        private Canvas _canvas;

        [SerializeField] private GameObject mainUI;
        [SerializeField] private GameObject settingsUI;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
        }

        public static void TogglePause()
        {
            AudioManager.Instance.PlaySfx("Click");
            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1;
                Instance._canvas.enabled = false;
            }
            else
            {
                IsPaused = true;
                Time.timeScale = 0;
                Instance._canvas.enabled = true;
            }
        }

        public void ToggleSettings(bool value)
        {
            AudioManager.Instance.PlaySfx("Click");
            mainUI.SetActive(!value);
            settingsUI.SetActive(value);
        }

        public void ExitToMenu()
        {
            AudioManager.Instance.PlaySfx("Click");
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }
}