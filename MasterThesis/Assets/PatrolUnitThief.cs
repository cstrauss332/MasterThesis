using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolUnitThief : MonoBehaviour {

	InfluencMap map;

	public int dangerValueYellow = 6;

	public float angleOne = 0.5f;
	public float angleTwo = 0.3f;
	public float angleThree = 0.1f;

	public float distanceTwo = 0.5f;
	public float distanceThree = 0.3f;

	public bool helpPointDangerValueTest = false;

	private float distanceValueTwo;
	private float distanceValueThree;

	public bool caught = false;
	private Vector3 caughtLocation = Vector3.zero;

	List<Node> changedValues = new List<Node>();
	SphereCollider sphere;

	int cornerMax = 100;

	void Start(){
		map = GameObject.Find ("A*").gameObject.GetComponent<InfluencMap> ();
		sphere = GetComponent<SphereCollider> ();

		distanceValueTwo = sphere.radius * distanceTwo;
		distanceValueThree = sphere.radius * distanceThree;

	}

	void Update() {
		if (helpPointDangerValueTest) {
			//CornerWeight
			List<GameObject> cornersinReach = CheckForCorners (this.transform.position, sphere.radius, "Corner");

			foreach (GameObject other in cornersinReach) {
				//Yellow
				other.GetComponent<CornerData> ().SetWeight (this, "Yellow");

				Vector3 direction = (other.gameObject.transform.position - this.transform.position).normalized;
				float dot = Vector3.Dot (direction, this.transform.forward);

				if ((dot > angleThree && Vector3.Distance (this.gameObject.transform.position, other.gameObject.transform.position) < distanceValueThree) ||
				   (dot > angleTwo && Vector3.Distance (this.gameObject.transform.position, other.gameObject.transform.position) < distanceValueTwo) ||
				   (dot > angleOne)) {

					//Red
					other.GetComponent<CornerData> ().SetWeight (this, "Red");
				}
			}
		}

	}

	public List<GameObject> CheckForCorners(Vector3 centerPoint, float radius, string layerName)
	{
		Collider[] colliders = new Collider[cornerMax];
		Physics.OverlapSphereNonAlloc(centerPoint, radius, colliders, (1 << LayerMask.NameToLayer(layerName)));

		List<GameObject> retCorners = new List<GameObject>();

		foreach (Collider col in colliders)
		{
			if (col == null)
				break;

			retCorners.Add(col.gameObject);
		}

		return retCorners;
	}
		

	void OnTriggerStay(Collider other) {
		Vector3 direction;
		float dot;

		if (other.gameObject.tag == "Player") {

			direction = (other.gameObject.transform.position - this.transform.position).normalized;
			dot = Vector3.Dot (direction, this.transform.forward);

			if ( (dot > angleThree && Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position) < distanceValueThree) ||
				(dot > angleTwo && Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position) < distanceValueTwo) ||
				 (dot > angleOne) 
				)
			{
				RaycastHit hit;

				if (Physics.Raycast (this.transform.position, direction, out hit)) {
					if (hit.collider.gameObject == other.gameObject) {
						caught = true;
						caughtLocation = other.gameObject.transform.position;
					}
				}
			}
		}	
	}

	void LateUpdate(){
		CleanMapFromMyWheights ();
		FillInfluence (this.transform.position, 1, dangerValueYellow);
	}

	public bool TargetCaught {
		get {
			return caught;
		}
		set{
			caught = value;
		}
	}

	public Vector3 LocationWhereTargetCaught {
		get {
			return this.caughtLocation;
		}
	}



	/// <summary>
	/// F///	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="oldValue">Old value.</param>
	/// <param name="newValue">New value.</param>
	void FillInfluence(Vector3 position, int oldValue, int newValue){

		Stack<Node> stack = new Stack<Node>();
		Node currentNode = map.NodeFromWorldPoint (position);

		stack.Push (currentNode);

		while (!(stack.Count == 0)) {
			Node n = stack.Pop();

			if (n.Weight == 1 && n.walkable) {

				if (ChangeInfluencMapValuesThief (n)) {
					List<Node> nodeList = new List<Node> ();
					nodeList = map.GetNeighbours (n);

					foreach (Node node in nodeList) {
						if (Vector3.Distance (this.transform.position, node.worldPosition) < sphere.radius) {
							stack.Push (node);
						}
					}
				}
			}
		}
	}
		

	void CleanMapFromMyWheights(){
		foreach (Node n in changedValues) {
			n.Weight = 1;
			n.walkable = true;
		}

		changedValues.Clear ();
	}
		

	bool ChangeInfluencMapValuesThief(Node nodeToChange){

		if (IsThereAStraightWayBack (nodeToChange)) {

			Vector3 direction = (nodeToChange.worldPosition - this.transform.position).normalized;
			float dot = Vector3.Dot (direction, this.transform.forward);

			if (dot > angleThree && Vector3.Distance(this.gameObject.transform.position, nodeToChange.worldPosition) < distanceValueThree) {
				nodeToChange.walkable = false;
			} else if (dot > angleTwo && Vector3.Distance(this.gameObject.transform.position, nodeToChange.worldPosition) < distanceValueTwo) {
				nodeToChange.walkable = false;
			} else 	if (dot > angleOne) {
				nodeToChange.walkable = false;
			} else {
				nodeToChange.Weight = dangerValueYellow;
			}

			if (changedValues.IndexOf (nodeToChange) < 0) {
				changedValues.Add (nodeToChange);
			}

			return true;
		}
		return false;
	}

	bool IsThereAStraightWayBack(Node node){
		
		Vector3 direction = (this.transform.position - node.worldPosition).normalized;
		RaycastHit hit;


		if (Physics.Raycast (node.worldPosition, direction, out hit)) {
			if (hit.collider.gameObject == this.gameObject) {
				return true;
			}
		}
		return false;
	}
		
}
