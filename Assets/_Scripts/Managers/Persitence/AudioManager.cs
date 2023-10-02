using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtilities;

public class AudioManager : PersistentSingletonMonoBehaviour<AudioManager> 
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    
    
    public void PlaySFX(AudioClip clip)
    {
        _sfxAudioSource.PlayOneShot(clip);
    }
    public void PlayBGM(AudioClip clip)
    {
        _bgmAudioSource.Stop();
        _bgmAudioSource.clip = clip;
        _bgmAudioSource.Play();
    }
    public void ToggleBGM()
    {
        _bgmAudioSource.mute = !_bgmAudioSource.mute;
    }
    public void ToggleSFX()
    {
        _sfxAudioSource.mute = !_sfxAudioSource.mute;
    }
    public void ChangeVolume(float volume)
    {
        _bgmAudioSource.volume = volume;
        _sfxAudioSource.volume = volume;
    }

}
