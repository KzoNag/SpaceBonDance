using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

public class KinectManager : MonoBehaviour
{
    private KinectSensor sensor;
    private MultiSourceFrameReader reader;
    private CoordinateMapper mapper;

    public int BodyCount { get; private set; }
    public Body[] BodyList { get; private set; }
    public byte[] BodyIndexData { get; private set; }
    public ushort[] DepthData { get; private set; }

    public FrameDescription BodyIndexFrameDescription { get; private set; }
    public FrameDescription DepthFrameDescription { get; private set; }

    public event System.Action<Body[]> BodyArrived;
    public event System.Action<byte[]> BodyIndexArrived;
    public event System.Action<ushort[]> DepthArrived;

    private static KinectManager instance;
    public static KinectManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<KinectManager>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        instance = this;

        sensor = KinectSensor.GetDefault();
        if (sensor == null) { return; }

        reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.BodyIndex | FrameSourceTypes.Depth);
        if (reader == null) { return; }

        mapper = sensor.CoordinateMapper;

        BodyIndexFrameDescription = sensor.BodyIndexFrameSource.FrameDescription;
        BodyIndexData = new byte[BodyIndexFrameDescription.BytesPerPixel * BodyIndexFrameDescription.LengthInPixels];

        DepthFrameDescription = sensor.DepthFrameSource.FrameDescription;
        DepthData = new ushort[DepthFrameDescription.Width * DepthFrameDescription.Height];

        for (var i = 0; i < BodyIndexData.Length; ++i)
        {
            BodyIndexData[i] = byte.MaxValue;
        }

        if (!sensor.IsOpen)
        {
            sensor.Open();
        }
    }

    private void OnApplicationQuit()
    {
        if (reader != null)
        {
            reader.Dispose();
            reader = null;
        }

        if (sensor != null)
        {
            if (sensor.IsOpen)
            {
                sensor.Close();
            }
            sensor = null;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        var frame = reader.AcquireLatestFrame();
        if (frame == null) { return; }

        if(frame.BodyFrameReference != null)
        {
            using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
            {
                UpdateBody(bodyFrame);
            }
        }

        if(frame.BodyIndexFrameReference != null)
        {
            using (var bodyIndexFrame = frame.BodyIndexFrameReference.AcquireFrame())
            {
                UpdateBodyIndex(bodyIndexFrame);
            }
        }

        if(frame.DepthFrameReference != null)
        {
            using (var depthFrame = frame.DepthFrameReference.AcquireFrame())
            {
                UpdateDepth(depthFrame);
            }
        }
    }

    void UpdateBody(BodyFrame frame)
    {
        if (frame == null) { return; }

        BodyCount = frame.BodyCount;
        BodyList = new Body[BodyCount];

        frame.GetAndRefreshBodyData(BodyList);

        if(BodyArrived != null)
        {
            BodyArrived(BodyList);
        }
    }

    void UpdateBodyIndex(BodyIndexFrame frame)
    {
        if (frame == null) { return; }

        frame.CopyFrameDataToArray(BodyIndexData);

        if(BodyIndexArrived != null)
        {
            BodyIndexArrived(BodyIndexData);
        }
    }

    void UpdateDepth(DepthFrame frame)
    {
        if (frame == null) { return; }

        frame.CopyFrameDataToArray(DepthData);

        if(DepthArrived != null)
        {
            DepthArrived(DepthData);
        }
    }

    public Vector3 CameraSpaceToWorldSpace(CameraSpacePoint cameraSpacePoint)
    {
        Vector3 position = transform.position;

        position += transform.right * -cameraSpacePoint.X // Kinectは右手座標系
            + transform.up * cameraSpacePoint.Y
            + transform.forward * cameraSpacePoint.Z; 

        return position;
    }

    public CameraSpacePoint DepthToCameraSpacePoint(DepthSpacePoint depthPoint)
    {
        int index = DepthFrameDescription.Width * (int)depthPoint.Y + (int)depthPoint.X;
        ushort depth = DepthData[index];

        // 範囲外チェック
        if (depth < 100){ depth = 100; }
        if (depth > 8000) { depth = 8000; }

        return mapper.MapDepthPointToCameraSpace(depthPoint, depth);
    }

    public Vector3 DepthToWorldPosition(DepthSpacePoint depthPoint)
    {
        CameraSpacePoint cameraPoint = DepthToCameraSpacePoint(depthPoint);
        return CameraSpaceToWorldSpace(cameraPoint);
    }

    public Vector2 CameraToDepthPosition(CameraSpacePoint cameraPoint)
    {
        DepthSpacePoint depthPoint = mapper.MapCameraPointToDepthSpace(cameraPoint);

        var depthPosition = new Vector2(depthPoint.X, depthPoint.Y);
        depthPosition.x = Mathf.Clamp(depthPosition.x, 0, DepthFrameDescription.Width);
        depthPosition.y = Mathf.Clamp(depthPosition.y, 0, DepthFrameDescription.Height);

        return depthPosition;
    }
}
