using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericWindow : MonoBehaviour {
	public static WindowManager manager;
	public static GameControl gameControl;

	private EventSystem eventSystem {
		get { return GameObject.Find ("EventSystem").GetComponent<EventSystem> (); }
	}

	protected virtual void Display(bool value){
		gameObject.SetActive(value);
	}

	public virtual void Open(){
		Display (true);
	}

	public virtual void Close(){
		Display (false);
	}
}