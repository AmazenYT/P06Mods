using UnityEngine;

public class PlayerStart : MonoBehaviour
{
	[Header("Framework")]
	public int Player_No;

	public string Player_Name;

	public bool Amigo;

	[Header("Optional")]
	public bool KeepPreviousAmigo;

	public bool NotVisiblyInteractable;

	public void SetParameters(int _Player_No, string _Player_Name, bool _Amigo)
	{
		Player_No = _Player_No;
		Player_Name = _Player_Name;
		Amigo = _Amigo;
	}
}
