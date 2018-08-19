using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathrequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
	PathRequest currentPathRequest;

	Queue<RequestedPath> requestPathQueue = new Queue<RequestedPath> ();
	RequestedPath request;

	static PathrequestManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;

	void Awake(){
		instance = this;
		instance.pathfinding = GetComponent<Pathfinding> ();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, bool useWeights){
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback, useWeights);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	public static void NewPaht(Vector3 start, Vector3 end){
		RequestedPath newRequest = new RequestedPath (start, end);
		instance.requestPathQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}


	void TryProcessNext(){
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessingPath = true;
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.useWeights);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success){
		currentPathRequest.callback (path, success);
		isProcessingPath = false;
		TryProcessNext ();
	}

	struct PathRequest{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;
		public bool useWeights;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback, bool _useWeights){
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			useWeights = _useWeights;
		}
	}

	struct RequestedPath{
		public Vector3 pathStart;
		public Vector3 pathEnd;


		public RequestedPath(Vector3 _start, Vector3 _end){
			pathStart = _start;
			pathEnd = _end;

		}
	}
}
