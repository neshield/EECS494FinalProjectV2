﻿using UnityEngine;
using System.Collections;

public class SN_aimController : MonoBehaviour {
	private SN_playerController player;
	
	private LineRenderer line;
	
	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.enabled = false;
		player = this.transform.parent.gameObject.GetComponent<SN_playerController>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 AimVec = player.aimVector;
		Vector3 playerPos = player.transform.position;
		Vector3 lineEndPos = AimVec + playerPos;
		line.SetVertexCount (2);
		line.SetPosition (0, playerPos);
		line.SetPosition (1, lineEndPos);
		line.enabled = true;
	}

}