using UnityEngine;
using System.Collections;

public class AlignToTracker : MonoBehaviour
{
    public OVRPose trackerPose = OVRPose.identity;

    void Awake()
    {
        OVRCameraRig rig = GameObject.FindObjectOfType<OVRCameraRig>();

        if (rig != null)
            rig.UpdatedAnchors += OnUpdatedAnchors;
    }

    void OnUpdatedAnchors(OVRCameraRig rig)
    {
        if (!enabled)
            return;

        OVRPose pose = rig.trackerAnchor.ToOVRPose(true).Inverse();
        pose = trackerPose * pose;
        rig.trackingSpace.FromOVRPose(pose, true);
    }
}