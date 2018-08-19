using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaughtInfo {

	[System.Serializable]
    public struct Vector3Serializer
    {
        public float x;
        public float y;
        public float z;
 
        public void Fill(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }
 
        public Vector3 V3
        { get { return new Vector3(x, y, z); } }
    }

	public int NumOfRuns = 0;
	public Vector3Serializer StartPos;
	public Vector3Serializer EndPos;
	public Vector3Serializer CaughtPos;

	public CaughtInfo(int numofruns, Vector3 start, Vector3 end,Vector3 caught){
		NumOfRuns = numofruns;
		StartPos.Fill(start);
		EndPos.Fill(end);
		CaughtPos.Fill(caught);
	}
}
