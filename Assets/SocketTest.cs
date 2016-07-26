﻿using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Net.Sockets;

public class SocketTest : MonoBehaviour {

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
	private float shoulderAngle, mainArmAngle, forearmAngle, wristX, wristY;

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
		// Updating the IK goal - UDP packet sent to the python app
        //  1 x 1 : enable-arm
        //  4 x 3 : goal-position
        //  4 x 3 : goal-orient
		byte[] send_raw = new byte[4 + sizeof(float)*6];
        Vector3 norm = transform.InverseTransformDirection(interactionTargetTransform.forward);
		Vector3 pos = transform.InverseTransformPoint(interactionTargetTransform.position);

        // enable
        send_raw[0] = Convert.ToByte(false);
        send_raw[1] = 0;
        send_raw[2] = 0;
        send_raw[3] = 0;
        // goal pos
        System.BitConverter.GetBytes(pos.x).CopyTo(send_raw, 4);
		System.BitConverter.GetBytes(pos.y).CopyTo(send_raw, 4 + 1*sizeof(float));
		System.BitConverter.GetBytes(pos.z).CopyTo(send_raw, 4 + 2*sizeof(float));
        // goal orient
        System.BitConverter.GetBytes(norm.x).CopyTo(send_raw, 4 + 3*sizeof(float));
        System.BitConverter.GetBytes(norm.y).CopyTo(send_raw, 4 + 4*sizeof(float));
        System.BitConverter.GetBytes(norm.z).CopyTo(send_raw, 4 + 5*sizeof(float));
		sockOut.Send(send_raw);

		// Receive stuff
		int nbytes = sockIn.Available;
		// Exhaustively empty the socket's buffer
		byte[] rawData = new byte[4096];
		while (sockIn.Available > 0) {
			sockIn.Receive(rawData);

			// Extract packed floating-point data from the raw bytes
			shoulderAngle = System.BitConverter.ToSingle(rawData, 0) * Mathf.Rad2Deg;
			mainArmAngle = System.BitConverter.ToSingle(rawData, 4) * Mathf.Rad2Deg;
			forearmAngle = System.BitConverter.ToSingle(rawData, 8) * Mathf.Rad2Deg;
            wristX = System.BitConverter.ToSingle(rawData, 12);
            wristY = System.BitConverter.ToSingle(rawData, 16);
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
		euler.y = wristX - 150;
		wristYawTransform.localRotation = Quaternion.Euler (euler);

		// And pitch
		euler = wristPitchTransform.localRotation.eulerAngles;
		euler.z = wristY - 150;
		wristPitchTransform.localRotation = Quaternion.Euler (euler);


//		if (_frame % 60 == 0) {
//			Debug.Log (_value);
//		}

		_frame++;
	}
}
