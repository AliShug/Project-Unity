﻿using UnityEngine;
using System.Collections;
using Leap.Unity;

using MeshExtensions; // custom extension methods

public class PhysicsInputManager : MonoBehaviour {

    public bool holdTarget = false;

	public Transform displayTargetWidget;
	public Transform displayBoneWidget;
	public RigidHand[] hands;

	public float hoverDistance = 0.1f;
    public Vector2 effectorDim;

    public SocketTest comms;

	private InteractiveObject _lastHovered;

    public int sensorThreshold = 500;

    private bool _sensor = false;
    public bool Sensor {
        get {
            return _sensor;
        }
    }
	
    void UpdateSensor() {
        if (comms != null) {
            int reading = comms.CapacitiveSensor;
            if (!_sensor && reading > sensorThreshold) {
                print("Press!");
                _sensor = true;
            }
            else if (_sensor && reading < 0.8 * sensorThreshold) {
                print("Release");
                _sensor = false;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		// Grab the current child input components
		InteractiveObject[] childInputs = transform.GetComponentsInChildren<InteractiveObject>();

		float closestDist = hoverDistance;
		Vector3 closestPoint = new Vector3();
		Vector3 closestNormal = new Vector3();
        Vector3 interactionPoint = new Vector3();
		Transform closestBone = null;
		InteractiveObject targetInput = null;

		// We cycle through every finger-end collider in both hands and for all inputs to find the likely interaction target
		foreach (InteractiveObject input in childInputs) {
			foreach (RigidHand hand in hands) {
				if (hand.isActiveAndEnabled) {
					foreach (RigidFinger finger in hand.fingers) {
						Transform bone = finger.bones[3];
						Vector3 point = input.GetNearestPoint(bone.position);
						float dist = Vector3.Distance(point, bone.position);

                        // If bone->point lines up with the normal, we're behind the interaction surface
                        Vector3 delta = point - bone.position;
                        float dot = Vector3.Dot(delta, input.InteractionNormal);
                        if (dot > 0 && Mathf.Abs(dot) < dist+0.01f) {
                            dist = 0.0f;
                        }

						if (dist < closestDist) {
							closestBone = bone;
							closestPoint = point;
							closestNormal = input.InteractionNormal;
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
				targetInput.HoverEnter();
				_lastHovered = targetInput;
			}
			else if (_lastHovered != targetInput) {
				_lastHovered.HoverExit();
				targetInput.HoverEnter();
				_lastHovered = targetInput;
			}

            // Get the actual interaction point
            interactionPoint = targetInput.GetInteractionPoint(closestPoint, effectorDim);
			if (displayTargetWidget && !holdTarget) {
				displayTargetWidget.gameObject.SetActive(true);
				displayTargetWidget.position = interactionPoint;
				Vector3 normalTarget = interactionPoint + closestNormal;
				displayTargetWidget.LookAt(normalTarget);
			}
			if (displayBoneWidget) {
				displayBoneWidget.gameObject.SetActive(true);
				displayBoneWidget.position = closestBone.position;
			}
		}
		else {
			// Dehover
			if (_lastHovered) {
				_lastHovered.HoverExit();
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

        // Capacitive sensor update
        UpdateSensor();
	}
}
