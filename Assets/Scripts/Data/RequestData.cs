using UnityEngine.Events;
using System;

[Serializable]
public class RequestData {
	public string name;
	public bool status;
	public UnityEvent function;
	public float timer;
	public float timerInterval;
	public float lastExecute;
	public bool pause = false;
}