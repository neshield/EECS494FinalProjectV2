﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class PlayerObject : MonoBehaviour {
	static public PlayerObject P;

	Animator anim;
	public float move_FSM_val;
	public bool facingRight;

	public int playerNum;
	
	public Vector3 startPosition;
	public Vector3 velocity;
	private float jumpSpeed = 18.0f;
	private static Vector3 playerGravity = new Vector3(0.0f, -35.0f, 0.0f);
	
	public bool playing;
	
	public List<Collider> groundList;
	public List<Collider> leftWallList;
	public List<Collider> rightWallList;

	public GameObject bullet;
	
	bool ________________________________________;
	//Physics stuff
	private int hitGroundTimer = 0;
	private Vector3 spawnPos = new Vector3(19.6f, 7.1f, 0.0f);
	private Vector3 forcedVelocity;
	private int forcedVelFrameCounter;
	private float forcedVelAbsMax;
	private Vector3 controlledVelocity;
	
	Vector3	curGroundRightCornerPos;
	Vector3 curGroundLeftCornerPos;
	Vector3 prevGroundLeftCornerPos;
	Vector3 prevGroundRightCornerPos;
	Vector3	curTopRightCornerPos;
	Vector3 curTopLeftCornerPos;
	Vector3 prevTopLeftCornerPos;
	Vector3 prevTopRightCornerPos;

	Vector3 aimVector;

	bool jumpQueued;


	//SCOTT AND MATT SHOOTING STUFF
	public GameObject leftBullet;
	public GameObject rightBullet;
	public GameObject jumpBullet;
	private GameObject currentBullet;
	int facing = 0;

	private InputDevice inputDevice;

	// Use this for initialization
	void Start () {

		spawnPos = this.transform.position;

		P = this;
		RestoreDefaults ();
		playing = true;

		forcedVelocity = Vector3.zero;
		forcedVelAbsMax = 11.0f;
		controlledVelocity = Vector3.zero;

		forcedVelFrameCounter = 0;


		spawnPos = this.transform.position;
		aimVector.x = 0f;
		aimVector.y = 1f;
		aimVector.z = 0f;

		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		if(inputDevice == null){
			Destroy(this.gameObject);
		}

		jumpQueued = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (inputDevice.LeftBumper.WasPressed) {
			jumpQueued = true;
		}

		//Get aim vector
		Vector2 rsv = inputDevice.RightStick;
		if (Mathf.Approximately (rsv.magnitude, 0)) {
			rsv = aimVector;
		}

		aimVector.x = rsv.x;
		aimVector.y = rsv.y;
		aimVector.z = 0.0f;
		
		aimVector.Normalize();
		
		aimVector = aimVector * 35.0f;

		//SCOTT AND MATT
		if(inputDevice.LeftTrigger.WasPressed){
			currentBullet = Instantiate (bullet) as GameObject;
			BulletScript obj = currentBullet.GetComponent<BulletScript>();

			obj.setPlayerRef(this);
			obj.setPull();

			//canFire = false;

			obj.setPosition(transform.position);
			obj.setVelocity(aimVector);
		}
		else if(inputDevice.RightTrigger.WasPressed){
			currentBullet = Instantiate (bullet) as GameObject;
			BulletScript obj = currentBullet.GetComponent<BulletScript>();
			
			obj.setPlayerRef(this);
			obj.setPush();

			obj.setPosition(transform.position);
			obj.setVelocity(aimVector);
		}

	}

	public void queueJump(){
		jumpQueued = true;
	}

	void FixedUpdate(){
		if(playing){

			//Stuff Used for FSM//////////////////////////////
			move_FSM_val = inputDevice.LeftStickX.Value;
			anim.SetFloat("XSpeed", Mathf.Abs(move_FSM_val));
			anim.SetFloat("YSpeed", this.rigidbody.velocity.y);
			///////////////////////////////////////////////////
			
			if(move_FSM_val > 0 && !facingRight){
				Flip();
			}
			else if(move_FSM_val < 0 && facingRight){
				Flip();
			}


			//Handle the x movement
			controlledVelocity.x = 6.0f * inputDevice.LeftStickX.Value;
			
			//Apply walls
			if(rightWallList.Count != 0 && controlledVelocity.x > 0){
				controlledVelocity.x = 0;
			}
			if(leftWallList.Count != 0 && controlledVelocity.x < 0){
				controlledVelocity.x = 0;
			}

			//Handle the y movement / jumping
			if(jumpQueued && groundList.Count != 0){
				Jump ();
				jumpQueued = false;
			}
			else{
				jumpQueued = false;
			}

			if(groundList.Count == 0){
				controlledVelocity += playerGravity * Time.fixedDeltaTime;
			} else if (velocity.y <= 0.0f && groundList.Count > 0){
				velocity.y = 0.0f;
			}
		
			//Decay forcedVelocity
			forcedVelFrameCounter++;
			if(forcedVelFrameCounter >= 25){
				forcedVelocity *= .5f;
				forcedVelFrameCounter = 0;
				if(forcedVelocity.magnitude <= 1.0f){
					forcedVelocity = Vector3.zero;
				}
			}

			Vector3 pos = this.transform.position;

			velocity = forcedVelocity + controlledVelocity;
				
			//Move the player
			pos += velocity * Time.deltaTime;
			this.transform.position = pos;
	
		}
		prevGroundLeftCornerPos = curGroundLeftCornerPos;
		prevGroundRightCornerPos = curGroundRightCornerPos;
		curGroundLeftCornerPos = getGroundLeftCorner ();
		curGroundRightCornerPos = getGroundRightCorner ();
		prevTopLeftCornerPos = curTopLeftCornerPos;
		prevTopRightCornerPos = curTopRightCornerPos;
		curTopLeftCornerPos = getTopLeftCorner ();
		curTopRightCornerPos = getTopRightCorner ();
		hitGroundTimer--;
	}
	
	void OnTriggerEnter(Collider other){
		HandleBulletCollision (other);
		HandleGroundCollision (other);
		HandleSpikeCollision (other);
	}

	public void HitGround(){
		if (hitGroundTimer > 0) {
			return;
		}
		Vector3 groundForce = Vector3.zero;
		groundForce.y = -velocity.y;
		velocity += groundForce;
		controlledVelocity.y = 0.0f;
		hitGroundTimer = 10;
	}
	
	void OnTriggerStay(Collider other){
		OnTriggerEnter(other);
	}
	
	void OnTriggerExit(Collider other){
		if(other.GetComponent<GroundObject>()){
			groundList.Remove(other);
			rightWallList.Remove(other);
			leftWallList.Remove(other);
		}
	}

	void HandleBulletCollision(Collider other){
		BulletScript bs = other.GetComponent<BulletScript> ();
		if(bs && (bs.getPlayerRef() != this)){
			float pushPullScaling = 5.0f;
			if(bs.getBulletType() == bulletType.PULL){
				Vector3 diff = transform.position - bs.getPlayerRef().transform.position;
				diff = diff.normalized * pushPullScaling;
				//this.velocity += new Vector3(diff.x, diff.y, diff.z);
				forcedVelocity += new Vector3(diff.x, diff.y, diff.z);
				Destroy(other.gameObject);
			}
			else if(bs.getBulletType() == bulletType.PUSH){
				Vector3 diff = bs.getPlayerRef().transform.position - transform.position;
				diff = diff.normalized * pushPullScaling;
				//this.velocity += new Vector3(diff.x, diff.y * 2, diff.z);
				forcedVelocity += new Vector3(diff.x, diff.y * 2, diff.z);
				Destroy(other.gameObject);	
			}
			else if(bs.getBulletType() == bulletType.JUMP){
				this.velocity += new Vector3(0,16,0);
				Destroy(other.gameObject);
			}

			if(forcedVelocity.x > forcedVelAbsMax){
				forcedVelocity.x = forcedVelAbsMax;
			}
			if(forcedVelocity.x < -forcedVelAbsMax){
				forcedVelocity.x = -forcedVelAbsMax;
			}
			if(forcedVelocity.y > forcedVelAbsMax){
				forcedVelocity.y = forcedVelAbsMax;
			}
			if(forcedVelocity.y < -forcedVelAbsMax){
				forcedVelocity.y = -forcedVelAbsMax;
			}
		}
	}

	void HandleGroundCollision(Collider other){
		if(!other.GetComponent<GroundObject>()){
			return;
		}
		if(groundList.Contains(other)){
			return;
		}
		if(rightWallList.Contains(other)){
			return;
		}
		if(leftWallList.Contains(other)){
			return;
		}
		
		
		float groundYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * 1);
		float ceilYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * -1);
		
		float leftSideWallPos = other.transform.position.x - (other.transform.localScale.x / 2.0f);
		float rightSideWallPos = other.transform.position.x + (other.transform.localScale.x / 2.0f);
		
		float approxVal = 0.1f;
		
		bool shouldMoveOnTop = false;
		bool shouldBounceOffBottom = false;
		bool shouldAssignAsLeftWall = false;
		bool shouldAssignAsRightWall = false;
		
		//This section 
		if (IsInside (other.collider, curGroundLeftCornerPos) && IsInside (other.collider, curGroundRightCornerPos)) {
			shouldMoveOnTop = true;
		} else if (IsInside (other.collider, curGroundLeftCornerPos)) {
			Vector3 cornerPosChangeDirection = curGroundLeftCornerPos - prevGroundLeftCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevGroundLeftCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, approxVal)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, approxVal)){
				shouldAssignAsLeftWall = true;
			}
			
		} else if (IsInside (other.collider, curGroundRightCornerPos)) {
			Vector3 cornerPosChangeDirection = curGroundRightCornerPos - prevGroundRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevGroundRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, approxVal)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, approxVal)){
				shouldAssignAsRightWall = true;
			}
		} else if (IsInside (other.collider, curTopLeftCornerPos) && IsInside (other.collider, curTopRightCornerPos)) {
			shouldBounceOffBottom = true;
		} else if (IsInside (other.collider, curTopLeftCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopLeftCornerPos - prevTopLeftCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopLeftCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, approxVal)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, approxVal)){
				shouldAssignAsLeftWall = true;
			}
		} else if (IsInside (other.collider, curTopRightCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopRightCornerPos - prevTopRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, approxVal)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, approxVal)){
				shouldAssignAsRightWall = true;
			}
		}
		
		if (shouldMoveOnTop && (Mathf.Sign(velocity.y) == -1)) {
			if (Mathf.Approximately (other.bounds.min.x, this.collider.bounds.max.x)
			    || Mathf.Approximately (other.bounds.max.x, this.collider.bounds.min.x)) {
				return;
			}
			//ONLY HIT GROUND IF VEL IS GOING DOWN
			HitGround ();
			if(!groundList.Contains(other)){
				groundList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y + radiAdd;
			this.transform.position = newPos;
		} 
		else if (shouldAssignAsLeftWall){
			if(!leftWallList.Contains(other)){
				leftWallList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.x) / 2.0f + (other.transform.localScale.x) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.x = groundPos.x + radiAdd + 0.01f;
			forcedVelocity.x = 0.0f;
			this.transform.position = newPos;
		} 
		else if (shouldAssignAsRightWall){
			//currentRightWall = other.gameObject.GetComponent<SolidObject>();
			if(!rightWallList.Contains(other)){
				rightWallList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.x) / 2.0f + (other.transform.localScale.x) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.x = groundPos.x - radiAdd - 0.01f;
			forcedVelocity.x = 0.0f;
			this.transform.position = newPos;
		} 
		else if (shouldBounceOffBottom && (Mathf.Sign(velocity.y) == 1)){
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y - radiAdd;// - 0.05f;
			velocity.y = 0.0f;
			controlledVelocity.y = 0.0f;
			forcedVelocity.y = 0.0f;
			this.transform.position = newPos;
		}
	}

	void HandleSpikeCollision(Collider other){
		if (other.gameObject.tag == "spike") {
			Die ();
		} else {
			return;
		}
	}

	void Die(){
		this.transform.position = spawnPos;
		RestoreDefaults ();
	}
	
	//ACTIONS
	public void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	public void Jump(){
		controlledVelocity.y += jumpSpeed;
	}
	
	public void Reverse(){
		velocity.x = -velocity.x;
	}
	
	public void RestoreDefaults(){
		groundList = new List<Collider> ();
		rightWallList = new List<Collider> ();
		leftWallList = new List<Collider> ();
		velocity = Vector3.zero;
		controlledVelocity = Vector3.zero;
		forcedVelocity = Vector3.zero;

		hitGroundTimer = 0;
	}



	//BOOKKEEPING
	private Vector3 getGroundLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -1;
		return pos;
	}
	
	private Vector3 getGroundRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -1;
		return pos;
	}
	
	private Vector3 getTopLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f);
		return pos;
	}
	
	private Vector3 getTopRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f);
		return pos;
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
