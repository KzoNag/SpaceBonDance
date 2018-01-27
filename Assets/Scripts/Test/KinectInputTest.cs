using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectInputTest : MonoBehaviour
{
    public enum InputMode
    {
        Pose,
        Clap
    }
    private InputMode mode;

    public KinectInput kinectInput;

    public RectTransform targetPrefab;
    private RectTransform[] currentTargets = new RectTransform[2];

    public GameObject clapText;

	// Use this for initialization
	void Start ()
    {
        StartNext();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(mode == InputMode.Pose)
        {
            if (IsHitAll())
            {
                StartNext();
            }
        }
        else
        {
            if(kinectInput.IsClap())
            {
                StartNext();
            }
        }
    }

    private void StartNext()
    {
        DestroyTarget();
        clapText.SetActive(false);

        if (Random.value < 0.5f)
        {
            CreateTarget();
            mode = InputMode.Pose;
        }
        else
        {
            clapText.SetActive(true);
            mode = InputMode.Clap;
        }
    }

    private void DestroyTarget()
    {
        if(currentTargets != null)
        {
            foreach(var target in currentTargets)
            {
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
        }
    }

    private void CreateTarget()
    {
        for(int i=0; i<currentTargets.Length; ++i)
        {
            Vector2 position;

            position.x = Random.Range(0.1f, 0.9f);
            position.y = Random.Range(0.1f, 0.9f);

            currentTargets[i] = CreateTarget(position);
        }
    }

    private RectTransform CreateTarget(Vector2 position)
    {
        var target = Instantiate(targetPrefab, kinectInput.ShiletteRootTransform);

        target.gameObject.SetActive(true);
        target.anchoredPosition = ToScreenPosition(position);

        return target;
    }

    private bool IsHitAll()
    {
        for(int i=0; i<currentTargets.Length; ++i)
        {
            if (!IsHit(currentTargets[i])) { return false; }
        }

        return true;
    }

    private bool IsHit(RectTransform target)
    {
        var normalizedPosition = ToNormalizePosition(target.anchoredPosition);

        Debug.LogFormat("###Target {0:0.00}, {1:0.00}", normalizedPosition.x, normalizedPosition.y);

        return kinectInput.IsHit(normalizedPosition.x, normalizedPosition.y, 50f / 512f);
    }

    private Vector2 ToScreenPosition(Vector2 position)
    {
        return new Vector2(
            position.x * kinectInput.ShiletteRootTransform.sizeDelta.x,
            -position.y * kinectInput.ShiletteRootTransform.sizeDelta.y
            );
    }

    private Vector2 ToNormalizePosition(Vector2 position)
    {
        return new Vector2(
            position.x / kinectInput.ShiletteRootTransform.sizeDelta.x,
            -position.y / kinectInput.ShiletteRootTransform.sizeDelta.y
            );
    }
}
