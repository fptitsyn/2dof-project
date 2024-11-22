using System;
using System.Collections;
using Audio;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadMainScene : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera mainCamera;
        [SerializeField] private CinemachineCamera carCamera;

        [SerializeField] private GameObject mainUI;
        [SerializeField] private GameObject settingsUI;

        private void Start()
        {
            #if UNITY_STANDALONE_WIN
                Cursor.visible = false;
            #elif UNITY_EDITOR
                Cursor.visible = true;
            #endif
            AudioManager.Instance.PlayMusic("Main Theme");
        }

        public void StartGame()
        {
            mainCamera.Priority = 1;
            carCamera.Priority = 0;
            AudioManager.Instance.PlaySfx("Click");
            StartCoroutine(LoadScene(1));
        }

        public void ToggleSettings(bool value)
        {
            AudioManager.Instance.PlaySfx("Click");
            mainUI.SetActive(!value);
            settingsUI.SetActive(value);
        }

        public void ExitGame()
        {
            AudioManager.Instance.PlaySfx("Click");
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private IEnumerator LoadScene(int index)
        {
            mainCamera.Priority = 0;
            carCamera.Priority = 1;
            yield return new WaitForSeconds(3f);
            
            SceneManager.LoadScene(index);
        }

        // private void OnDisable()
        // {
        //     StopCoroutine(nameof(LoadScene));
        // }
    }
}