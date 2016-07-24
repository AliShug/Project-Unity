using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhysicsInput : MonoBehaviour {

	public UnityEvent pressEvent;
	public UnityEvent releaseEvent;
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
	private float _furthestDistance;
	private float _homeDistance;

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
		_homeDistance = 0.0f;
		OnStart();
	}
	
	// Update is called once per frame
	void Update () {
		// Clicking logic - pass click distance from last 'home' point to enter,
		// return < 90% of 'away' point to exit
		float distFromStart = Vector3.Distance(_originalPosition, transform.position);
		if (_clicked) {
			if (distFromStart < 0.9f*_furthestDistance) {
				// Unclick
				_clicked = false;
				_homeDistance = distFromStart;
				OnClickExit();
				releaseEvent.Invoke();
			}
			else if (distFromStart > _furthestDistance) {
				_furthestDistance = distFromStart;
			}
		}
		else {
			if (distFromStart > _homeDistance + clickDistance) {
				_clicked = true;
				_furthestDistance = 0.0f;
				OnClickEnter();
				pressEvent.Invoke();
			}
			else if (distFromStart < _homeDistance) {
				_homeDistance = distFromStart;
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
