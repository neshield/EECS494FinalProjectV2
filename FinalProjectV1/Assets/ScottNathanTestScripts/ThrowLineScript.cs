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

		} else {

		}

	
	}

	void setThrowerRef(SN_playerController thrower_){
		thrower = thrower_;
	}

	void removeThrowerRef(){
		thrower = null;
	}
}
