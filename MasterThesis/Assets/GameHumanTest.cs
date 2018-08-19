using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameHumanTest : MonoBehaviour {

	[SerializeField]
	public GameObject m_StealthUnit;
	[SerializeField]
	public List<GameObject> m_PatrolUnits;
	[SerializeField]
	private List<GameObject> m_TargetPoints;
	[SerializeField]
	private DebugWayPointIndicator m_dwpi;

	[SerializeField]
	public int m_NumberOfRuns;
	[SerializeField]
	public string m_FileName;

	private string m_FileOutput ="";
	private List<GameObject> m_WorkList;

	private Transform m_StartPoint;
	private Transform m_EndPoint;
	private int m_NumberOfProcessedRuns = 0;

	private List<PatrolUnitThief> m_PatrolUnitScript;


	private List<CaughtInfo> m_CaughtInfoList;

	// Use this for initialization
	void Awake () {

		if (m_TargetPoints.Count <= 2) {
			Debug.LogError ("Too few or no TargetPoints specified!");
		}

		if (!m_StealthUnit) {
			Debug.LogError ("No StealthUnit Assigned!");
		}	

		if (m_PatrolUnits.Count > 0) {
			m_PatrolUnitScript = new List<PatrolUnitThief> ();
			foreach (GameObject patrol in m_PatrolUnits) {
				PatrolUnitThief help = patrol.GetComponent<PatrolUnitThief> ();
				m_PatrolUnitScript.Add (help);
			}
		}

		m_FileOutput = "";
		m_FileOutput += "Number" + "\t" + "Path" + "\t" + "Coordinates";

		m_CaughtInfoList = new List<CaughtInfo> ();

		//Resets WorkList
		Reset();

		//Init FirstRun
		CalculateFirstPoint();
		m_StealthUnit.transform.position = m_StartPoint.position + Vector3.up;
		//m_StealthUnit.ResetValues ();
		//m_StealthUnit.target = m_EndPoint;	

	}
	
	// Update is called once per frame
	void Update () {

		if (m_PatrolUnitScript.Count > 0) {
			bool caught = false;

			foreach (PatrolUnitThief unit in m_PatrolUnitScript) {
				if (unit.TargetCaught) {
					unit.TargetCaught = false;
					m_FileOutput += System.Environment.NewLine + m_NumberOfProcessedRuns.ToString () + ";" + m_StartPoint.ToString() + "-" + m_EndPoint.ToString() + ";" + "x=" + ";" + unit.LocationWhereTargetCaught.x + ";" + "y=" + ";" + unit.LocationWhereTargetCaught.z;
					m_CaughtInfoList.Add(new CaughtInfo(m_NumberOfProcessedRuns,m_StartPoint.position,m_EndPoint.position,unit.LocationWhereTargetCaught));
					caught = true;
					//m_FileOutput += "";
					break;
				}
			}

			if (caught) {
				NextTarget ();
				Debug.Log ("Caught");
				m_NumberOfProcessedRuns++;
			}
		}

		if (Vector3.Distance(m_StealthUnit.transform.position, m_EndPoint.position) < 5f) {
			m_FileOutput += System.Environment.NewLine + m_NumberOfProcessedRuns.ToString () + ";" + m_StartPoint.ToString () + "-" + m_EndPoint.ToString ();
			m_CaughtInfoList.Add(new CaughtInfo(m_NumberOfProcessedRuns,m_StartPoint.position,m_EndPoint.position,Vector3.zero));
			NextTarget ();
			m_NumberOfProcessedRuns++;
			Debug.Log ("Run Done");
		}


			
		if (m_NumberOfRuns == m_NumberOfProcessedRuns) {
			System.IO.File.WriteAllText (m_FileName + ".csv", m_FileOutput);

			Save (m_CaughtInfoList, m_FileName + ".serilized");

			Debug.Log ("Writing Done");
			m_NumberOfProcessedRuns++;
			Debug.Log ("Time = " + Time.realtimeSinceStartup);

			Application.Quit ();
		}
	}

	void NextTarget(){
		if (m_WorkList.Count == 0) {
			Reset ();
			m_WorkList.Remove (m_EndPoint.gameObject);
		}

		CalculateNewPoint ();

		m_StealthUnit.transform.position = m_StartPoint.position + Vector3.up;
		//m_StealthUnit.ResetValues ();
		//m_StealthUnit.target = m_EndPoint;	
	}

	private void Reset(){
		m_WorkList = new List<GameObject> ();
		m_WorkList.AddRange (m_TargetPoints);
	}

	private void CalculateNewPoint() {
		m_StartPoint = m_EndPoint;

		GameObject target = m_WorkList [UnityEngine.Random.Range (0, m_WorkList.Count - 1)];
		m_EndPoint = target.transform;
		m_dwpi.Target = target;
		m_WorkList.Remove (m_EndPoint.gameObject);
	}

	private void CalculateFirstPoint() {
		m_EndPoint = m_WorkList[UnityEngine.Random.Range (0, m_WorkList.Count-1)].transform;
		m_WorkList.Remove (m_EndPoint.gameObject);

		CalculateNewPoint ();
	}

    public void Save(System.Object objectToSerialize, string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, objectToSerialize);
        stream.Close();
    }

}
