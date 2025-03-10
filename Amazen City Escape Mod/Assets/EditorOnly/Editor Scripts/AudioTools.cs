using System;
using UnityEngine;

[ExecuteAlways]
public class AudioTools : MonoBehaviour
{

	public AudioSource SoundEffect;
	[Space]
	public AudioSource VoiceEffect;
	[Space]
	[Header("Temporary References do not change these")]
	public AudioSource Audio;
	public AudioClip Loop;


	void OnValidate()
    {
		if (SoundEffect != null)
		{
			Audio = SoundEffect;
		}
		if (VoiceEffect != null)
        {
			Loop = VoiceEffect.clip;
		}
		
    }
}
