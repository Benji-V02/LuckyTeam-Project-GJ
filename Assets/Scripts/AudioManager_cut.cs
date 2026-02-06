using UnityEngine;

public class AudioManager_cut : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;   // background
    [SerializeField] AudioSource soundSource;   // speech / foreground

    public AudioClip gether;
    public AudioClip menu;
    public AudioClip fight;
    public AudioClip speach;

    private void Start()
    {
        // Background menu music (quiet)
        musicSource.clip = menu;
        musicSource.volume = 0.2f;   // jemne do pozadia
        musicSource.loop = true;
        musicSource.Play();

        // Speech (foreground)
        soundSource.clip = speach;
        soundSource.volume = 1.0f;
        soundSource.loop = false;
        soundSource.Play();
    }
}
