using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Net.Sockets;

public class socket_test : MonoBehaviour {

	public int targetPort = 14001;
	public int bindPort = 14002;

	public Transform interactionTargetTransform;

	public Transform shoulderTransform;
	public Transform mainArmTransform;
	public Transform forearmTransform;
	public Transform wristPlatformTransform;
	public Transform wristYawTransform;
	public Transform wristPitchTransform;

	private Socket sockIn;
	private Socket sockOut;

	private int _frame = 0;
	private float shoulderAngle, mainArmAngle, forearmAngle;

	// Use this for initialization
	void Start() {
		sockIn = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		try {
			sockIn.Bind(new IPEndPoint(IPAddress.Loopback, bindPort));
			Debug.Log("Bound " + bindPort.ToString());

			sockOut = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			sockOut.Connect(new IPEndPoint(IPAddress.Loopback, targetPort));

			// Initialize angles to set transforms
			shoulderAngle = shoulderTransform.localRotation.eulerAngles.y;
			mainArmAngle = 90 - mainArmTransform.localRotation.eulerAngles.z;
			forearmAngle = - forearmTransform.localRotation.eulerAngles.z;
		}
		catch {
			Debug.LogError ("Unable to initialize socket connection");
			enabled = false;
		}
	}
	
	// Update is called once per frame, if enabled
	void Update() {
		// Send stuff
//		string nstr = "hello";
//		byte[] bytes = Encoding.ASCII.GetBytes(nstr);
//		int sent = sockOut.Send(bytes);

		byte[] send_raw = new byte[sizeof(float) * 3];
		System.BitConverter.GetBytes (interactionTargetTransform.localPosition.x).CopyTo(send_raw, 0 * sizeof(float));
		System.BitConverter.GetBytes (interactionTargetTransform.localPosition.y).CopyTo(send_raw, 1 * sizeof(float));
		System.BitConverter.GetBytes (interactionTargetTransform.localPosition.z).CopyTo(send_raw, 2 * sizeof(float));
		sockOut.Send (send_raw);

		// Receive stuff
		int nbytes = sockIn.Available;
		// Exhaustively empty the socket's buffer
		byte[] rawData = new byte[4096];
		while (sockIn.Available > 0) {
			sockIn.Receive(rawData);

			// Extract packed floating-point data from the raw bytes
			shoulderAngle = System.BitConverter.ToSingle (rawData, 0) * Mathf.Rad2Deg;
			mainArmAngle = System.BitConverter.ToSingle (rawData, 4) * Mathf.Rad2Deg;
			forearmAngle = System.BitConverter.ToSingle (rawData, 8) * Mathf.Rad2Deg;
		}

		// Set the arm configuration
		Vector3 euler;
		euler = shoulderTransform.localRotation.eulerAngles;
		euler.y = 90 + shoulderAngle;
		shoulderTransform.localRotation = Quaternion.Euler (euler);

		euler = mainArmTransform.localRotation.eulerAngles;
		euler.z = 90 - mainArmAngle;
		mainArmTransform.localRotation = Quaternion.Euler (euler);

		euler = forearmTransform.localRotation.eulerAngles;
		euler.z = - forearmAngle;
		forearmTransform.localRotation = Quaternion.Euler (euler);

		// We rotate the end-effector platform flush to the ground plane
		euler = wristPlatformTransform.localRotation.eulerAngles;
		euler.z = mainArmAngle + forearmAngle - 90;
		wristPlatformTransform.localRotation = Quaternion.Euler (euler);

		// Wrist's lateral movement
		euler = wristYawTransform.localRotation.eulerAngles;
		euler.y = -shoulderAngle;
		wristYawTransform.localRotation = Quaternion.Euler (euler);

		// And pitch
		euler = wristPitchTransform.localRotation.eulerAngles;
		euler.z = 0.0f;
		wristPitchTransform.localRotation = Quaternion.Euler (euler);


//		if (_frame % 60 == 0) {
//			Debug.Log (_value);
//		}

		_frame++;
	}
}
