using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ReactionMenu : PhysicsMenu {

    public int displayIterations = 10;
    public Vector2 placementLimits = new Vector2(0.2f, 0.15f);

    public UnityEvent onNext;

    private int _displayCount = 0;

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

    protected override void OnShow() {
        base.OnShow();
        // Reposition the button(s)
        // Avoid placing any button too close to the other(s)
        var buttons = GetComponentsInChildren<PhysicsInput>(true);
        var usedPositions = new List<Vector3>();
        foreach (var btn in buttons) {
            Debug.Log(btn);
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
            btn.transform.localPosition = newPos;
            // Force the physics joint to reset
            btn.gameObject.SetActive(false);
            btn.gameObject.SetActive(true);
        }

        _displayCount++;
    }

    protected override void OnHide() {
        base.OnHide();
        if (_displayCount >= displayIterations) {
            onNext.Invoke();
        }
    }

}
