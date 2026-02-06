using UnityEngine;
using UnityEngine.SceneManagement;


public class BackToMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadSceneAsync(0);
    }
}

