using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmRingObject : MonoBehaviour 
{
	

	public int RhythmFrame = 30;
	private int _currentFrame;

	Animator _animator;
	SpriteRenderer _renderer;

	void Awake ()
	{
		_animator = this.GetComponent<Animator> ();
		_renderer = this.GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		_currentFrame++;
		if (RhythmFrame < _currentFrame) {
			_renderer.color = Color.white;
			_currentFrame = 0;
		}
	}


	public void Clap ()
	{
		_renderer.color = Color.red;
	}
}
