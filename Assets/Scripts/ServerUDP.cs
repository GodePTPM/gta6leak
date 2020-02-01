using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
 
public class ServerUDP : MonoBehaviour {
	private byte[] data;
	private Socket server;
	private IPEndPoint ipep;
 
	void Start() {
		ipep = new IPEndPoint(IPAddress.Parse("176.152.202.128"), 9050);
		server = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
		data = new byte[1024];
		string myIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
		string joinMessage = "["+ myIP +"] has joined the game.";
		data = Encoding.ASCII.GetBytes(joinMessage);
		server.SendTo(data, data.Length, SocketFlags.None, ipep);
		// Console.WriteLine("Message received from {0}:", tmpRemote.ToString());
		// Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

		//server.Close();
		// Debug.Log(tmpRemote.ToString());
	}

	void OnApplicationQuit() {
		data = new byte[1024];
		string myIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
		string dataSend = "["+ myIP +"] has left the game.";
		data = Encoding.ASCII.GetBytes(dataSend);
		server.SendTo(data, data.Length, SocketFlags.None, ipep);
	}

	void Update() {
		//data = new byte[1024];
		// string posSend = "gode PosX:"+ transform.position.x +" / posY: "+ transform.position.y +" / posZ "+ transform.position.z;
		// data = Encoding.ASCII.GetBytes(posSend);
		// server.SendTo(data, data.Length, SocketFlags.None, ipep);
	}
}