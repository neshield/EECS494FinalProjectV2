using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillScript : MonoBehaviour {
	List<int> playerScores = new List<int>();
	List<int> playerScoreTicks = new List<int>();
	List<PlayerBaseClass> playerObjectList = new List<PlayerBaseClass> ();

	// Use this for initialization
	void Start () {
		for(int i=0; i < 4; i++){
			playerScores.Add (0);
			playerScoreTicks.Add (0);
		}


		playerObjectList.Capacity = 4;

		GameObject[] players;
		players = GameObject.FindGameObjectsWithTag ("Player");
		print (players.Length);

		foreach (PlayerBaseClass player in players) {
						print ("player exists...");
				}

		/*
		foreach (GameObject player in players) {
			print (player);
			SN_playerController snpc = player.GetComponent<SN_playerController>();
			if(snpc != null){
				playerObjectList[snpc.playerNum] = snpc.gameObject;
				print ("added snpc...");
			} else {
				PlayerObject po = player.GetComponent<PlayerObject>();
				if(po != null){
					playerObjectList[po.playerNum] = po.gameObject;
					print ("added po...");
				}
			}
		}
		*/

		for(int i=0; i < playerObjectList.Capacity; i++){
			print (playerObjectList[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//http://answers.unity3d.com/questions/163864/test-if-point-is-in-collider.html
	//Answer by DarkharStudio
	static public bool IsInside ( Collider test, Vector3 point)
	{
		Vector3    center;
		Vector3    direction;
		Ray        ray;
		RaycastHit hitInfo;
		bool       hit;
		
		// Use collider bounds to get the center of the collider. May be inaccurate
		// for some colliders (i.e. MeshCollider with a 'plane' mesh)
		center = test.bounds.center;
		
		// Cast a ray from point to center
		direction = center - point;
		ray = new Ray(point, direction);
		hit = test.Raycast(ray, out hitInfo, direction.magnitude);
		
		// If we hit the collider, point is outside. So we return !hit
		return !hit;
	}
}
