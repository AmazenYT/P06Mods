using System.Collections.Generic;
using UnityEngine;

public class ObjectEventTrigger : MonoBehaviour
{
	public enum Mode
	{
		StageEvent,
		ObjectGroupEvent
	}

	[Header("Manual Stage Setup")]
	public Mode EventMode;

	public List<GameObject> LockObjects;

	[Header("Stage Event (Call a stage event)")]
	public string EventName;

	[Header("Object Group Event (Activate these)")]
	public GameObject[] ObjectGroup;

	private bool Triggered;
}
