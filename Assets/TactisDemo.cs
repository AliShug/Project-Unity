using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TactisDemo : MonoBehaviour
{

    private static TactisDemo _instance;
    public static TactisDemo Instance
    {
        get
        {
            return _instance;
        }
    }

    public TactisDemo()
    {
        _instance = this;
    }

    public PhysicsInputManager vrInput;
    public Animator infoAnim;
    public Text infoText;
}
