using UnityEngine;
using System.Collections;

public class ThrowLineScript : MonoBehaviour {
	private SN_playerController player;
	private SN_playerController thrower;
	private LineRenderer line;
	
	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.enabled = false;
		player = this.transform.parent.gameObject.GetComponent<SN_playerController>();
	}
	
	// Update is called once per frame
	void Update () {
		//YOU BOUT TO GET TOSSSSSEEEDD
		if (thrower != null) {
			Vector3 AimVec = thrower.aimVector;
			Vector3 playerPos = player.transform.position;
			Vector3 lineEndPos = AimVec + playerPos;
			line.SetVertexCount (2);
			line.SetPosition (0, playerPos);
			line.SetPosition (1, lineEndPos);
			line.enabled = true;
		}
	}

	public void setThrowerRef(SN_playerController thrower_){
		thrower = thrower_;
	}

	public void removeThrowerRef(){
		thrower = null;
		line.SetVertexCount (0);
		line.enabled = false;
	}
}
