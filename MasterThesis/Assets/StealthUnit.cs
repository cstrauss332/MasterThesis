using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StealthUnit : MonoBehaviour
{

    InfluencMap map;

	[SerializeField]
    public Transform target;

    public float speed = 0;
	private bool isFinished = true;

    Vector3[] path;
	Vector3[] wayToGoal;
	Vector3[] helpPath;

    float currentDangerValue;

    int targetIndex;
    bool useWeights = true;
    bool checkPaths = true;

    List<Vector3> wayPoints;
	List<Vector3[]> listPath;


    Vector3 currentWaypoint = Vector3.zero;

    [SerializeField]
    float rad = 20;
    [SerializeField]
    float weight = 1;
	[SerializeField]
	float dangerValueForEnemy = 50;
	[SerializeField]
	[Space]
	float enemyDetectionRadius = 10;
	[Space]
	[SerializeField]
	int lookAhead = 10;

    int cornerMax = 50;

    void Awake()
    {
        //PathrequestManager.RequestPath (transform.position, target.position, OnPathFound, useWeights);
        map = GameObject.Find("A*").gameObject.GetComponent<InfluencMap>();
        wayPoints = new List<Vector3>();
        listPath = new List<Vector3[]>();
        currentDangerValue = float.MaxValue;
        path = null;
		wayToGoal = null;
		isFinished = true;
		currentWaypoint = this.transform.position;
    }

    void Update()
    {
		//Debug.Log (isFinished);
		//Debug.Log(path);

		if (target != null) {	
			if (Vector3.Distance (this.transform.position, target.transform.position) <= 1.0f) {
				isFinished = true;
				target = null;
				path = null;
			}
		}

		if (target != null && path == null) {
			FindNextPoint ();
			targetIndex = 0;
		}else if (path != null) {
			if (transform.position == currentWaypoint) {
				targetIndex++;

				if (targetIndex >= path.Length) {
					path = null;
				} else {
					currentWaypoint = path [targetIndex];
				}
			}

			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
		}
		
			/*if (path != null && isFinished){
			isFinished = false;
			StopAllCoroutines ();
			StartCoroutine (FollowPath ());
		}

		/*if ((Vector3.Distance (this.transform.position, target.transform.position) > 1.0f) && target != null) {
			if (path == null) {
				PathrequestManager.RequestPath (transform.position, target.transform.position, GetBestPath, useWeights);
				if (wayToGoal != null) {
					Vector3[] helpPath = null;

					if (wayToGoal.Length < this.rad) {
						helpPath = wayToGoal;
					} else {
						int v = wayToGoal.Length - 1;
						if (v > lookAhead) {
							v = lookAhead;
						}

						wayPoints = CheckForCorners (wayToGoal [v], rad, "Corner");

						FindAllPaths ();

						currentDangerValue = float.MaxValue;

						foreach (Vector3[] currentPath in listPath) {
							float val = CalculatePathValue (currentPath);
							if (currentPath.Length > 0) {
								float dangerLevel = CalculateDangerLevel (currentPath, enemyDetectionRadius, "Enemy");
								val += dangerLevel;
							}

							if (val < currentDangerValue) {
								currentDangerValue = val;
								helpPath = currentPath;
							}
						}
					}

					path = helpPath;
					listPath.Clear ();

					if (path != null)
						StartCoroutine (FollowPath ());
				}
			}
		} else {
			isFinished = true;
		}*/
    }
		
	public void FindNextPoint()
	{
		PathrequestManager.RequestPath (transform.position, target.transform.position, GetBestPath, useWeights);
		if (wayToGoal != null) {
			Vector3[] helpPathPoint = null;

			wayPoints.Clear ();

			int v = wayToGoal.Length - 1;

			while (wayPoints.Count < 1) {// && wayPoints [0] != target.position) 
				//Debug.Log (wayPoints.Count);
				if (v > lookAhead) {
					v = lookAhead;
				}

				wayPoints.Clear ();

				if (Vector3.Distance (wayToGoal [v], target.position) < 2.0f) {
					wayPoints.Add (target.position);
					//Debug.Log ("here");

				} else {
					wayPoints = CheckForCorners (wayToGoal [v], rad, "Corner");	
					//Debug.Log (wayPoints.Remove (this.transform.position));
				}
			}


			FindAllPaths ();
			currentDangerValue = float.MaxValue;
			//Debug.Log (listPath.Count );

			foreach (Vector3[] currentPath in listPath) {
				float val = CalculatePathValue (currentPath);
				float multi = 10.0f;
				float dangerLevel;

				if (currentPath.Length == 0) {
					val += Vector3.Distance (this.transform.position, target.position) * multi;
					dangerLevel = CalculateDangerLevel (this.transform.position, enemyDetectionRadius, "Enemy");
				} else {
					//val += Vector3.Distance (currentPath [currentPath.Length - 1], target.position) * multi;

					PathrequestManager.RequestPath (currentPath[currentPath.Length-1], target.transform.position, GetPathForHelp, useWeights);

					dangerLevel = CalculateDangerLevel (currentPath[currentPath.Length-1], enemyDetectionRadius, "Enemy");
				}
					


				if (helpPath != null) {
					dangerLevel += CalculatePathValue (helpPath) * 2.0f;
				}

				helpPath = null;

				val += dangerLevel;

				//Debug.Log (val);
				if (val < currentDangerValue) {
					currentDangerValue = val;
					helpPathPoint = currentPath;
				}

				//Debug.Log (val);
			}

			path = helpPathPoint;
			listPath.Clear ();
		}
	}

	public Transform TargetValue {
		get {
			return this.target;
		}
		set {
			target = value;
		}
	}

	public bool IsDone{
		get {
			return this.isFinished;
		}
		set{
			isFinished = value;
		}
	}

	public void ResetValues(){
		StopAllCoroutines ();
		wayPoints.Clear();
		listPath.Clear();
		path = null;
		wayToGoal = null;
		isFinished = false;
		currentWaypoint = this.transform.position;
	}

	private float CalculateDangerLevel(Vector3 targetway, float radius, string name)
	{
		float dLevel = 0;

		Collider[] colliders = new Collider[cornerMax];
		Physics.OverlapSphereNonAlloc(targetway, radius, colliders, (1 << LayerMask.NameToLayer(name)));

		List<Vector3> retCorners = new List<Vector3>();

		foreach (Collider target in colliders)
		{
			if (target == null)
				break;

			dLevel += dangerValueForEnemy;
		}
			
		return dLevel;
	}

    private float CalculateDistance(Vector3[] targetway)
    {
        float ret = 0;
        for (int i = 0; i < targetway.Length - 1; i++)
        {
            ret += Vector3.Distance(targetway[i], targetway[i + 1]);
            if (target.transform.position == targetway[i + 1])
            {
				return -float.MaxValue;
            }
            
        }
        ret += Vector3.Distance(targetway[0], this.transform.position);
        return ret;
    }

	public List<Vector3> CheckForCorners(Vector3 centerPoint, float radius, string layerName)
    {
        Collider[] colliders = new Collider[cornerMax];
        Physics.OverlapSphereNonAlloc(centerPoint, radius, colliders, (1 << LayerMask.NameToLayer(layerName)));

        List<Vector3> retCorners = new List<Vector3>();

        foreach (Collider col in colliders)
        {
            if (col == null)
                break;


            retCorners.Add(col.gameObject.transform.position);
        }

        return retCorners;
    }

    void FindAllPaths()
    {
		listPath.Clear ();
        foreach (Vector3 point in wayPoints)
        {
            PathrequestManager.RequestPath(transform.position, point, AddPathToList, useWeights);
        }
    }


    public void AddPathToList(Vector3[] newPath, bool pathsuccessfull)
    {
        if (pathsuccessfull)
        {
            listPath.Add(newPath);
        }
    }

	public void GetBestPath(Vector3[] newPath, bool pathsuccessfull)
	{
		if (pathsuccessfull) {
			wayToGoal = newPath;
		} else {
			wayToGoal = null;
		}
	}

	public void GetPathForHelp(Vector3[] newPath, bool pathsuccessfull)
	{
		if (pathsuccessfull) {
			helpPath = newPath;
		} else {
			helpPath = null;
		}
	}

    int CalculatePathValue(Vector3[] currentPath)
    {
        int value = 0;

        foreach (Vector3 point in currentPath)
        {
            Node n = map.NodeFromWorldPoint(point);
            value += n.Weight;
        }

        return value;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessfull)
    {
        if (pathSuccessfull)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
		if (path != null) {
			if (path.Length > 0) {
				currentWaypoint = path [0];
			}

			targetIndex = 0;

			while (true) {
				if (path == null) {
					yield return null;
					isFinished = true;
				}
            
				if (transform.position == currentWaypoint) {
					targetIndex++;
					if (targetIndex >= path.Length) {
						path = null;

						yield break;
					}

					currentWaypoint = path [targetIndex];
				}

				transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
				yield return null;
			}
		}
		yield return null;
	}

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
		if (wayToGoal != null)
        {
			for (int i = targetIndex; i < wayToGoal.Length-1; i++)
            {
                Gizmos.color = Color.black;
				Gizmos.DrawCube(wayToGoal[i], Vector3.one);
            }
        }

        List<Vector3> cornersinReach = CheckForCorners(this.transform.position, rad, "Corner");
        Vector3[] Corners = new Vector3[cornersinReach.Count];
        int k = 0;

        foreach (Vector3 pos in cornersinReach)
        {
            Corners[k] = pos;
            k++;
        }

        UnityEditor.Handles.color = Color.red;
        foreach (Vector3 pos in Corners)
        {
            UnityEditor.Handles.DrawLine(transform.position, pos);
        }

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, rad);
    }
#endif
}
