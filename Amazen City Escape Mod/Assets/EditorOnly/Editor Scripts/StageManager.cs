using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	public enum Stage
	{
		wvo,
		dtd,
		wap,
		csc,
		flc,
		rct,
		tpj,
		kdv,
		aqa,
		other
	}

	public enum Section
	{
		A,
		B,
		C,
		D,
		E,
		F
	}

	public enum PlayerName
	{
		Sonic_New,
		Sonic_Fast,
		Princess,
		Snow_Board,
		Shadow,
		Silver,
		Tails,
		Amy,
		Knuckles,
		Blaze,
		Rouge,
		Omega,
		Metal_Sonic
	}

	public enum State
	{
		Playing,
		Paused,
		Event,
		Result
	}

	[Header("Main Stage")]
	public Stage _Stage;

	public Section StageSection;

	public PlayerName Player;

	public bool FirstSection;

	internal State StageState;

	[Header("BGM")]
	public AudioClip StageBGM;

	public int CustomSample;

	public int StartSample;

	public int EndSample;

	internal PlayerStart[] PlayerStarts;

	internal AudioSource BGMPlayer;

	private bool IsLooping;

	
	}
