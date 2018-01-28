using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeCircle : MonoBehaviour
{
    public Image image;
    public RectTransform Target { get { return (RectTransform)transform.parent; } }
    public NodeDetail Node { get; private set; }

    private float rate;
    public float Rate
    {
        set
        {
            rate = value;
            transform.localScale = Mathf.Lerp(2, 1, rate) * Vector3.one;
            image.color = new Color(image.color.r, image.color.g, image.color.b, rate);
        }
    }

    public void Setup(Transform parent, NodeDetail node)
    {
        Node = node;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        ((RectTransform)transform).offsetMax = Vector2.zero;
        ((RectTransform)transform).offsetMin = Vector2.zero;

        Rate = 0.0f;
    }
}
