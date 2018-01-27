using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmScoreText : MonoBehaviour 
{
	Animator _animator;


	void Awake ()
	{
		_animator = this.GetComponent<Animator> ();
	}


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.9f) {
			GameObject.Destroy (this.gameObject);
		}
	}
}
