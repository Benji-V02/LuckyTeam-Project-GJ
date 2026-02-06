using UnityEngine;
using UnityEngine.UI;

public class SettingsVolume : MonoBehaviour
{
    private AudioSource musicSource;
    private Toggle musicToggle;
    private bool isMusicEnabled;

    private void Start()
    {
        musicToggle = GetComponent<Toggle>();
        musicSource = FindObjectOfType<AudioSource>();

        isMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Nastav Toggle pod¾a uloreného stavu
        musicToggle.isOn = isMusicEnabled;

        // Pripoj event
        musicToggle.onValueChanged.AddListener(OnMusicToggle);

        ApplyMusicState();
    }

    public void OnMusicToggle(bool isEnabled)
    {
        isMusicEnabled = isEnabled;
        PlayerPrefs.SetInt("MusicEnabled", isMusicEnabled ? 1 : 0);
        ApplyMusicState();
    }

    private void ApplyMusicState()
    {
        if (musicSource == null) return;

        if (isMusicEnabled)
            musicSource.UnPause();
        else
            musicSource.Pause();
    }
}