using UnityEngine;

public class AudioManager_fight : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;

    public AudioClip gether;
    public AudioClip menu;
    public AudioClip fight;
    

    private void Start(){
        musicSource.clip = fight;
        if (PlayerPrefs.GetInt("MusicEnabled", 1) == 1)
            musicSource.Play();

    }
}
