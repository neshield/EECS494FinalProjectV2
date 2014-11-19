using UnityEngine;
using System.Collections;

public class SN_grappleController : MonoBehaviour {
	SN_playerController shooter;

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
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
