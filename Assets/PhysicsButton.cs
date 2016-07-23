using UnityEngine;
using System.Collections;

public class PhysicsButton : PhysicsInput {

	public Color baseColor;
	public Color hoverColor;
	public Color pressedColor;

	private Color currentColor;
	private Material materialOriginal;
	private Material materialUnique;

	private Renderer myRenderer;

	protected override void OnStart() {
		// Create a new 
		myRenderer = GetComponent<Renderer>();
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
	void ColorUpdate() {
		// Modify our unique material's emissive properties
		materialUnique.SetColor("_EmissionColor", currentColor);
	}

	/**
	 * Called when the object physically enters its depressed (clicked) state
	 */
	protected override void OnClickEnter() {
		currentColor = pressedColor;
		ColorUpdate();
	}

	/**
	 * Called when the object physically leaves its depressed state
	 */
	protected override void OnClickExit() {
		if (Hovered)
			currentColor = hoverColor;
		else
			currentColor = baseColor;
		ColorUpdate();
	}

	protected override void OnHoverEnter() {
		currentColor = hoverColor;
		ColorUpdate();
	}

	protected override void OnHoverExit() {
		currentColor = baseColor;
		ColorUpdate();
	}
}
