using UnityEngine;
using System.Collections;
using InControl;


public class SN_playerController : MonoBehaviour {

	//private Vector3 xMovement = new Vector3;
	public int playerNum;
	private InputDevice inputDevice;
	private float playerMaxSpeed = 5f;

	public Vector3 aimVector;
	private float sightLength = 4.0f;

	private bool movementDisabled;

	private GameObject currentBullet;

	public GameObject grappleBullet;

	private Vector3 forcedVelocity;
	private int forcedVelFrameCounter;
	private float forcedVelAbsMax;
	private Vector3 controlledVelocity;

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
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
			lsv = lsv * playerMaxSpeed;
			controlledVelocity.x = lsv.x;
			controlledVelocity.y = lsv.y;
			controlledVelocity.z = 0f;
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
		movementDisabled = true;

		//This will be "struggle time"
		//yield return new WaitForSeconds(0.5f);
		StartCoroutine(waitForThrow (0.5f, thrower));
	}

	IEnumerator waitForThrow(float seconds, SN_playerController thrower){
		yield return new WaitForSeconds(seconds);
		this.forcedVelocity += 5f * thrower.aimVector;
		print ("Getting thrown with vel: " + this.forcedVelocity);
		movementDisabled = false;
	}

	void OnTriggerEnter(Collider other){
		SN_grappleController grapple = other.GetComponent<SN_grappleController> ();
		if(!grapple){
			return;
		}

		print ("Entering trigger... playerRef = " + grapple.getPlayerRef());

		if(grapple.getPlayerRef() != this){
			GetThrown(grapple.getPlayerRef());
		}
	}
}
