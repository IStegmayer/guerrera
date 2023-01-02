using UnityEngine;
using System;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio Source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem>
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundsSource;

    public Sound[] sounds;

    void Start()
    {
        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        _musicSource.clip = s.Clip;
        _musicSource.loop = s.Loop;
        _musicSource.volume = s.Volume;

        _musicSource.Play();
    }

    public void PlaySound(string name, Vector3 pos, float vol = 1)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        _soundsSource.transform.position = pos;
        _soundsSource.clip = s.Source.clip;
        _soundsSource.loop = s.Source.loop;
        _soundsSource.volume = s.Source.volume;

        if (_soundsSource.loop)
            _soundsSource.Play();
        else
            _soundsSource.PlayOneShot(_soundsSource.clip, vol);
    }
}