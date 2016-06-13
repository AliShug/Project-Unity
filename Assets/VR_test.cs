using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class VR_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		OVRTracker tracker = OVRManager.tracker;

		transform.position = tracker.GetPose().position;
		transform.rotation = tracker.GetPose().orientation;
	}
}
