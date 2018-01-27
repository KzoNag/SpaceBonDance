using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gohst : MonoBehaviour
{
    public Animator animator;

    public float speed;
    private Vector3 targetPosition;
    private bool isMoving;

	// Use this for initialization
	void Start ()
    {
        targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
	}

    public void Clap()
    {
        animator.SetTrigger("clap");
    }

    public void Move(float distance)
    {
        isMoving = true;
        targetPosition += transform.forward * distance;
    }
}
