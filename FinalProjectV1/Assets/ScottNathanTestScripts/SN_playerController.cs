using UnityEngine;
using System.Collections;

public class SN_playerController : MonoBehaviour {

	private Vector3 xMovement = new Vector3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		xMovement = inputDevice.LeftStickX.Value;
	}
}
