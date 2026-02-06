using UnityEngine;

public class AudioManager_menu : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;

    public AudioClip gether;
    public AudioClip menu;
    public AudioClip fight;
    

    private void Start(){
        musicSource.clip = menu;
        if (PlayerPrefs.GetInt("MusicEnabled", 1) == 1)
            musicSource.Play();

    }
}
