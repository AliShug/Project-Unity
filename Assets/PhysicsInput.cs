using UnityEngine;
using System.Collections;

public class PhysicsInput : MonoBehaviour {

	public float clickDistance = 0.02f;

	private Collider _collider;
	public Collider collider {
		get {
			if (!_collider)
				_collider = GetComponent<Collider>();
			return _collider;
		}
	}

	private Vector3 _originalPosition;

	private bool _hovering = false;
	private bool _clicked = false;

	public bool Hovered {
		get { return _hovering; }
	}
	public bool Clicked {
		get { return _clicked; }
	}

	// Use this for initialization
	void Start () {
		_originalPosition = transform.position;
		OnStart();
	}
	
	// Update is called once per frame
	void Update () {
		// Clicking logic - pass click distance to enter, return to 0.2*dist to exit
		float distFromStart = Vector3.Distance(_originalPosition, transform.position);
		if (_clicked) {
			if (distFromStart < 0.2f * clickDistance) {
				// Unclick
				_clicked = false;
				OnClickExit();
			}
		}
		else {
			if (distFromStart > clickDistance) {
				_clicked = true;
				OnClickEnter();
			}
		}

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
