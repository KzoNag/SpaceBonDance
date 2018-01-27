using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHand : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 subPosition = this.transform.position;
		subPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		subPosition.z = 0;
		this.transform.position = subPosition;
	}
}
