using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour {
	[HideInInspector]
	public List<SoundData> sounds;
	private GameControl gameControl;

	void Awake(){
		gameControl = GetComponent<GameControl> ();

		if (sounds.Count > 0) {
			foreach (var sound in sounds) {
				sound.source = new AudioSource ();
				sound.source = gameObject.AddComponent<AudioSource> ();
				sound.source.clip = sound.clip;
				sound.source.loop = sound.loop;
			}
		}
	}

	// Update
	void Update() {
		if (sounds.Count > 0) {
			foreach (var sound in sounds) {
				if (sound.status && !sound.source.isPlaying) {
					sound.source.Play(); 
					sound.status = false;
				}

				if (sound.loop != sound.source.loop)
					sound.source.loop = sound.loop;

				if (sound.name == "Main" || sound.name == "New Game") {
					sound.source.volume = gameControl.music;
				} else {
					sound.source.volume = gameControl.sfx;
				}
			}
		}
	}

	public bool EnableSound(string name , bool loop = false){
		if (sounds.Count > 0)
			foreach (var sound in sounds)
				if (sound.name == name) {
					sound.status = true;

					if (sound.source != null)
						sound.source.loop = loop;

					return true;
				}

		return false;
	}

	public bool DisableSound(string name){
		if (sounds.Count > 0)
			foreach (var sound in sounds)
				if (sound.name == name) {
					sound.status = false;

					if(sound.source != null)
						sound.source.Stop ();
					return true;
				}

		return false;
	}

	public void DisableAllSounds(){
		if (sounds.Count > 0)
			foreach (var sound in sounds) {
				sound.status = false;

				if(sound.source != null)
					sound.source.Stop ();
			}
	}

	public bool GetStatus(string name){
		if (sounds.Count > 0)
			foreach (var sound in sounds)
				if (sound.name == name && sound.source != null)
					return sound.source.isPlaying;

		return false;
	}

	// Reset Audio Source
	public void ResetAudioSource(){
		if (sounds.Count > 0) {
			foreach (var sound in sounds) {
				Destroy (sound.source);
				sound.source = new AudioSource ();
				sound.source = gameObject.AddComponent<AudioSource> ();
				sound.source.clip = sound.clip;
				sound.source.loop = sound.loop;
			}
		}
	}
}