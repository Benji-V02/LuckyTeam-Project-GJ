using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public void PlayGame(){
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AudioListener.pause = false;
        AudioListener.volume = 1f;

        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame(){
        Application.Quit();
        Debug.Log("QUIT");
    }
}

