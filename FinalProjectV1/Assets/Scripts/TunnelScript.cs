using UnityEngine;
using System.Collections;

public enum TunnelType {vertical, horizontal, top, bottom, left, right};

public class TunnelScript : MonoBehaviour {
	static bool beenThroughTunnel;
	public TunnelScript pairedTunnel;
	public Vector3 spawnLocation;
	public TunnelType tType;

	// Use this for initialization
	void Start () {
		beenThroughTunnel = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		PlayerObject pon = other.GetComponent<PlayerObject> ();
		BulletScript bs = other.GetComponent<BulletScript> ();

		if(!pon && !bs){
			return;
		}

		//remove it's current grounds/walls
		if(pon){
			pon.resetGround();
		}

		//position of the object going through the tunnel
		Vector3 pos = other.transform.position;

		BoxCollider bc = other.GetComponent<BoxCollider>();

		//if it uses a box collider
		if(bc){
			float buffer = 0.15f;
			float newX = bc.transform.position.x;
			float newY = 0.0f;

			//then the paired tunnel is to my left and the spawn location should
			//be to the right of that one
			if(pairedTunnel.tType == TunnelType.right){
				newX = (pairedTunnel.transform.position.x + (pairedTunnel.transform.localScale.x / 2))
					+ ((other.transform.localScale.x * bc.size.x) / 2) + buffer;
				//print ("player should be moved to the right of the paired tunnel at x: " + newX);

				//need to figure out the new y.  the paire tunnel may be on a different plane than this one.
				float pairedTop = pairedTunnel.transform.position.y + (pairedTunnel.transform.localScale.y / 2);
				//calculates this top and subtracts the position
				float diffFromTop = (this.transform.position.y + (this.transform.localScale.y / 2)) - other.transform.position.y;
				newY = pairedTop - diffFromTop;

				pairedTunnel.setSpawnLocation(new Vector3(newX, newY + 0.045f, pairedTunnel.spawnLocation.z));

			}
			//otherwise it goes to the left
			else if(pairedTunnel.tType == TunnelType.left){
				newX = (pairedTunnel.transform.position.x - (pairedTunnel.transform.localScale.x / 2))
					- ((other.transform.localScale.x * bc.size.x) / 2) - buffer;
			/*	print ("player should be moved to the left of the paired tunnel at x: " + newX);
				print ("\tpos.x: " + pairedTunnel.transform.position.x);
				print ("\tPaired local scale y: " + pairedTunnel.transform.localScale.y);
				print ("\tPaired local scale x: " + pairedTunnel.transform.localScale.x);
				print ("\tObj Local scale x: " + other.transform.localScale.x);
				print ("\tObj box collider size.x: " + bc.size.x);
				print ("\tBuffer: " + buffer);
				*/
				float pairedTop = pairedTunnel.transform.position.y + (pairedTunnel.transform.localScale.y / 2);
				float diffFromTop = (this.transform.position.y + (this.transform.localScale.y / 2)) - other.transform.position.y;
				newY = pairedTop - diffFromTop;

				pairedTunnel.setSpawnLocation(new Vector3(newX, newY + 0.045f, pairedTunnel.spawnLocation.z));
			}
			else if(pairedTunnel.tType == TunnelType.bottom){
				newY = (pairedTunnel.transform.position.y - (pairedTunnel.transform.localScale.y / 2)) -
					(other.transform.localScale.y / 2) - buffer;

				float pairedLeft = pairedTunnel.transform.position.x - (pairedTunnel.transform.localScale.x / 2);
				float diffFromLeft = other.transform.position.x - (this.transform.position.x - (this.transform.localScale.x / 2));
				newX = pairedLeft + diffFromLeft;

				pairedTunnel.setSpawnLocation(new Vector3(newX, newY, pairedTunnel.spawnLocation.z));

				print ("setting spawn pos at " + pairedTunnel.spawnLocation);
			}

			else if(pairedTunnel.tType == TunnelType.top){
				newY = (pairedTunnel.transform.position.y + (pairedTunnel.transform.localScale.y / 2)) +
					(other.transform.localScale.y / 2) + buffer;

				float pairedLeft = pairedTunnel.transform.position.x - (pairedTunnel.transform.localScale.x / 2);
				float diffFromLeft = other.transform.position.x - (this.transform.position.x - (this.transform.localScale.x / 2));
				newX = pairedLeft + diffFromLeft;

				pairedTunnel.setSpawnLocation(new Vector3(newX, newY, pairedTunnel.spawnLocation.z));
			}
			else{
				print ("this isn't supposed to run");
			}


			
		}
	

		/*
		//if it's a vertical then the x direction matters
		else{
			float leftSide = this.transform.position.x - (this.transform.localScale.x / 2);
			//float diff = pon.transform.position.x - Mathf.Abs(leftSide);
			float diff = pos.x - Mathf.Abs(leftSide);
			float otherLeft = pairedTunnel.transform.position.x - (pairedTunnel.transform.localScale.x / 2);

			float newX = otherLeft + diff;


			pairedTunnel.setSpawnLocation(
				new Vector3(newX, 
			            pairedTunnel.spawnLocation.y, 
			            pairedTunnel.spawnLocation.z)
				);
		}
		*/
		other.transform.position = pairedTunnel.spawnLocation;
		
	}

	void setSpawnLocation(Vector3 location_){
		spawnLocation = location_;
	}
}
