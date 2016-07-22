using UnityEngine;
using System.Collections;

public class PhysicsInput : MonoBehaviour {

	public float clickDistance = 0.02f;

	public Color objectColor;
	public Color hoverColor;
	public Color pressedColor;

	private Color currentColor;
	private Color originalColor;
	private Material materialColored;

	private Renderer myRenderer;

	private Collider _collider;
	public Collider collider {
		get {
			if (!_collider)
				_collider = GetComponent<Collider>();
			return _collider;
		}
	}

	private Vector3 _originalPosition;

	private bool _hovering = false;
	private bool _clicked = false;

	// TODO subclass this crap
	void ColorUpdate() {
		if (objectColor != currentColor) {
			// Clean up the old material
			if (materialColored != null) {
				string materialPath = UnityEditor.AssetDatabase.GetAssetPath(materialColored);
				UnityEditor.AssetDatabase.DeleteAsset(materialPath);
			}
			
			// Create a new material
			materialColored = new Material(Shader.Find("Standard"));
			materialColored.color = currentColor = objectColor;
			if (currentColor != originalColor) {
				materialColored.SetColor("_EmissionColor", objectColor);
				materialColored.EnableKeyword("_EMISSION");
			}
			myRenderer.material = materialColored;
		}
	}

	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<Renderer>();
		originalColor = objectColor;

		_originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Clicking logic - pass click distance to enter, return to 0.2*dist to exit
		float distFromStart = Vector3.Distance(_originalPosition, transform.position);
		if (_clicked) {
			if (distFromStart < 0.2f * clickDistance) {
				// Unclick
				_clicked = false;
				OnClickExit();
			}
		}
		else {
			if (distFromStart > clickDistance) {
				_clicked = true;
				OnClickEnter();
			}
		}

		ColorUpdate();
	}

	void OnCollisionEnter(Collision collision) {

	}

	void OnCollisionExit(Collision collision) {

	}

	/**
	 * Called when the object physically enters its depressed (clicked) state
	 */
	void OnClickEnter() {
		objectColor = pressedColor;
	}

	/**
	 * Called when the object physically leaves its depressed state
	 */
	void OnClickExit() {
		if (_hovering)
			objectColor = hoverColor;
		else
			objectColor = originalColor;
	}

	public void OnHoverEnter() {
		objectColor = hoverColor;
		_hovering = true;
	}

	public void OnHoverExit() {
		objectColor = originalColor;
		_hovering = false;
	}
}
