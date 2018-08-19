using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class CaughtInfoShower : MonoBehaviour {

	public List<string> Filenames;
	public float Radius = 2f;

	private List<CaughtInfo> m_CaughtInfoList = new List<CaughtInfo> ();

	public void CalculatePoints(){
		m_CaughtInfoList = new List<CaughtInfo> ();

		//Clear UP
		/*for (int i = 0; i < this.transform.childCount; i++) {
			DestroyImmediate (this.transform.GetChild (i).gameObject);
		}*/

		foreach (string filename in Filenames) {

			m_CaughtInfoList = (List<CaughtInfo>)Load (filename + ".serilized");



			foreach (CaughtInfo ci in m_CaughtInfoList) {
				if (ci.CaughtPos.V3 != Vector3.zero) {
					GameObject obj = new GameObject (); //Instantiate (GameObject, this.gameObject);
					obj.transform.SetParent (this.gameObject.transform);
					obj.transform.position = ci.CaughtPos.V3;
					obj.name = "CaughtPos" + ci.CaughtPos.V3.ToString ();
				}
			}
		}
    }

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		for (int i = 0; i < this.transform.childCount; i++) {
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere (this.transform.GetChild (i).position, Radius);
		}
	}
#endif


	public System.Object Load(string fileName)
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		System.Object deserializedObject = formatter.Deserialize(stream);
		stream.Close();

		return deserializedObject;
	}
}

#if UNITY_EDITOR

[CustomEditor(typeof(CaughtInfoShower))]
class CaughtInfoShowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

		CaughtInfoShower myTarget = (CaughtInfoShower)target;

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();

		if (GUILayout.Button ("Calculate"))
			myTarget.CalculatePoints ();
		
    }
}

#endif