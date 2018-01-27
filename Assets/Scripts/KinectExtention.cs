using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

public static class KinectExtention
{
    public static Vector3 ToVector3(this CameraSpacePoint cameraSpacePoint)
    {
        return new Vector3(cameraSpacePoint.X, cameraSpacePoint.Y, cameraSpacePoint.Z);
    }
}
