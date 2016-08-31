using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class KeyboardListener : MonoBehaviour
{
    public virtual void OnKeyDown(PhysicsKeyboard.PhysicsKey key) { return; }
    public virtual void OnKeyUp(PhysicsKeyboard.PhysicsKey key) { return; }
}

