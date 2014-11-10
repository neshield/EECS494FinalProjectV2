using UnityEngine;
using System.Collections;

public class Lavamovement : MonoBehaviour {


	public GameObject lava;
	int facing = 1;
	public int offset;
	public int offset2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	


		if (facing == 1) {
						lava.rigidbody.velocity = new Vector3 (2, 0, 0);

				} else {
						lava.rigidbody.velocity = new Vector3 (-3, 0, 0);
				}
		if (lava.transform.position.x > offset + this.transform.position.x + (this.transform.localScale.x / 2)) {

			facing = 0;
				} else if (lava.transform.position.x < offset2 + this.transform.position.x - (this.transform.localScale.x / 2)) {

			facing = 1;
				}
	}	
}
