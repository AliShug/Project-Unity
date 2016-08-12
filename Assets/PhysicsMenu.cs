using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhysicsMenu : MonoBehaviour {

    public bool touchEnabled = false;
    public bool showIntroScreen = false;
    public bool showEndScreen = false;

    [SerializeField]
    private PhysicsMenu _previous;
    [SerializeField]
    private PhysicsMenu _next;

    public virtual PhysicsMenu PreviousMenu {
        get {
            return _previous;
        }
        protected set {
            _previous = value;
        }
    }
    public virtual PhysicsMenu NextMenu {
        get {
            return _next;
        }
        protected set {
            _next = value;
        }
    }

    // In/out events
    public UnityEvent onShow;
    public UnityEvent onHide;

    private IEnumerator _switchTimer;
    private bool _selected = false;

    public IEnumerator WaitAndSwitch(float time, PhysicsMenu menu) {
        yield return new WaitForSeconds(0.1f);
        Hide();
        yield return new WaitForSeconds(time);
        GetComponentInParent<PhysicsInputManager>().CurrentMenu = menu;
        menu.Show();
    }

    protected virtual void OnShow() {
        return;
    }

    protected virtual void OnHide() {
        return;
    }

    public void Show() {
        var children = GetComponentsInChildren<InteractiveObject>(true);
        foreach (var child in children) {
            child.PhysicsReset();
            child.gameObject.SetActive(true);
        }
        onShow.Invoke();
        OnShow();

        _selected = true;
        ApplySettings();
    }

	public void Hide() {
        onHide.Invoke();
        OnHide();
        var children = GetComponentsInChildren<InteractiveObject>(true);
        foreach (var child in children) {
            child.gameObject.SetActive(false);
        }

        _selected = false;
    }

    public void Next() {
        if (NextMenu != null) {
            SwitchTo(NextMenu);
        }
    }

    public void Previous() {
        if (PreviousMenu != null) {
            SwitchTo(PreviousMenu);
        }
    }

    public void SwitchTo(PhysicsMenu menu) {
        _switchTimer = WaitAndSwitch(0.3f, menu);
        StartCoroutine(_switchTimer);

        RecordingManager.Instance.Log(string.Format(
            "EVENT: menu_transition from=<{0}> to=<{1}>",
            name, menu.name));
    }

    private void ApplySettings() {
        InfoText textComp = GetComponent<InfoText>();
        if (textComp != null) {
            TactisDemo.Instance.infoText.text = textComp.text;
        }

        Animator infoAnim = TactisDemo.Instance.infoAnim;
        infoAnim.SetBool("show_intro", showIntroScreen);
        infoAnim.SetBool("end", showEndScreen);
        infoAnim.SetBool("show_pane", textComp != null);

        if (touchEnabled) {
            GetComponentInParent<PhysicsInputManager>().EnableTouch();
        }
        else {
            GetComponentInParent<PhysicsInputManager>().DisableTouch();
        }
    }
}
