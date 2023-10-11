using UnityEngine;
using System;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio Source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : PersistentSingleton<AudioSystem> {
    [SerializeField][Range(0f, 1f)] public float masterVolume;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundsSource;
    private AudioSource currentMusicSoundSource;
    private AudioSource currentSFXSoundSource;
    public Sound[] sounds;

    void Start() {
        foreach (Sound s in sounds) {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }
        
        PlayMusic("mainTheme");
    }

    public void PlayMusic(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.Name == soundName);
        if (s == null) {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        currentMusicSoundSource = s.Source;
        musicSource.clip = s.Source.clip;
        musicSource.loop = s.Source.loop;
        musicSource.volume = s.Source.volume * masterVolume;

        musicSource.Play();
    }

    public void PlaySound(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.Name == soundName);
        if (s == null) {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        currentSFXSoundSource = s.Source;
        soundsSource.clip = s.Source.clip;
        soundsSource.loop = s.Source.loop;
        soundsSource.volume = s.Source.volume * masterVolume;

        if (soundsSource.loop)
            soundsSource.Play();
        else
            soundsSource.PlayOneShot(soundsSource.clip, soundsSource.volume);
    }

    public void SetMasterVolume(float volume) {
        masterVolume = volume/100f;
        soundsSource.volume = currentSFXSoundSource.volume * masterVolume;
        musicSource.volume = currentMusicSoundSource.volume * masterVolume;
    }
}