using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonText : MonoBehaviour
{
    public RectTransform canvasTransform;
    public RectTransform rectTransform { get { return (RectTransform)transform; } }
    public RectTransform arrow;
    public Text text;
    public Transform target;

    public string Text { set { text.text = value; } }

    private float startTime;
    private bool showing;

    private void Start()
    {
        gameObject.SetActive(false);
    }

	void Update ()
    {
		if(showing && Time.time - startTime > 2)
        {
            showing = false;
            gameObject.SetActive(false);

            UpdateArrowPosition();
        }
	}

    public void Show(string text)
    {
        Text = text;
        showing = true;
        startTime = Time.time;
        UpdateArrowPosition();
        gameObject.SetActive(true);
    }

    private void UpdateArrowPosition()
    {
        var viewportPoint = Camera.main.WorldToViewportPoint(target.position);

        float canvaspoint = canvasTransform.sizeDelta.x * viewportPoint.x;

        var anchoredPosition = arrow.anchoredPosition;
        anchoredPosition.x = canvaspoint - rectTransform.anchoredPosition.x;

        arrow.anchoredPosition = anchoredPosition;
    }
}
