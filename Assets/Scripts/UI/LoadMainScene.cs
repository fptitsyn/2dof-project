using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadMainScene : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera mainCamera;
        [SerializeField] private CinemachineCamera carCamera;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                StartCoroutine(LoadScene(1));
            }
        }

        private IEnumerator LoadScene(int index)
        {
            mainCamera.Priority = 0;
            carCamera.Priority = 1;
            
            yield return new WaitForSeconds(1.5f);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}