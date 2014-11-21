using UnityEngine;
using System.Collections;
using InControl;


public class SN_playerController : PlayerBaseClass {
	static private float throwAimDuration = 0.5f;


	//private Vector3 xMovement = new Vector3;
	public int playerNum;
	private InputDevice inputDevice;
	private float playerMaxSpeed = 5f;

	public Vector3 aimVector;
	private float sightLength = 4.0f;

	private bool movementDisabled;

	private GameObject currentBullet;

	public GameObject grappleBullet;

	public Vector3 forcedVelocity;
	private int forcedVelFrameCounter;
	private float forcedVelAbsMax;
	public Vector3 controlledVelocity;
	private Vector2 blinkVector;
	private int blinkScalar = 1;
	private bool fast = false;

	public Vector3 rBodyVel;

	private ThrowLineScript throwLine;
	private SN_aimController aimLine;

	// Use this for initialization
	void Start () {
		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;

		controlledVelocity = Vector3.zero;
		forcedVelocity = Vector3.zero;
		forcedVelAbsMax = 110.0f;

		aimVector.x = 0f;
		aimVector.y = 0f;
		aimVector.z = 0f;

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
				controlledVelocity.x = 2 * lsv.x;
				controlledVelocity.y = 2 * lsv.y;
				controlledVelocity.z = 0f;
			}
			else{
				controlledVelocity.x = lsv.x;
				controlledVelocity.y = lsv.y;
				controlledVelocity.z = 0f;
			}
		}

		//Blink, obviously needs a time restriction between uses
		if (inputDevice.Action1.WasPressed) {
			this.transform.position.x += blinkScalar * blinkVector.x;
			this.transform.position.y += blinkScalar * blinkVector.y;
		}

		//SuperSpeed, obviously needs a limit also
		if (inputDevice.Action2.WasPressed) {
			StartCoroutine(setSpeedTime(3f));		
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
			obj.setVelocity(aimVector * 3.0f);
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

	IEnumerator setSpeedTime(float seconds){
		fast = true;
		yield return new WaitForSeconds(seconds);
		fast = false;
	}
	
}
