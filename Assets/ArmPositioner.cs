using UnityEngine;
using System.Collections;

public class ArmPositioner : MonoBehaviour {

	public Transform tracker;

	public Vector3 offset = new Vector3();

	// Update is called once per frame
	void Update () {
		transform.position = tracker.position + offset;
	}
}
