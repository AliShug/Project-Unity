using UnityEngine;
using System.Collections;


public class OVRViewCone : MonoBehaviour
{

    private GameObject viewConeMesh;
    public OVRManager camController;
    public OVRTracker tracker;
    public GameObject TrackedCam;

    public float updateInterval = 5f;
    float lastUpdate = 0;

    // Use this for initialization
    void Start()
    {
        //OVRDevice.ResetOrientation ();
        //updateCameraTracker ();

        viewConeMesh = transform.FindChild("viewConeMesh").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown (KeyCode.JoystickButton0) || Input.GetKeyDown (KeyCode.KeypadEnter))
        {
            OVRDevice.ResetOrientation ();
            updateCameraTracker ();
        }
        */

        if (lastUpdate + updateInterval <Time.time)
		{
            if (camController.usePositionTracking)
            {
                updateCameraTracker();
            }
            lastUpdate = Time.time;
        }

        Vector3 cameraPos = TrackedCam.transform.position;
        viewConeMesh.GetComponent<Renderer>().material.SetVector("_CameraPos", new Vector4(cameraPos.x, cameraPos.y, cameraPos.z, 0));
    }

    public void updateCameraTracker()
    {
        Vector3 IRCameraPos = Vector3.zero;
        Quaternion IRCameraRot = Quaternion.identity;

        float cameraHFov = 0;
        float cameraVFov = 0;
        float cameraNearZ = 0;
        float cameraFarZ = 0;

        GetIRCamera(ref IRCameraPos, ref IRCameraRot, ref cameraHFov, ref cameraVFov, ref cameraNearZ, ref cameraFarZ);

        //IRCameraPos.z *= -1;

        transform.localPosition = IRCameraPos;
        transform.localRotation = IRCameraRot;

        //Debug.Log ("HFov " + cameraHFov.ToString ());
        //Debug.Log ("VFov " + cameraVFov.ToString ());

        float horizontalScale = Mathf.Tan(cameraHFov / 2f);
        float verticalScale = Mathf.Tan(cameraVFov / 2f);

        //Debug.Log ("HDistance " + horizontalScale.ToString ());
        //Debug.Log ("VDistance " + verticalScale.ToString ());

        transform.localScale = new Vector3(horizontalScale * cameraFarZ, verticalScale * cameraFarZ, cameraFarZ);
    }

    bool GetIRCamera(ref Vector3 position,
                     ref Quaternion rotation,
                     ref float cameraHFov,
                     ref float cameraVFov,
                     ref float cameraNearZ,
                     ref float cameraFarZ)
    {
        //if (!OVRManager.isSupportedPlatform || Hmd==null) return false;
        /*
        ovrTrackingState ss = OVRDevice.HMD.GetTrackingState();
		
        rotation = new Quaternion(	ss.CameraPose.Orientation.x,
                                  ss.CameraPose.Orientation.y,
                                  ss.CameraPose.Orientation.z,
                                  ss.CameraPose.Orientation.w);
		
        position = new Vector3(	ss.CameraPose.Position.x,
                               ss.CameraPose.Position.y,
                               ss.CameraPose.Position.z);
        */
        OVRPose ss = OVRManager.tracker.GetPose();

        rotation = new Quaternion(ss.orientation.x,
                                  ss.orientation.y,
                                  ss.orientation.z,
                                  ss.orientation.w);

        position = new Vector3(ss.position.x,
                               ss.position.y,
                               ss.position.z);

        OVRTracker.Frustum ff = OVRManager.tracker.GetFrustum();


        cameraHFov = ff.fov.x * (float)Mathf.PI / 180.0f;
        cameraVFov = ff.fov.y * (float)Mathf.PI / 180.0f;
        cameraNearZ = ff.nearZ;
        cameraFarZ = ff.farZ;

        /*
		HmdDesc desc = Hmd.HmdDesc();
		
		cameraHFov = desc.CameraFrustumHFovInRadians;
		cameraVFov = desc.CameraFrustumVFovInRadians;
		cameraNearZ = desc.CameraFrustumNearZInMeters;
		cameraFarZ = desc.CameraFrustumFarZInMeters;
		*/
        //OVRDevice.OrientSensor (ref rotation);

        return true;
    }
}