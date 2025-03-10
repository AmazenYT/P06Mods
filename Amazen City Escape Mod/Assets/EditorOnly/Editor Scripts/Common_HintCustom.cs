using System;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class Common_HintCustom : MonoBehaviour
{
	// Token: 0x04001EEC RID: 7916
	[Header("First Message")]
	public string[] HintText;
	public AudioSource HintAudio;
    public bool AllowMessageText;
	public float HintMessageTime;
	[Header("Second Message")]
	public string[] HintText2;
	public AudioSource HintAudio2;
	public bool AllowMessageText2;
    public float HintMessageTime2;
	public float TimeTill2ndMessage;
	[Header("Third Message")]
	public string[] HintText3;
	public AudioSource HintAudio3;
	public bool AllowMessageText3;
    public float HintMessageTime3;
	public float TimeTill3rdMessage;
	[Header("Fourth Message")]
	public string[] HintText4;
	public AudioSource HintAudio4;
	public bool AllowMessageText4;
    public float HintMessageTime4;
	public float TimeTill4thMessage;
	[Header("Fifth Message")]
	public string[] HintText5;
	public AudioSource HintAudio5;
	public bool AllowMessageText5;
	public float HintMessageTime5;
	public float TimeTill5thMessage;
}
