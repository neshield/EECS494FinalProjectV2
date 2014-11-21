using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillScript : MonoBehaviour {
	List<int> playerScores = new List<int>();
	List<int> playerScoreTicks = new List<int>();
	List<PlayerBaseClass> playerObjectList = new List<PlayerBaseClass> ();

	//Singleton
	public static HillScript S;

	List<Vector3> hillLocations = new List<Vector3>();

	private int curPos;


	// Use this for initialization
	void Start () {
		S = this;
		hillLocations.Add (new Vector3 (0f, 4.5f, 0.5f));
		hillLocations.Add (new Vector3 (0f, -4.5f, 0.5f));
		hillLocations.Add (new Vector3 (-10f, -4.75f, 0.5f));
		hillLocations.Add (new Vector3 (10f, 4.75f, 0.5f));
		curPos = -1;

		StartCoroutine(LocationMover ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator LocationMover(){
		while(true){
			yield return new WaitForSeconds(5f);
			print ("moving hill...");
			int newPos = curPos;
			while(newPos == curPos){
				newPos = Random.Range(0, 3);
			}
			curPos = newPos;
			this.transform.position = hillLocations[newPos];
		}
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
