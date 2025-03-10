using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000062 RID: 98
public partial class ReadCustomAudio : MonoBehaviour
{
	private AudioMixer MainMixer;
	public AudioSource SoundEffect;
	public AudioSource VoiceEffect;
	public bool BypassReverbZones;

	// Token: 0x0600045D RID: 1117 RVA: 0x0002ABC8 File Offset: 0x00028DC8
	private void Awake()
	{
		this.MainMixer = (Resources.Load("Master Mixer") as AudioMixer);
		if (SoundEffect != null)
		{

			this.SoundEffect.outputAudioMixerGroup = this.MainMixer.FindMatchingGroups("Sounds")[0];
			if (BypassReverbZones)
			{
				this.SoundEffect.bypassReverbZones = true;
			}
		}
		if (VoiceEffect != null)
		{

			this.VoiceEffect.outputAudioMixerGroup = this.MainMixer.FindMatchingGroups("Voices")[0];
			if (BypassReverbZones)
			{
				this.VoiceEffect.bypassReverbZones = true;
			}
		}
	}
}
