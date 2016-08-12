using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrackingReset : MonoBehaviour {

	public KeyCode resetKey = KeyCode.R;
	public TrackerPos tracker;
	public RectTransform calibrationUI;

	private Vector3 _desiredPos;
	private int _frames = 0;


	void Start() {
		_desiredPos = tracker.transform.position;
		//StartCoroutine("timedReset");

        if (RecordingManager.Instance.mode == RecordingManager.Mode.Playback) {
            // disable calibration
            calibrationUI.gameObject.SetActive(false);
            enabled = false;
        }
	}

	void Update () {
		// Initial calibration places the VR 'head' relative to the tracking camera
		if (OVRManager.tracker.isPositionTracked &&  _frames < 200) {
			calibrationUI.gameObject.SetActive(true);
			Retrack();
			_frames++;
		}
		else {
			calibrationUI.gameObject.SetActive(false);
			OVRManager.DismissHSWDisplay();
		}

	}

	public void Retrack() {
		UnityEngine.VR.InputTracking.Recenter();
		tracker.Reposition();

		// Rotate tracking rig so that tracker faces -Z direction
		float angle = tracker.transform.rotation.eulerAngles.y;
		transform.parent.Rotate(0, 180-angle, 0);

		// Move tracking rig so that tracker is in original desired position
		transform.parent.Translate(_desiredPos - tracker.transform.position);
	}

	IEnumerator timedReset() {
		while (true) {
			Retrack();
			yield return new WaitForSeconds(.5f);
		}
	}
}
