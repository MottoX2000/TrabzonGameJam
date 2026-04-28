using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MySceneManager : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Son sahnedesin!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatýldý.");
    }
}