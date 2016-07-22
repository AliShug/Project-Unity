using UnityEngine;
using System.Collections;

public class SurfacePredictor : MonoBehaviour {

	public Transform armBase;
	public Transform[] hands;

	private Plane interaction_plane = new Plane (new Vector3 (0, 0, 1), new Vector3 (0, 0, 0.2f));
	
	// Update is called once per frame
	void Update () {
		// Find nearest hand
		Transform nearest = null;
		float nearest_dist = Mathf.Infinity;
		for (int i = 0; i < hands.Length; i++) {
			// Skip invalid or undetected hands
			if (hands [i] == null || !hands[i].gameObject.activeInHierarchy)
				continue;
			Vector3 hand_pos = armBase.InverseTransformPoint (hands [i].position);
			float dist = interaction_plane.GetDistanceToPoint(hand_pos);
			if (dist < nearest_dist) {
				nearest = hands [i];
				nearest_dist = dist;
			}
		}

		if (nearest != null) {
			// Place on plane near hand
			// just project hand position along plane normal to plane
			Vector3 hand_pos = armBase.InverseTransformPoint(nearest.position);
			float normal_dist = interaction_plane.GetDistanceToPoint (hand_pos);
			Vector3 translation = Vector3.Normalize (interaction_plane.normal) * -normal_dist;
			Vector3 new_pos = hand_pos + translation;
			new_pos.y -= 0.1f;
			transform.localPosition = new_pos;
		}
	}
}
