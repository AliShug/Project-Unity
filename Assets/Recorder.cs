using UnityEngine;
using System.Collections;

// Placeholder for drawer script
public class UniqueIDAttribute : PropertyAttribute {}

public class Recorder : MonoBehaviour {
    [UniqueID]
    public string uniqueID;
}
