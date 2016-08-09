using UnityEngine;
using System.Collections;

public class PhysicsMenu : MonoBehaviour {

    public PhysicsMenu previous;
    public PhysicsMenu next;

    private IEnumerator switchTimer;

    public IEnumerator WaitAndSwitch(float time, PhysicsMenu menu) {
        yield return new WaitForSeconds(0.05f);
        Hide();
        yield return new WaitForSeconds(time);
        menu.Show();
    }

    protected virtual void OnShow() {
        return;
    }

    protected virtual void OnHide() {
        return;
    }

    public void Show() {
        OnShow();
        var children = GetComponentsInChildren<InteractiveObject>(true);
        foreach (var child in children) {
            child.PhysicsReset();
            child.gameObject.SetActive(true);
        }
    }

	public void Hide() {
        OnHide();
        var children = GetComponentsInChildren<InteractiveObject>(true);
        foreach (var child in children) {
            child.gameObject.SetActive(false);
        }
    }

    public void Next() {
        if (next != null) {
            SwitchTo(next);
        }
    }

    public void Previous() {
        if (previous != null) {
            SwitchTo(previous);
        }
    }

    public void SwitchTo(PhysicsMenu menu) {
        switchTimer = WaitAndSwitch(0.3f, menu);
        StartCoroutine(switchTimer);

        RecordingManager.Instance.Log(string.Format(
            "EVENT: menu_transition from=<{0}> to=<{1}>",
            name, menu.name));
    }
}
