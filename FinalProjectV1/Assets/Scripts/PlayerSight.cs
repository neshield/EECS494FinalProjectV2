using UnityEngine;
using System.Collections;

public class PlayerSight : MonoBehaviour {
	private PlayerObject player;

	private LineRenderer line;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.enabled = false;
		player = this.transform.parent.gameObject.GetComponent<PlayerObject>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 AimVec = player.aimVector * 35.0f;
		Vector3 playerPos = player.transform.position;
		playerPos.y += 0.2f;
		Vector3 lineEndPos = AimVec + playerPos;
		line.SetVertexCount (2);
		line.SetPosition (0, playerPos);
		line.SetPosition (1, lineEndPos);
		line.enabled = true;
	}
}
