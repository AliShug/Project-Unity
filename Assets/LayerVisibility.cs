using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class LayerVisibility : MonoBehaviour {
    private int _armInd = 9;
    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    public bool ArmVisible
    {
        get { return (1<<_armInd & _cam.cullingMask) != 0; }
        set
        {
            int mask = 1 << _armInd;
            if (value)
            {
                _cam.cullingMask &= ~mask;
            }
            else
            {
                _cam.cullingMask |= mask;
            }
        }
    }
}
