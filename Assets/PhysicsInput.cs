using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInput : InteractiveObject
{

    public static int clickThreshold = 5500;

    public enum Mode
    {
        Simple,
        Complex,
        StaticTouch
    }

    public UnityEvent pressEvent;
    public UnityEvent releaseEvent;
    public float clickDistance = 0.02f;

    public Mode defaultInteractionMode = Mode.Complex;

    private Mode _mode;
    public Mode InteractionMode
    {
        get
        {
            return _mode;
        }
        set
        {
            SetMode(value);
        }
    }

    private Vector3 _originalPosition;
    private float _furthestDistance;
    private float _homeDistance;

    private bool _clicked = false;

    public bool Clicked
    {
        get { return _clicked; }
    }

    public override bool Touchable
    {
        get
        {
            if (_mode == Mode.StaticTouch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Collider logic
    private List<GameObject> _currentColliders = new List<GameObject>();

    // Deals with enabling/disabling the input's physics
    public void SetMode(Mode newMode)
    {
        //var rigidbody = GetComponent<Rigidbody>();
        var colliders = GetComponentsInChildren<Collider>();
        switch (newMode)
        {
            case Mode.Simple:
            case Mode.StaticTouch:
                // Disable physics
                //rigidbody.detectCollisions = false;
                //rigidbody.isKinematic = true;
                foreach (var collider in colliders)
                {
                    collider.isTrigger = true;
                }
                PhysicsReset();
                break;
            case Mode.Complex:
                // Enable physics
                //rigidbody.detectCollisions = true;
                //rigidbody.isKinematic = false;
                foreach (var collider in colliders)
                {
                    collider.isTrigger = false;
                }
                PhysicsReset();
                break;
        }
        _mode = newMode;
    }

    // Use this for initialization
    protected override void OnStart()
    {
        base.OnStart();
        _originalPosition = transform.position;
        _homeDistance = 0.0f;
        SetMode(defaultInteractionMode);
    }

    // Call update function for selected mode
    protected override void OnUpdate()
    {
        base.OnUpdate();
        switch (_mode)
        {
            case Mode.Simple:
                SimpleUpdate();
                break;
            case Mode.Complex:
                ComplexUpdate();
                break;
            case Mode.StaticTouch:
                StaticTouchUpdate();
                break;
        }
    }

    void SimpleUpdate()
    {
        // Collision-based clicking logic (see OnTriggerEnter/Exit)
    }

    void ComplexUpdate()
    {
        // Physics-based clicking logic - pass click distance from last 'home' point to enter,
        // return < 90% of 'away' point to exit
        float distFromStart = Vector3.Distance(_originalPosition, transform.position);
        if (_clicked)
        {
            if (distFromStart < 0.9f*_furthestDistance)
            {
                // Unclick
                _clicked = false;
                _homeDistance = distFromStart;
                OnClickExit();
                releaseEvent.Invoke();
            }
            else if (distFromStart > _furthestDistance)
            {
                _furthestDistance = distFromStart;
            }
        }
        else
        {
            if (distFromStart > _homeDistance + clickDistance)
            {
                _clicked = true;
                _furthestDistance = 0.0f;
                OnClickEnter();
                pressEvent.Invoke();
            }
            else if (distFromStart < _homeDistance)
            {
                _homeDistance = distFromStart;
            }
        }
    }

    void StaticTouchUpdate()
    {
        // Capacitive sensor-based clicking logic
        var comms = SocketTest.Instance;
        // Limited to the currently hovered input
        if (Hovered)
        {
            if (comms.CapacitiveSensor > clickThreshold && !_clicked)
            {
                _clicked = true;
                OnClickEnter();
                pressEvent.Invoke();
            }
            else if (comms.CapacitiveSensor < clickThreshold && _clicked)
            {
                _clicked = false;
                OnClickExit();
                releaseEvent.Invoke();
            }
        }
    }

    protected override void OnPhysicsReset()
    {
        base.OnPhysicsReset();
        transform.position = _originalPosition;
        _clicked = false;
        _homeDistance = 0.0f;
        GetComponent<Rigidbody>().velocity = new Vector3();
        _currentColliders.Clear();
    }

    protected override void OnMove()
    {
        base.OnMove();
        _originalPosition = transform.position;
        _clicked = false;
        _homeDistance = 0.0f;
        GetComponent<Rigidbody>().velocity = new Vector3();
        _currentColliders.Clear();
    }

    // Triggering for simple click model
    void OnTriggerEnter(Collider other)
    {
        _currentColliders.Add(other.gameObject);

        if (_mode == Mode.Simple && !_clicked)
        {
            _clicked = true;
            OnClickEnter();
            pressEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        _currentColliders.Remove(other.gameObject);

        // Only end click once all colliders have exited
        if (_mode == Mode.Simple && _clicked && _currentColliders.Count == 0)
        {
            _clicked = false;
            OnClickExit();
            releaseEvent.Invoke();
        }
    }
}
