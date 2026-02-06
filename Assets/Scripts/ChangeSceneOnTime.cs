using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTime : MonoBehaviour
{
    public float changeTime;
    public string sceneName;

    private void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <= 0){
            SceneManager.LoadScene(2);
        }
    }
}
