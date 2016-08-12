﻿using UnityEngine;
using System.Collections;
using Leap.Unity;

using MeshExtensions; // custom extension methods

public class PhysicsInputManager : MonoBehaviour {

    public enum Handedness {
        Both,
        Left, Right
    }

    public PhysicsMenu defaultMenu;

    private PhysicsMenu _currentMenu;
    public PhysicsMenu CurrentMenu {
        get {
            return _currentMenu;
        }
        set {
            _currentMenu = value;
        }
    }

    public bool holdTarget = false;

    public Transform safeZone;
	public Transform displayTargetWidget;
	public Transform displayBoneWidget;

    public LeapHandController handController;

    [SerializeField]
    private Handedness _handChoice = Handedness.Both;
    public Handedness HandChoice {
        get {
            return _handChoice;
        }
        set {
            if (value == Handedness.Left) {
                handController.enableLeft = true;
                handController.enableRight = false;
            }
            else if (value == Handedness.Right) {
                handController.enableLeft = false;
                handController.enableRight = true;
            }
            else if (value == Handedness.Both) {
                handController.enableLeft = handController.enableRight = true;
            }
        }
    }

    // Physical touch enable/disable (arm moves out of the way when disabled)
    [SerializeField]
    private bool _touchActive = false;

	public RigidHand[] hands;

	public float hoverDistance = 0.1f;
    public Vector2 effectorDim;

    public SocketTest comms;
    public int sensorThreshold = 500;


	private InteractiveObject _lastHovered;

    private bool _sensor = false;
    public bool Sensor {
        get {
            return _sensor;
        }
    }

    // For UnityEvent connections
    public void SetLeftHand() {
        HandChoice = Handedness.Left;
        RecordingManager.Instance.Log("SELECT: hand_choice=left");
    }
    public void SetRightHand() {
        HandChoice = Handedness.Right;
        RecordingManager.Instance.Log("SELECT: hand_choice=right");
    }

    void Start() {
        if (safeZone == null) {
            Debug.LogError("Arm operation requires the safe-zone to be specified");
        }

        // Hide all but the default menu
        // active any disabled menus (disabled for practical reasons in-editor)
        if (defaultMenu != null) {
            var menus = GetComponentsInChildren<PhysicsMenu>(true);
            foreach (var menu in menus) {
                menu.gameObject.SetActive(true);
                menu.Hide();
            }

            defaultMenu.Show();
            _currentMenu = defaultMenu;
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

        // Count up the active hands
        int activeHands = 0;
        RigidHand activeHand = null;
        foreach (var hand in hands) {
            if (hand.isActiveAndEnabled) {
                activeHands++;
                activeHand = hand;
            }
        }

		// We cycle through every finger-end collider in both hands and for all inputs to find the likely interaction target
		foreach (InteractiveObject input in childInputs) {
			foreach (RigidHand hand in hands) {
				if (hand.isActiveAndEnabled) {
					foreach (RigidFinger finger in hand.fingers) {
                        if (input.Touchable) {
                            Transform bone = finger.bones[3];
                            Vector3 point = input.GetNearestPoint(bone.position);
                            float dist = Vector3.Distance(point, bone.position);

                            // If bone->point lines up with the normal, we're behind the interaction surface and treat distance as 0
                            // This is limited to points directly behind the surface, and within a reasonable distance
                            Vector3 delta = point - bone.position;
                            float dot = Vector3.Dot(delta, input.InteractionNormal);
                            if (dot > 0 && Mathf.Abs(dot) > dist-0.01f && dist < 0.5f*hoverDistance) {
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
		}

		if (targetInput) {
			// Hover selection logic <THERE CAN BE ONLY ONE>
			if (!_lastHovered || !_lastHovered.gameObject.activeInHierarchy) {
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
        else if (activeHands == 1) {
            // Hand-follow behaviour
            // Only one hand to follow
            
            // Dehover
            if (_lastHovered) {
                _lastHovered.HoverExit();
                _lastHovered = null;
            }
            // move to offset from index finger
            if (displayTargetWidget && !holdTarget) {
                displayTargetWidget.gameObject.SetActive(true);
                Vector3 offset = new Vector3(0.0f, 0.0f, 0.18f);
                // offset from index finger's end
                displayTargetWidget.position = activeHand.fingers[1].bones[3].transform.position + offset;
            }

            if (displayBoneWidget) {
                displayBoneWidget.gameObject.SetActive(false);
            }
        }
        // No (or both) hands visible
		else {
			// Dehover
			if (_lastHovered) {
				_lastHovered.HoverExit();
				_lastHovered = null;
			}
            // Hide the arm (not used)
            RetractArm();
		}

        // Regardless of anything else, retract if touch is disabled
        if (!_touchActive) {
            RetractArm();
        }

        // Capacitive sensor update
        UpdateSensor();

        // Trigger next menu
        if (Input.GetButtonDown("Submit")) {
            _currentMenu.Next();
        }
	}

    void RetractArm() {
        // Track to safe zone
        if (displayTargetWidget && !holdTarget) {
            displayTargetWidget.gameObject.SetActive(true);
            displayTargetWidget.transform.position = safeZone.transform.position;
            displayTargetWidget.transform.rotation = safeZone.transform.rotation;
        }
        if (displayBoneWidget) {
            displayBoneWidget.gameObject.SetActive(false);
        }
    }

    public void DisableTouch() {
        _touchActive = false;
    }

    public void EnableTouch() {
        _touchActive = true;
    }
}
