using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ToggleEvent : MonoBehaviour {

    public UnityEvent onSet;
    public UnityEvent onUnset;
    public bool triggerOnStart = false;

    [SerializeField]
    private bool _set = false;
    public bool Set
    {
        get
        {
            return _set;
        }
        set
        {
            _set = value;
            DoTrigger();
        }
    }

	// Use this for initialization
	void Start () {
	    if (triggerOnStart)
        {
            DoTrigger();
        }
	}
	
	public void Trigger()
    {
        _set = !_set;
        DoTrigger();
    }

    private void DoTrigger()
    {
        if (_set)
        {
            onSet.Invoke();
        }
        else
        {
            onUnset.Invoke();
        }
    }
}
