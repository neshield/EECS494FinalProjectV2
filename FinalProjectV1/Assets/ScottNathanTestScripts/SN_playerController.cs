using UnityEngine;
using System.Collections;
using InControl;


public class SN_playerController : PlayerBaseClass {
	static private float throwAimDuration = 0.5f;

	private Vector3 spawnPos;

	//private Vector3 xMovement = new Vector3;
	public int playerNum;
	private InputDevice inputDevice;
	private float playerMaxSpeed = 7.5f;

	public Vector3 aimVector;
	private float sightLength = 5.0f;

	private bool movementDisabled;

	private GameObject currentBullet;

	public GameObject grappleBullet;

	public Vector3 forcedVelocity;
	private int forcedVelFrameCounter;
	private float forcedVelAbsMax;
	public Vector3 controlledVelocity;
	private Vector2 blinkVector;
	private float blinkScalar = 2.5f;
	private bool fast = false;

	public Vector3 rBodyVel;

	private ThrowLineScript throwLine;
	private SN_aimController aimLine;

	public int points;

	// Use this for initialization
	void Start () {
		print (InputManager.Devices.Count);
		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		print ("Assigning input device... player num: " + playerNum);
		spawnPos = this.transform.position;

		controlledVelocity = Vector3.zero;
		forcedVelocity = Vector3.zero;
		forcedVelAbsMax = 110.0f;

		aimVector.x = 0f;
		aimVector.y = 0f;
		aimVector.z = 0f;

		points = 0;

		movementDisabled = false;

		foreach(Transform child in transform){
			if(throwLine == null){
				throwLine = child.GetComponent<ThrowLineScript>();
			}
			if(aimLine == null){
				aimLine = child.GetComponent<SN_aimController>();
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rBodyVel = this.rigidbody.velocity;
		//GetAiming
		if(inputDevice!= null){
			Vector2 rsv = inputDevice.RightStick;
			if (rsv.magnitude <= 0.4f) {
				rsv = aimVector;
			}
			
			aimVector.x = rsv.x;
			aimVector.y = rsv.y;
			aimVector.z = 0.0f;
			
			aimVector.Normalize();
			
			aimVector = aimVector * sightLength;

			//GetMovement
			Vector2 lsv = inputDevice.LeftStick;
			blinkVector = inputDevice.LeftStick;
			blinkVector.Normalize();
			lsv = lsv * playerMaxSpeed;
			if(fast){
				controlledVelocity.x = 1.75f * lsv.x;
				controlledVelocity.y = 1.75f * lsv.y;
				controlledVelocity.z = 0f;
			}
			else{
				controlledVelocity.x = lsv.x;
				controlledVelocity.y = lsv.y;
				controlledVelocity.z = 0f;
			}

			//Blink, obviously needs a time restriction between uses
			if (inputDevice.Action1.WasPressed) {
				Vector3 blinkV3;
				blinkV3.x = blinkScalar  * blinkVector.x;
				blinkV3.y = blinkScalar * blinkVector.y;
				blinkV3.z = 0f;
				
				Vector3 newPos = this.transform.position + blinkV3;
				this.transform.position = newPos;
			}
			
			//SuperSpeed, obviously needs a limit also
			if (inputDevice.Action2.WasPressed) {
				StartCoroutine(setSpeedTime(1.25f));		
			}
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

		if(movementDisabled){
			controlledVelocity = Vector3.zero;
		}

		this.transform.position += (controlledVelocity + forcedVelocity) * Time.fixedDeltaTime;
	}

	void Update(){
		if (inputDevice == null) {
						return;
				}
		if (inputDevice.RightTrigger.WasPressed) {
			currentBullet = Instantiate (grappleBullet) as GameObject;

			SN_grappleController obj = currentBullet.GetComponent<SN_grappleController>();
			
			obj.setShooterRef(this);
			//canFire = false;
			
			obj.setPosition(this.transform.position);
			obj.setVelocity(aimVector * 4.0f);
		}
	}

	private void GetThrown(SN_playerController thrower){
		//movementDisabled = true;

		//This will be "struggle time"
		//yield return new WaitForSeconds(0.5f);
		StartCoroutine(waitForThrow (thrower));
	}

	IEnumerator waitForThrow(SN_playerController thrower){
		aimLine.Disable ();
		throwLine.setThrowerRef (thrower);
		yield return new WaitForSeconds(throwAimDuration);
		this.forcedVelocity += 4f * thrower.aimVector;
		//movementDisabled = false;
		throwLine.removeThrowerRef ();
		aimLine.Enable ();
	}

	public void throwPlayer(SN_playerController thrown){
		StartCoroutine(whileThrowing(thrown));
	}

	IEnumerator whileThrowing(SN_playerController thrown){
		print (this.gameObject + " is throwing");
		aimLine.Disable ();
		//movementDisabled = true;
		yield return new WaitForSeconds(throwAimDuration);
		//movementDisabled = false;
		aimLine.Enable ();
	}

	//void OnCollisionEnter(Collision otherC){
	//	Collider other = otherC.collider;
	void OnTriggerStay(Collider other){
		HillScript hs = other.GetComponent<HillScript> ();
		if(hs){
			points++;
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "spike") {
			this.transform.position = spawnPos;
			this.rigidbody.velocity = new Vector3(0,0,0);
			this.forcedVelocity = Vector3.zero;
			this.controlledVelocity = Vector3.zero;
			return;
		}

		SN_grappleController grapple = other.GetComponent<SN_grappleController> ();
		print ("Player is colliding with: " + grapple);
		if(!grapple || grapple.getPlayerRef() == this){
			return;
		}

		if(grapple.getPlayerRef() != this){
			GetThrown(grapple.getPlayerRef());
			grapple.informShooterOnThrowBegin(this);
		}
	}

	IEnumerator setSpeedTime(float seconds){
		fast = true;
		yield return new WaitForSeconds(seconds);
		fast = false;
	}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.layer == 11) {
			this.rigidbody.velocity = new Vector3(0,0,0);		
		}
	}

}
