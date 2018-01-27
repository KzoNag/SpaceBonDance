using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardClap : MonoBehaviour 
{
	public int ClapFrame = 30;
	public int ClapLine = 10;
	private int _currentFrame;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		_currentFrame++;

		if (_currentFrame < ClapLine) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				ClapHit ();
			}
		}

		// リセット
		if (ClapFrame < _currentFrame) {
			_currentFrame = 0;
		}
	}


	public void ClapHit ()
	{
		GameObject.Find ("RhythmRingObject").GetComponent<RhythmRingObject> ().Clap ();
	}
}
