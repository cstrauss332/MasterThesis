using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

	PathrequestManager requestManager;
	InfluencMap grid;

	void Awake(){
		requestManager = GetComponent<PathrequestManager> ();
		grid = GetComponent<InfluencMap> ();
	}
		

	public void StartFindPath(Vector3 startPos, Vector3 targetPos, bool useWeights){
		FindPath(startPos, targetPos, useWeights);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos, bool useWeights){

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closeSet = new HashSet<Node> ();

			openSet.Add (startNode);

			while (openSet.Count >= 0) {
				Node currentNode = openSet.RemoveFirst ();
				closeSet.Add (currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closeSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + (neighbour.Weight * Convert.ToInt32(useWeights));
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						} else {
							openSet.UpdateItem (neighbour);
						}
					}
				}
			}
		}

		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
	}

	Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node> ();

		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints;

		//Simplyfied
		//waypoints = SimplifyPath (path);
		//Array.Reverse(waypoints);

		//Normal
		path.Reverse();
		waypoints = TransformToVector3 (path);

		return waypoints;	
	}

	Vector3[] TransformToVector3(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();

		for (int i = 1; i < path.Count; i++) {
			waypoints.Add (path [i].worldPosition);
		}
		return waypoints.ToArray();
	}

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);

			if (directionNew != directionOld) {
				waypoints.Add (path [i].worldPosition);
			}

			directionOld = directionNew;
		}
		return waypoints.ToArray ();
	}

	int GetDistance(Node nodeA, Node nodeB){
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}
}
