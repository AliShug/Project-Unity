using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour {

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
}
