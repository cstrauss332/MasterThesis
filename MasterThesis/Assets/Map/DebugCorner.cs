using System.Collections.Generic;
using UnityEngine;

public class DebugCorner : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> m_Corners;

    void Start()
    {
        if (m_Corners == null)
            m_Corners = new List<Vector3>();

		Reset ();
    }

    void Reset()
    {
        m_Corners = new List<Vector3>();

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            m_Corners.Add(transform.GetChild(i).transform.position);
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawPolyLine(m_Corners.ToArray());
    }
#endif
}
