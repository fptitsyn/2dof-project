using System;
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

        public void OpenSettings()
        {
            settingsUI.SetActive(true);
        }

        public void ExitToMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}