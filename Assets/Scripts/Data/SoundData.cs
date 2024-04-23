using UnityEngine;
using System;

[Serializable]
public class SoundData {
	public string name;
	public bool status;
	public bool loop;
	public AudioClip clip;
	public AudioSource source;
}