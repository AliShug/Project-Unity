using UnityEngine;
using System.Collections;

public class FrustrumChecker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var frustrum = OVRManager.tracker.GetFrustum();
        Debug.LogFormat("near {0} far {1} fov {2}", frustrum.nearZ, frustrum.farZ, frustrum.fov);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
