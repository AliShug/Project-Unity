using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhysicsInput : InteractiveObject {

	public UnityEvent pressEvent;
	public UnityEvent releaseEvent;
	public float clickDistance = 0.02f;

	private Collider _collider;
    public new Collider collider {
        get {
            if (!_collider)
                _collider = GetComponent<Collider>();
            return _collider;
        }
    }

    private Vector3 _originalPosition;
	private float _furthestDistance;
	private float _homeDistance;

	private bool _clicked = false;

	public bool Clicked {
		get { return _clicked; }
	}

	// Use this for initialization
	protected override void OnStart () {
        base.OnStart();
		_originalPosition = transform.position;
		_homeDistance = 0.0f;
	}
	
	// Update is called once per frame
	protected override void OnUpdate () {
        base.OnUpdate();

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
	}
}
