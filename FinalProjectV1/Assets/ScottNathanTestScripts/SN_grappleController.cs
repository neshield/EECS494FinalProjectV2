using UnityEngine;
using System.Collections;

public class SN_grappleController : MonoBehaviour {
	SN_playerController shooter;
	Vector3 startPos;

	public void setShooterRef(SN_playerController shooterRef_){
		shooter = shooterRef_;
	}

	public SN_playerController getPlayerRef(){
		return shooter;
	}

	public void setVelocity(Vector3 velocity_){
		rigidbody.velocity += velocity_;
	}
	
	public void setPosition(Vector3 position_){
		transform.position = position_;
		startPos = position_;
	}

	public void informShooterOnThrowBegin(SN_playerController thrown){
		shooter.throwPlayer (thrown);

		//After the shooter knows they've thrown, grapple can disappear.
		Destroy (this.gameObject);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 distTraveled = startPos - this.transform.position;
		if(distTraveled.magnitude > 4.0f){
			Destroy(this.gameObject);
		}
	}

}
