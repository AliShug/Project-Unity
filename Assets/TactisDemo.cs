using UnityEngine;
using System.Collections;

public class TactisDemo : MonoBehaviour {

    enum Stage {
        Start,
        Intro0,
        Intro1,
        HandSelect,
        Tutorial0,
        Reaction0,
        End
    }

    public PhysicsInputManager vrInput;
    public Animator infoAnim;

    private Stage _curStage = Stage.Start;
    private bool _advanceFlag = false;

	// Use this for initialization
	void Start () {
	
	}

    void ControlAdvance(bool nextMenu = false) {
        if (Input.GetButtonDown("Submit")) {
            _curStage++;
            if (nextMenu) {
                NextMenu();
            }
            // Clamp to start/end
            _curStage = (Stage)Mathf.Clamp((int)_curStage, (int)Stage.Start, (int)Stage.End);
        }
    }

    void UserAdvance() {
        if (_advanceFlag) {
            _curStage++;
            _advanceFlag = false;
            // Clamp to start/end
            _curStage = (Stage)Mathf.Clamp((int)_curStage, (int)Stage.Start, (int)Stage.End);
        }
    }

    void NextMenu() {
        if (vrInput) {
            vrInput.CurrentMenu.Next();
        }
    }

	// Update is called once per frame
	void Update () {
        bool intro0 = false;
        bool intro1 = false;
        bool handSelect = false;
        bool pane = true;
        bool endScreen = false;

        bool reactionTut0 = false;

        // Stately-ish behaviour
        switch (_curStage) {
            case Stage.Start:
                ControlAdvance();
                pane = false;
                break;
            case Stage.Intro0:
                ControlAdvance();
                pane = false;
                intro0 = true;
                break;
            case Stage.Intro1:
                ControlAdvance(true);
                intro1 = true;
                break;
            case Stage.HandSelect:
                UserAdvance();
                handSelect = true;
                break;
            case Stage.Tutorial0:
                ControlAdvance(true);
                reactionTut0 = true;
                break;
            case Stage.Reaction0:
                UserAdvance();
                pane = false;
                break;
            case Stage.End:
                pane = false;
                endScreen = true;
                break;
            default:
                break;
        }

        // Show information content w/ animations
        infoAnim.SetBool("show_pane", pane);
        infoAnim.SetBool("show_hand_text", handSelect);
        infoAnim.SetBool("show_intro", intro0);
        infoAnim.SetBool("show_introtext", intro1);
        infoAnim.SetBool("reaction_tutorial", reactionTut0);
        infoAnim.SetBool("end", endScreen);
	}

    public void Advance() {
        _advanceFlag = true;
    }
}
