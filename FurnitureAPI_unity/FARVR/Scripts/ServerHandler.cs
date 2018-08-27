using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandler : MonoBehaviour {

	private RoomOptions roomOptions;

	private int RoomID = 1;

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings ("v1.0");
		roomOptions = new RoomOptions () { IsVisible = true, MaxPlayers = 6 };
	}

	void OnConnectedToPhoton() {
		Debug.Log ("Connected to Photon");
	}

	void OnJoinedLobby() {
		Debug.Log ("Joined Lobby");
		PhotonNetwork.JoinOrCreateRoom (RoomID.ToString (), roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom() {
		Debug.Log ("Successfully joined room " + RoomID.ToString());
	}

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
		Debug.Log (newPlayer.ID.ToString () + " has joined the room");
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer newPlayer) {
		Debug.Log (newPlayer.ID.ToString () + " has disconnected");		
	}

	void OnDisconnectedFromPhoton() {
		Debug.Log ("Disconnected from Photon Server");
	} 
}
