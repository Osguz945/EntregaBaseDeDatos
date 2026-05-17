using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    public string sceneToLoad;
    
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            Time.timeScale = 1;
        }

        else
        {
            Debug.LogError("Algo salio mal");
        }
    }

    public void ExitProgram()
    {
        Debug.Log("Game Over");
        Application.Quit();
    }
}