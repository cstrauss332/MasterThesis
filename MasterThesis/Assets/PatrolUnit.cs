using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolUnit : MonoBehaviour {

	InfluencMap map;

	public float angle = 0.5f;
	public int dangerValueYellow = 6;

	List<Node> changedValues = new List<Node>();
	SphereCollider sphere;


	void Start(){
		map = GameObject.Find ("A*").gameObject.GetComponent<InfluencMap> ();
		sphere = GetComponent<SphereCollider> ();

	}
		
	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {
			
			Vector3 direction = (other.gameObject.transform.position - this.transform.position).normalized;
			float dot = Vector3.Dot (direction, this.transform.forward);

			if (dot > angle) {
				
				RaycastHit hit;

				if (Physics.Raycast (this.transform.position, direction, out hit)) {
					if (hit.collider.gameObject == other.gameObject) {
						Debug.Log ("Found");
					}
				}
			}
		}	
	}

	void LateUpdate(){
		CleanMapFromMyWheights ();
		FillInfluence (this.transform.position, 1, dangerValueYellow);
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

				if (ChangeInfluencMapValues (n)) {
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
		
	bool ChangeInfluencMapValues(Node nodeToChange){
		
		if (IsThereAStraightWayBack (nodeToChange)) {

			Vector3 direction = (nodeToChange.worldPosition - this.transform.position).normalized;
			float dot = Vector3.Dot (direction, this.transform.forward);

			if (dot > angle) {
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
