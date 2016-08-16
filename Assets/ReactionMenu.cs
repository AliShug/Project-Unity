using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ReactionMenu : PhysicsMenu {

    public static List<PhysicsInput.Mode> availableModes = new List<PhysicsInput.Mode> {
        PhysicsInput.Mode.Simple,
        PhysicsInput.Mode.Complex,
        PhysicsInput.Mode.StaticTouch
    };

    public int displayIterations = 10;
    public Vector2 placementLimits = new Vector2(0.2f, 0.15f);

    public UnityEvent onFirstShow;
    public UnityEvent onNext;

    private int _displayCount = 0;
    private bool _hasShown = false;

    private PhysicsInput.Mode _interactionMode;
    public PhysicsInput.Mode InteractionMode {
        get {
            return _interactionMode;
        }
    }

    public override PhysicsMenu NextMenu {
        get {
            // Loop back on self
            if (_displayCount < displayIterations) {
                return this;
            }
            else {
                return base.NextMenu;
            }
        }
    }

    public void Awake() {
        // Pick our interaction mode and remove it from the available list
        _interactionMode = availableModes[Random.Range(0, availableModes.Count)];
        RecordingManager.Instance.Log(string.Format("{0} picked interaction mode {1}", this, _interactionMode));
        availableModes.Remove(_interactionMode);
    }

    protected override void OnShow() {
        base.OnShow();

        if (!_hasShown) {
            // Setup buttons for interaction
            var buttons = GetComponentsInChildren<PhysicsInput>(true);
            foreach (var btn in buttons) {
                btn.SetMode(_interactionMode);
            }
            onFirstShow.Invoke();
            _hasShown = true;
        }

        RandomizeButtons();
        _displayCount++;
    }

    protected override void OnHide() {
        base.OnHide();
        if (_displayCount >= displayIterations) {
            onNext.Invoke();
            _displayCount = 0;
        }
    }

    public void RandomizeButtons() {
        // Reposition the button(s)
        // Avoid placing any button too close to the other(s)
        var buttons = GetComponentsInChildren<PhysicsInput>(true);
        var usedPositions = new List<Vector3>();
        foreach (var btn in buttons) {
            Vector3 newPos = new Vector3();
            float dist = 0.0f;
            while (dist < 0.08f) {
                newPos.x = Random.Range(-0.2f, 0.2f);
                newPos.y = Random.Range(-0.2f, 0.2f);
                if (usedPositions.Count > 0) {
                    float closest = Mathf.Infinity;
                    // Distance to closest used position
                    foreach (var point in usedPositions) {
                        float newDist = Vector3.Distance(point, newPos);
                        if (newDist < closest) {
                            closest = newDist;
                        }
                    }
                    dist = closest;
                }
                else {
                    break;
                }
            }
            usedPositions.Add(newPos);
            btn.MoveLocal(newPos);
            // Force the physics joint to reset
            btn.gameObject.SetActive(false);
            btn.gameObject.SetActive(true);
        }
    }

}
