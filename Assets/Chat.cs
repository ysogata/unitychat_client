using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class Chat : MonoBehaviour {
	private string serverConfFile = "server_info.conf";
	private string serverUri = "";
	void Start () {
		InitServerUri();
		Connect();
	}

	void Update () {
		
	}

	string message = "";

	List<string> messages = new List<string>(10);

	void OnGUI(){
		
		// Input text
		message = GUI.TextArea(new Rect(10,Screen.height*0.9f - 10,Screen.width * 0.7f,Screen.height / 10),message);
		
		// Send message button
		if(GUI.Button(new Rect(Screen.width * 0.75f,Screen.height*0.9f - 10,Screen.width * 0.2f,Screen.height / 10),"Send")){
			SendChatMessage();
		}
		
		// Show chat messages
		var l = string.Join("\n",messages.ToArray());
		var myStyle = new GUIStyle();
		var stringSize = myStyle.CalcSize(new GUIContent(l));
		var height = stringSize.y + 10;
		GUI.Label(
			new Rect(10,Screen.height*0.9f - 10 - height,Screen.width * 0.8f,height),
			l);
		
	}
	
	WebSocket ws;
	
	void Connect(){
		ws =  new WebSocket(serverUri);
		
		// called when websocket messages come.
		ws.OnMessage += (sender, e) =>
		{
			string s = e.Data;
			Debug.Log(string.Format( "Receive {0}",s));
			messages.Add("> " + e.Data);
			if(messages.Count > 10){
				messages.RemoveAt(0);
			}
		};
		
		ws.Connect();
		Debug.Log("Connect to " + ws.Url);
	}
	
	void SendChatMessage(){
		Debug.Log("Send message " + message);
		ws.Send(message);
		this.message = "";
	}

	//////////////

	void InitServerUri(){
		FileInfo fi = new FileInfo(Application.dataPath + "/" + serverConfFile);
		try {
			using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)){
				serverUri = sr.ReadToEnd();
			}
		} catch (Exception e){
			serverUri = "ws://localhost:8888/";
		}
	}
}