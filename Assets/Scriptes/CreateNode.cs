using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNode : MonoBehaviour 
{
	public GameObject NodeObject;

	public int CreateFrame = 60;
	private int _currentFrame;

	private int[] _nodeArray = {1, 1, 2, 1, 2, 1, 2, 4};
	private int _nodeNumber;

	// Use this for initialization
	void Start () 
	{
		_nodeNumber = 0;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		_currentFrame++;

		if (CreateFrame < _currentFrame) {
			int c = 0;

			if (_nodeArray.Length <= _nodeNumber) {
				_nodeNumber = 0;
			}

			c = _nodeArray [_nodeNumber];
			this.Create (c);

			_nodeNumber++;
			_currentFrame = 0;
		}
	}


	public void Create ()
	{
		Vector3 c_position = Vector3.zero;
		c_position.x = Random.Range (-5f, 5f);
		c_position.y = Random.Range (-2.5f, 2.5f);
		c_position.z = 0;

		GameObject.Instantiate (NodeObject, c_position, Quaternion.identity);
	}


	public void Create (int count) 
	{
		Vector3 c_position = Vector3.zero;

		for(int i = 0; i < count; i++) {
			
			c_position.x = Random.Range (-5f, 5f);
			c_position.y = Random.Range (-2.5f, 2.5f);
			c_position.z = 0;

			GameObject.Instantiate (NodeObject, c_position, Quaternion.identity);
		}
	}
}
