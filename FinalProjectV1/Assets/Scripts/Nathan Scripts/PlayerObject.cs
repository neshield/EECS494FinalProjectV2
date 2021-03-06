﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class PlayerObject : MonoBehaviour {
	//Animation Stuff
	Animator anim;
	public float move_FSM_val;
	public bool facingRight;
	private SpriteRenderer sRenderer;
	
	public int playerNum;
	
	public Vector3 startPosition;
	public Vector3 velocity;
	
	public bool playing;
	
	public List<Collider> groundList;
	public List<Collider> leftWallList;
	public List<Collider> rightWallList;
	
	public GameObject bullet;
	
	bool ________________________________________;
	
	private Vector3 spawnPos;
	private Vector3 forcedVelocity;
	private int forcedVelFrameCounter;
	private float forcedVelAbsMax;
	private Vector3 controlledVelocity;
	private static Vector3 playerGravity = new Vector3(0.0f, -35.0f, 0.0f);
	
	Vector3	curGroundRightCornerPos;
	Vector3 curGroundLeftCornerPos;
	Vector3 prevGroundLeftCornerPos;
	Vector3 prevGroundRightCornerPos;
	Vector3	curTopRightCornerPos;
	Vector3 curTopLeftCornerPos;
	Vector3 prevTopLeftCornerPos;
	Vector3 prevTopRightCornerPos;
	
	public Vector3 aimVector;
	
	bool jumpQueued;
	bool jumpEnded;
	
	private int lives;
	
	//SCOTT AND MATT SHOOTING STUFF
	public GameObject leftBullet;
	public GameObject rightBullet;
	public GameObject jumpBullet;
	private GameObject currentBullet;
	int facing = 0;
	
	private InputDevice inputDevice;
	
	private bool invincible;
	private int invincibleCounter;
	private int deadCounter;
	
	// Use this for initialization
	void Start () {
		spawnPos = this.transform.position;
		
		print (InputManager.Devices.Count);
		if(inputDevice == null){
			inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		}
		if(inputDevice == null){
			//	print ("manager m: " + Manager.m);
			Manager.m.removePlayerGUIs(playerNum);
			Destroy(this.gameObject);
		}
		
		RestoreDefaults ();
		playing = true;
		spawnPos = this.transform.position;
		forcedVelocity = Vector3.zero;
		forcedVelAbsMax = 11.0f;
		controlledVelocity = Vector3.zero;
		
		forcedVelFrameCounter = 0;
		aimVector.x = 0f;
		aimVector.y = 1f;
		aimVector.z = 0f;
		
		jumpQueued = false;
		jumpEnded = true;
		
		sRenderer = this.GetComponent<SpriteRenderer> ();
		
		invincibleCounter = 50;
		StartCoroutine(setInvincibleTime (1f));
		StartCoroutine(setFlashingTime(1f));
		
		lives = 3;
		
		grounded = true;
		isJumping = false;
		totalForce = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(Manager.m.canRun){
			if (inputDevice.LeftBumper.IsPressed) {
				jumpQueued = true;
			} else {
				jumpQueued = false;
			}
			
			//Get aim vector
			Vector2 rsv = inputDevice.RightStick;
			if (rsv.magnitude <= 0.4f) {
				rsv = aimVector;
			}
			
			aimVector.x = rsv.x;
			aimVector.y = rsv.y;
			aimVector.z = 0.0f;
			
			aimVector.Normalize();
			
			aimVector = aimVector * 10.0f;
			
			Vector3 fireFromPos = this.transform.position;
			fireFromPos.y += 0.2f;
			
			//SCOTT AND MATT
			if(inputDevice.LeftTrigger.WasPressed){
				currentBullet = Instantiate (bullet) as GameObject;
				BulletScript obj = currentBullet.GetComponent<BulletScript>();
				
				obj.setPlayerRef(this);
				obj.setPull();
				
				//canFire = false;
				
				obj.setPosition(fireFromPos);
				obj.setVelocity(aimVector.normalized * 30f);
			}
			else if(inputDevice.RightTrigger.WasPressed){
				currentBullet = Instantiate (bullet) as GameObject;
				BulletScript obj = currentBullet.GetComponent<BulletScript>();
				
				//	print (obj);
				
				obj.setPlayerRef(this);
				obj.setPush();
				
				obj.setPosition(fireFromPos);
				obj.setVelocity(aimVector.normalized * 30f);
			}
			else if(inputDevice.RightBumper.WasPressed){
				currentBullet = Instantiate (bullet) as GameObject;
				BulletScript obj = currentBullet.GetComponent<BulletScript>();
				
				//	print (obj);
				
				obj.setPlayerRef(this);
				obj.setJump();
				
				obj.setPosition(fireFromPos);
				obj.setVelocity(aimVector.normalized * 30f);
			}
		}
	}
	
	
	
	bool grounded;
	bool isJumping;
	Vector3 totalForce;
	void FixedUpdate(){
		if(playing && Manager.m.canRun){
			totalForce = Vector3.zero;
			//this.rigidbody.velocity = totalForce;
			//totalForce += new Vector3(inputDevice.LeftStickX.Value * 15.0f, 0, 0);
			
			Vector3 pos = new Vector3(this.transform.position.x + (inputDevice.LeftStickX.Value * 0.19f), 
			                          this.transform.position.y + (inputDevice.LeftStickY.Value * 0.19f),
			                          this.transform.position.z);
			this.rigidbody.MovePosition(pos);
			
			if(jumpQueued && !isJumping && grounded){
				//	print ("jumping");
				//this.rigidbody.AddForce(new Vector3(0, 250.0f,0));
				totalForce += new Vector3(0, 250.0f, 0);
				grounded = false;
				isJumping = true;
				jumpQueued = false;
			}
			
			else if(jumpQueued && isJumping){
				//	print ("double jump");
				//this.rigidbody.AddForce(new Vector3(0, 190.0f, 0));
				totalForce += new Vector3(0, 190.0f, 0);
				isJumping = false;
				jumpQueued = false;
			}
			
			/*	print ("y"+this.rigidbody.velocity.y);
			print("jumping: " + isJumping);
			print ("grounded: " + grounded);
			if(this.rigidbody.velocity.y == 0 && !isJumping && !grounded){
				//print ("resetting");
				grounded = true;
				isJumping = false;
				jumpQueued = false;
			}
*/
			//		this.rigidbody.AddForce(totalForce);
			
			//print (this.rigidbody.velocity.y);
			
			
			
			/*//Handle the x movement
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
				//	float jumpSpeed = 18.0f;
				//	controlledVelocity.y += jumpSpeed;
				
				this.rigidbody.AddForce(new Vector3(0,18.0f,0));
				
			} else if (!jumpQueued && controlledVelocity.y > 5.0f){
				//This is to have variable jump height if the jump button is held
				//controlledVelocity.y = 5.0f;
				
				this.rigidbody.AddForce(new Vector3(0,5.0f,0));
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
			//this.rigidbody.AddForce(new Vector3(0,4.0f,0));	
			//Move the player
			pos += velocity * Time.deltaTime;
			this.transform.position = pos;
			
			*/
		}
		
		prevGroundLeftCornerPos = curGroundLeftCornerPos;
		prevGroundRightCornerPos = curGroundRightCornerPos;
		curGroundLeftCornerPos = getGroundLeftCorner ();
		curGroundRightCornerPos = getGroundRightCorner ();
		prevTopLeftCornerPos = curTopLeftCornerPos;
		prevTopRightCornerPos = curTopRightCornerPos;
		curTopLeftCornerPos = getTopLeftCorner ();
		curTopRightCornerPos = getTopRightCorner ();
	}
	
	public void resetGround(){
		groundList.Clear ();
		leftWallList.Clear ();
		rightWallList.Clear ();
	}
	
	void HandleTunnelCollision(Collider other){
		TunnelScript ts = other.gameObject.GetComponent<TunnelScript> ();
		if(ts){
			ts.MovePlayer(this.collider);
		}
	}
	
	void OnTriggerEnter(Collider other){
		HandleTunnelCollision (other);
		HandleBulletCollision (other);
		HandleSpikeCollision (other);
	}
	
	void OnCollisionEnter(Collision other){
		
		HandleBulletCollision (other.collider);
		HandleSpikeCollision (other.collider);
		
		if(other.collider.gameObject.GetComponent<GroundObject>()){
			Vector3 vel = new Vector3 (0, 0, 0);
			this.rigidbody.velocity = vel;
		}
	}
	
	public bool beingFlung;
	void OnCollisionStay(Collision collide){
		
		//	print ("collided with: " + collide.collider.gameObject.name);
		if(collide.collider.gameObject.GetComponent<BulletScript>()){
			//		print ("Collide with bullet! ");
		}
		else if(!beingFlung){
			//		print (beingFlung);
			//		print ("staying on wall and setting to 0");
			Vector3 vel = new Vector3 (0, 0, 0);
			this.rigidbody.velocity = vel;
			//		print (this.rigidbody.velocity);
		}
	}
	
	void OnCollisionExit(Collision collide){
		if(collide.collider.gameObject.GetComponent<BulletScript>()){
			
		}
		else if(collide.collider.gameObject.GetComponent<GroundObject>() && !beingFlung){
			//		print ("setting to 0");
			Vector3 vel = new Vector3 (0, 0, 0);
			this.rigidbody.velocity = vel;
		}
	}
	
	public void HitGround(){
		Vector3 groundForce = Vector3.zero;
		groundForce.y = -velocity.y;
		velocity.y = 0.0f;
		controlledVelocity.y = 0.0f;
		forcedVelocity.y = 0.0f;
		
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
	IEnumerator HitByThrowBullet(){
		SpriteRenderer sr = this.GetComponent<SpriteRenderer> ();
		switch(playerNum){
		case 0:
			sr.color = Color.grey;
			break;
		case 1:
			sr.color = Color.cyan;
			break;
		case 2:
			sr.color = Color.green;
			break;
		case 3:
			sr.color = Color.red;
			break;
		}
		
		yield return new WaitForSeconds (0.5f);
		
		//color to switch back to.  keep it at white
		sr.color = Color.white;
		
		
	}
	
	void HandleBulletCollision(Collider other){
		BulletScript bs = other.GetComponent<BulletScript> ();
		if(bs && (bs.getPlayerRef() != this)){
			bulletType bType = bs.getBulletType();
			
			//If you're invincible, destroy the object and don't move
			if(invincible){
				Destroy(other.gameObject);
				return;
			}
			
			float pushPullScaling = 7.0f;
			
			if(bType == bulletType.PUSH){
				Vector3 diff = transform.position - bs.getPlayerRef().transform.position;
				diff = diff.normalized * pushPullScaling;
				forcedVelocity += new Vector3(diff.x, diff.y * 4, diff.z);
				Destroy(other.gameObject);
			}
			else if(bType == bulletType.PULL){
				Vector3 diff = bs.getPlayerRef().transform.position - transform.position;
				diff = diff.normalized * pushPullScaling;
				forcedVelocity += new Vector3(diff.x, diff.y * 6 + -(playerGravity.y), diff.z);
				Destroy(other.gameObject);	
			}
			else if(bType == bulletType.JUMP){
				if(!beingFlung){
					//			print ("should be flung");
					StartCoroutine(HitByThrowBullet());
					this.GetThrown(bs.getPlayerRef());
					Destroy(other.gameObject);
				}
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
			if(groundList.Count != 0 && forcedVelocity.y < 0){
				forcedVelocity.y = 0.0f;
			}
			if(rightWallList.Count != 0 && forcedVelocity.x > 0){
				forcedVelocity.x = 0.0f;
			}
			if(leftWallList.Count != 0 && forcedVelocity.x < 0){
				forcedVelocity.x = 0.0f;
			}
		}
	}
	
	void HandleSpikeCollision(Collider other){
		if (other.gameObject.tag == "spike") {
			
			//If you're invincible, dont die!
			if(invincible){
				return;
			}
			
			lives--;
			//GameObject p1Lives = GameObject.Find("p1Lives");
			//GUIText t = p1Lives.GetComponent<GUIText>();
			//t.text = "P" + playerNum + " Lives: " + lives;
			//Die ();
			Manager.m.setPlayerLives(playerNum, lives);
			
			if(lives <= 0){
				
			} else {
				Die();
			}
		} else {
			return;
		}
	}
	
	void Die(){
		//Run Death Animation
		
		//deadCounter = 50;
		//this.transform.position = spawnPos;
		//RestoreDefaults ();
		Respawn ();
	}
	
	void Respawn(){
		this.transform.position = spawnPos;
		this.transform.position = Manager.m.getRespawnLocation ();
		RestoreDefaults ();
		StartCoroutine(setInvincibleTime (2f));
		StartCoroutine(setFlashingTime(2f));
	}
	
	//ACTIONS
	public void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
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
	}
	
	IEnumerator setInvincibleTime(float seconds){
		invincible = true;
		yield return new WaitForSeconds(seconds);
		invincible = false;
	}
	
	IEnumerator setFlashingTime(float seconds){
		int iterations = Mathf.RoundToInt(seconds / 0.25f);
		
		Color normalColor = sRenderer.material.color;
		Color fadedColor = normalColor;
		fadedColor.a = 0.4f;
		
		for(int i = 0; i < iterations; i++){
			sRenderer.material.color = fadedColor;
			yield return new WaitForSeconds(0.125f);
			sRenderer.material.color = normalColor;
			yield return new WaitForSeconds(0.125f);
		}
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
	
	
	//GRAPPLE CODE
	static private float throwAimDuration = 0.5f;
	public float throwStrength = 20f;
	
	
	private void GetThrown(PlayerObject thrower){
		//movementDisabled = true;
		
		//This will be "struggle time"
		//yield return new WaitForSeconds(0.5f);
		StartCoroutine(waitForThrow (thrower));
	}
	
	IEnumerator ResetFlung(){
		yield return new WaitForSeconds (1.0f);
		beingFlung = false;
	}
	IEnumerator waitForThrow(PlayerObject thrower){
		//aimLine.Disable ();
		//throwLine.setThrowerRef (thrower);
		beingFlung = true;
		yield return new WaitForSeconds(throwAimDuration);
		Vector2 throwerAimVec = thrower.aimVector.normalized;
		Vector3 throwVector = Vector3.zero;
		throwVector.x = throwerAimVec.x;
		throwVector.y = throwerAimVec.y;
		throwVector.z = 0;
		this.forcedVelocity += throwStrength * throwVector;
		
		this.rigidbody.AddForce (throwStrength * throwVector * 30.0f);
		//	print ("was flung with force");
		//	print (throwStrength * throwVector * 30.0f);
		StartCoroutine (ResetFlung ());
		//movementDisabled = false;
		//throwLine.removeThrowerRef ();
		//aimLine.Enable ();
	}
	/*
	public void throwPlayer(PlayerObject thrown){
		StartCoroutine(whileThrowing(thrown));
	}
	
	IEnumerator whileThrowing(PlayerObject thrown){
		print (this.gameObject + " is throwing");
		aimLine.Disable ();
		//movementDisabled = true;
		yield return new WaitForSeconds(throwAimDuration);
		//movementDisabled = false;
		aimLine.Enable ();
	}


	void OnTriggerEnter(Collider other){
		SN_grappleController grapple = other.GetComponent<SN_grappleController> ();
		if(!grapple){
			return;
		}
		
		if(grapple.getPlayerRef() != this){
			GetThrown(grapple.getPlayerRef());
			grapple.informShooterOnThrowBegin(this);
		}
	}
*/
}