using UnityEngine;
using System.Collections;

public class CustomPlayback : MonoBehaviour
{
    public Recorder target;
    public string uniqueID
    {
        get
        {
            return target.uniqueID;
        }
    }
}
