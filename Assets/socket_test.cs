using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Net.Sockets;

public class socket_test : MonoBehaviour {

	public int targetPort = 14001;
	public int bindPort = 14002;

	private Socket sockIn;
	private Socket sockOut;

	// Use this for initialization
	void Start() {
		sockIn = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		sockIn.Bind(new IPEndPoint(IPAddress.Loopback, bindPort));

		sockOut = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		sockOut.Connect(new IPEndPoint(IPAddress.Loopback, targetPort));

//		// Stop a stupid error
//		uint IOC_IN = 0x80000000;
//		uint IOC_VENDOR = 0x18000000;
//		uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
//		udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

		Debug.Log("Bound " + bindPort.ToString());
	}
	
	// Update is called once per frame
	void Update() {
		// Send stuff
		string nstr = "hello";
		byte[] bytes = Encoding.ASCII.GetBytes(nstr);
		int sent = sockOut.Send(bytes);

		// Receive stuff
		int nbytes = sockIn.Available;
		if (nbytes > 0) {
			byte[] data = new byte[4096];
			sockIn.Receive(data);
			Debug.Log(Encoding.ASCII.GetString(data));
		}
	}
}
