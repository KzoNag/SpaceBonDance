using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Windows.Kinect;

public class KinectInput : MonoBehaviour
{
    [Range(0, 1)]
    public float alpha = 1f;

    [SerializeField]
    private RectTransform shiletteRootTransform;
    public RectTransform ShiletteRootTransform { get { return shiletteRootTransform; } }

    [SerializeField]
    private RawImage shiletteImage;
    public Canvas TargetCanvas;

    public Texture2D BodyIndexTexture { get; private set; }
    private Color32[] bodyIndexColorData;

    private static readonly Color32 NoneColor = Color.clear;
    private static readonly Color32 BodyColor = Color.cyan;

    private Body[] currentBodies;
    private byte[] currentBodyIndexData;

    private Body currentBody;

    private bool isPrevClap;
    private bool isClap;
    public bool IsClap { get { return isClap && !isPrevClap; } }

    // Use this for initialization
    void Start ()
    {
        var description = KinectManager.Instance.BodyIndexFrameDescription;

        BodyIndexTexture = new Texture2D(description.Width, description.Height);
        bodyIndexColorData = new Color32[description.Width * description.Height];

        shiletteImage.texture = BodyIndexTexture;

        KinectManager.Instance.BodyIndexArrived += Instance_BodyIndexArrived;
        KinectManager.Instance.BodyArrived += Instance_BodyArrived;
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateTexture(currentBodyIndexData);
        UpdateCurrentBody();
	}

    private void Instance_BodyIndexArrived(byte[] data)
    {
        currentBodyIndexData = data;
    }

    private void UpdateTexture(byte[] data)
    {
        if (data == null) { return; }

        for (int y = 0; y < BodyIndexTexture.height; ++y)
        {
            for (int x = 0; x < BodyIndexTexture.width; ++x)
            {
                int dataIndex = BodyIndexTexture.width * y + x;

                int bodyIndex = data[dataIndex];

                Color32 color = (0 <= bodyIndex && bodyIndex <= 5) ? BodyColor : NoneColor;

                color.a = (byte)(color.a * alpha);

                int textureIndex = BodyIndexTexture.width * (BodyIndexTexture.height - y - 1) + x;

                bodyIndexColorData[textureIndex] = color;
            }
        }

        BodyIndexTexture.SetPixels32(bodyIndexColorData);
        BodyIndexTexture.Apply();

        if(!shiletteImage.enabled)
        {
            shiletteImage.enabled = true;
        }
    }

    private void Instance_BodyArrived(Body[] bodies)
    {
        currentBodies = bodies;
    }

    private void UpdateCurrentBody()
    {
        if (currentBodies == null) { return; }

        // すでにトラッキング中ならそれを探す
        if(currentBody != null)
        {
            currentBody = currentBodies.FirstOrDefault<Body>(_body => _body.IsTracked && _body.TrackingId == currentBody.TrackingId);
        }

        // トラッキングしてない場合は１人決める
        if(currentBody == null)
        {
            currentBody = currentBodies.FirstOrDefault<Body>(_body => _body.IsTracked);
        }

        isPrevClap = isClap;
        isClap = CheckClap();
    }

    public bool IsHit(float x, float y, float radius)
    {
        if(currentBody == null) { return false; }

        var joints = new Windows.Kinect.Joint[] 
        {
            currentBody.Joints[JointType.HandLeft],
            currentBody.Joints[JointType.HandRight],
        };

        foreach(var joint in joints)
        {
            if(IsHit(joint, x, y, radius))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsHit(Windows.Kinect.Joint joint, float x, float y, float radius)
    {
        var cameraPosition = joint.Position;
        var screenPosition = KinectManager.Instance.CameraToDepthPosition(cameraPosition);

        var normalizedScreenPosition = Vector2.zero;
        normalizedScreenPosition.x = (float)screenPosition.x / KinectManager.Instance.DepthFrameDescription.Width;
        normalizedScreenPosition.y = (float)screenPosition.y / KinectManager.Instance.DepthFrameDescription.Height;

        var distance = Vector2.Distance(new Vector2(x, y), normalizedScreenPosition);

        return distance < radius;
    }

    private bool CheckClap()
    {
        // テスト用
        if (Input.GetKeyDown(KeyCode.Return)) { return true; }

        if (currentBody == null) { return false; }

        var leftPosition = currentBody.Joints[JointType.HandLeft].Position.ToVector3();
        var rightPosition = currentBody.Joints[JointType.HandRight].Position.ToVector3();

        return Vector3.Distance(leftPosition, rightPosition) < 0.15f;
    }
}
