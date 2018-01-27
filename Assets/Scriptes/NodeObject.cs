using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour 
{
	public GameObject ScoreText;

	public int DeleteFrame = 60;
	private int _currentFrame;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		_currentFrame++;

		if (DeleteFrame < _currentFrame) {
			this.ScoreUp ();
			GameObject.Destroy (this.gameObject);
		}
	}


	public void ScoreUp ()
	{
		GameObject clone = GameObject.Instantiate (ScoreText, this.transform.position, Quaternion.identity);
		clone.transform.position = this.transform.position;
		clone.GetComponent<TextMesh> ().text = 100.ToString ();
	}
}
