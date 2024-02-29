using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;
	private bool muteAll = false;

	public Sound[] sounds;

	void Awake(){		
		instance = this;

		foreach (Sound s in sounds){
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.volume = s.volume;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	private void LowVolumeToAnObject(string name){
		Array.Find(sounds, item => item.name == name).source.volume -= 0.01f;
	}

	private void UpVolumeToAnObject(string name){
		Array.Find(sounds, item => item.name == name).source.volume += 0.01f;
	}

	public void LowMusicVolume(){ 
		// LowVolumeToAnObject("SpaceLevel1");
		// LowVolumeToAnObject("Boss1");
	}

	public void UpMusicVolume(){
		// UpVolumeToAnObject("SpaceLevel1");
		// UpVolumeToAnObject("Boss1");
	}

	public void LowSFXVolume(){ 
		// LowVolumeToAnObject("PlayerDeath");
		// LowVolumeToAnObject("PlayerHit");
	}

	public void UpSFXVolume(){ 
		// UpVolumeToAnObject("PlayerDeath");
		// UpVolumeToAnObject("PlayerHit");
	}

	public void LowVoiceVolume(){ 
		// LowVolumeToAnObject("CorazonAdrian");
		// LowVolumeToAnObject("CorazonAlex");
		// LowVolumeToAnObject("CuadradoAdrian");
		// LowVolumeToAnObject("CuadradoAlex");
	}

	public void UpVoiceVolume(){ 
		// UpVolumeToAnObject("CorazonAdrian");
		// UpVolumeToAnObject("CorazonAlex");
	}

	public void LowVolume(){ foreach (Sound s in sounds){ s.source.volume = s.source.volume - 0.01f; }}

	public void UpVolume(){ foreach (Sound s in sounds){ s.source.volume = s.source.volume + 0.01f; }}

	public void Play(string sound){
		if(muteAll) return;

		Sound s = Array.Find(sounds, item => item.name == sound);

		if (s == null){
			//Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.Play();	
	}

	public void Stop(string sound){
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			//Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.Stop();
	}

    public IEnumerator FadeOut (string sound, float FadeTime) {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			//Debug.LogWarning("Sound: " + sound + " not found!");
            yield return null;
		}

        float startVolume = s.source.volume;
 
        while (s.source.volume > 0) {
            s.source.volume -= startVolume * Time.deltaTime / FadeTime; 
            yield return null;
        }
 
        s.source.Stop();
        s.source.volume = startVolume;
    }

    public IEnumerator FadeIn (string sound, float FadeTime) {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			//Debug.LogWarning("Sound: " + sound + " not found!");
            yield return null;
		}

        float startVolume = s.source.volume;
 
        while (s.source.volume < 1) {
            s.source.volume += 0.5f * Time.deltaTime / FadeTime; 
            yield return null;
        }
    }
}
