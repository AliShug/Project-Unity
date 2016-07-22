using UnityEngine;
using System.Collections;
using Leap.Unity;

public class PhysicsInputManager : MonoBehaviour {

	public Transform displayTargetWidget;
	public Transform displayBoneWidget;
	public RigidHand[] hands;

	public float hoverDistance = 0.1f;

	private PhysicsInput _lastHovered;
	
	// Update is called once per frame
	void Update () {
		// Grab the current child input components
		PhysicsInput[] childInputs = transform.GetComponentsInChildren<PhysicsInput>();

		float closestDist = hoverDistance;
		Vector3 closestPoint = new Vector3(0,0,0);
		Transform closestBone = null;
		PhysicsInput targetInput = null;

		// We cycle through every finger-end collider in both hands and for all inputs to find the likely interaction target
		foreach (PhysicsInput input in childInputs) {
			foreach (RigidHand hand in hands) {
				if (hand.isActiveAndEnabled) {
					foreach (RigidFinger finger in hand.fingers) {
						Transform bone = finger.bones[3];
						Vector3 point = input.collider.ClosestPointOnBounds(bone.position);
						float dist = Vector3.Distance(point, bone.position);

						if (dist < closestDist) {
							closestBone = bone;
							closestPoint = point;
							closestDist = dist;
							targetInput = input;
						}
					}
				}
			}
		}


		if (targetInput) {
			// Hover selection logic <THERE CAN BE ONLY ONE>
			if (!_lastHovered) {
				targetInput.OnHoverEnter();
				_lastHovered = targetInput;
			}
			else if (_lastHovered != targetInput) {
				_lastHovered.OnHoverExit();
				targetInput.OnHoverEnter();
				_lastHovered = targetInput;
			}

			// Debug display
			if (displayTargetWidget) {
				displayTargetWidget.gameObject.SetActive(true);
				displayTargetWidget.position = closestPoint;
			}
			if (displayBoneWidget) {
				displayBoneWidget.gameObject.SetActive(true);
				displayBoneWidget.position = closestBone.position;
			}
		}
		else {
			// Dehover
			if (_lastHovered) {
				_lastHovered.OnHoverExit();
				_lastHovered = null;
			}

			// Hide debug stuff
			if (displayTargetWidget) {
				displayTargetWidget.gameObject.SetActive(false);
			}
			if (displayBoneWidget) {
				displayBoneWidget.gameObject.SetActive(false);
			}
		}

		// Debug display

	}
}
