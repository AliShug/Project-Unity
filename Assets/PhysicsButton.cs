using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhysicsButton : PhysicsInput
{
    public Color enableColor, disableColor;

    public Color baseColor;
    public Color hoverColor;
    public Color pressedColor;

    private Color currentColor;
    private Material materialOriginal;
    private Material materialUnique;

    public Renderer myRenderer;
    private Light myLight = null;

    protected override void OnStart()
    {
        base.OnStart();

        myLight = GetComponent<Light>();

        materialOriginal = myRenderer.material;
        materialUnique = new Material(materialOriginal);
        materialUnique.EnableKeyword("_EMISSION");
        myRenderer.material = materialUnique;
        currentColor = baseColor;

        ColorUpdate();
    }

    /**
	 * Update the button's emissive highlight
	 */
    void ColorUpdate()
    {
        // Modify our unique material's emissive properties
        if (materialUnique)
        {
            materialUnique.SetColor("_EmissionColor", currentColor);
            if (myLight)
            {
                myLight.color = currentColor;
            }
        }
    }

    /**
	 * Called when the object physically enters its depressed (clicked) state
	 */
    protected override void OnClickEnter()
    {
        base.OnClickEnter();
        currentColor = pressedColor;
        ColorUpdate();
    }

    /**
	 * Called when the object physically leaves its depressed state
	 */
    protected override void OnClickExit()
    {
        base.OnClickExit();
        if (Hovered)
            currentColor = hoverColor;
        else
            currentColor = baseColor;
        ColorUpdate();
    }

    protected override void OnHoverEnter()
    {
        base.OnHoverEnter();
        currentColor = hoverColor;
        ColorUpdate();
    }

    protected override void OnHoverExit()
    {
        base.OnHoverExit();
        currentColor = baseColor;
        ColorUpdate();
    }

    protected override void OnPhysicsReset()
    {
        base.OnPhysicsReset();
        currentColor = baseColor;
        ColorUpdate();
    }

    // Color changing stuff
    public void SetColor(Color col)
    {
        materialUnique.color = col;
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
