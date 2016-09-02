using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class ColorChanger : MonoBehaviour {

    public Color enableColor, disableColor;

    private Material _uniqueMaterial;

	void Awake() {
        var renderer = GetComponent<Renderer>();
        _uniqueMaterial = new Material(renderer.material);
        renderer.material = _uniqueMaterial;
	}

    public void SetColor(Color col)
    {
        _uniqueMaterial.color = col;
    }

    public void SetGreen()
    {
        SetColor(Color.green);
    }

    public void SetRed()
    {
        SetColor(Color.red);
    }

    public void SetEnableCol(bool enable)
    {
        if (enable)
        {
            SetColor(enableColor);
        }
        else
        {
            SetColor(disableColor);
        }
    }
}
