using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour {

    public Vector2 surfaceExtents = new Vector2(0.5f, 0.5f);
    public float surfaceOffset = 0.5f;

    private bool _hovering = false;
    public bool Hovered {
        get { return _hovering; }
    }

    // Use this for initialization
    void Start () {
        // Call derived class stuff
        OnStart();
	}
	
	// Update is called once per frame
	void Update () {
        // Call derived class stuff
        OnUpdate();
    }

    protected virtual void OnStart() { return; }
    protected virtual void OnUpdate() { return; }
    /**
	* Called when the object physically leaves its depressed state
	*/
    protected virtual void OnClickExit() { return; }
    /**
	* Called when the object physically enters its depressed (clicked) state
	*/
    protected virtual void OnClickEnter() { return; }
    protected virtual void OnHoverEnter() { return; }
    protected virtual void OnHoverExit() { return; }

    public void HoverEnter() {
        if (!_hovering) {
            _hovering = true;
            OnHoverEnter();
        }
    }

    public void HoverExit() {
        _hovering = false;
        OnHoverExit();
    }

    public Vector3 GetNearestPoint(Vector3 poi) {
        Vector3 localPoi = transform.InverseTransformPoint(poi);
        // flatten on z axis
        localPoi.z = surfaceOffset;
        // clamp on x/y
        localPoi.x = Mathf.Clamp(localPoi.x, -surfaceExtents.x, surfaceExtents.x);
        localPoi.y = Mathf.Clamp(localPoi.y, -surfaceExtents.y, surfaceExtents.y);
        return transform.TransformPoint(localPoi);
    }

    public Vector3 GetInteractionPoint(Vector3 poi, Vector2 effectorDim) {
        Vector3 localPoi = transform.InverseTransformPoint(poi);
        // flatten on z axis
        localPoi.z = surfaceOffset;
        // clamp on x/y, including effector's dimensions
        // effector's dimensions must be corrected for local space
        float xoff = (effectorDim.x / transform.localScale.x) / 2;
        float yoff = (effectorDim.y / transform.localScale.y) / 2;
        localPoi.x = Mathf.Clamp(localPoi.x, xoff-surfaceExtents.x, surfaceExtents.x-xoff);
        localPoi.y = Mathf.Clamp(localPoi.y, yoff-surfaceExtents.y, surfaceExtents.y-yoff);
        return transform.TransformPoint(localPoi);
    }

    public Vector3 InteractionCenter {
        get {
            Vector3 localCenter = new Vector3(0, 0, surfaceOffset);
            return transform.TransformPoint(localCenter);
        }
    }

    public Vector3 InteractionNormal {
        get {
            return transform.forward;
        }
    }
}
