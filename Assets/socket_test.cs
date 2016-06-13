using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;

public class socket_test : MonoBehaviour {

	public int port = 14002;

	private int n = 0;
	private UdpClient sender;
	private UdpClient receiver;

	// Use this for initialization
	void Start () {
		sender = new UdpClient ("localhost", port);
		receiver = new UdpClient (port + 1);
	}
	
	// Update is called once per frame
	void Update () {
		string nstr = n.ToString ();
		byte[] bytes = Encoding.ASCII.GetBytes (nstr);
		sender.Send(bytes, nstr.Length);

		n++;
	}
}
