using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class TrackerPos : MonoBehaviour {

	public void Reposition() {
		OVRTracker tracker = OVRManager.tracker;
		
		if (tracker != null) {
			transform.localPosition = tracker.GetPose().position;
			transform.localRotation = tracker.GetPose().orientation;
		}
	}
		

	// Update is called once per frame
	void Update () {
		Reposition();
	}
}
