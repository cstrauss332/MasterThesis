using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerData : MonoBehaviour
{

    private float m_Weight = 0;

    private const float YellowWeight = 10f;
    private const float RedWeight = 40f;

    public List<PatrolUnitThief> m_ThiefsYellow = new List<PatrolUnitThief>();
    public List<PatrolUnitThief> m_ThiefsRed = new List<PatrolUnitThief>();

    void Awake()
    {
        this.name = "[" + m_Weight + "]";
    }

    public void SetWeight(PatrolUnitThief unit, string color)
    {
        if (color == "Yellow")
        {
            if (!m_ThiefsYellow.Contains(unit))
            {
                m_Weight += YellowWeight;
                this.name = "[" + m_Weight + "]";
                m_ThiefsYellow.Add(unit);
            }
        }
        if (color == "Red")
        {
            if (!m_ThiefsRed.Contains(unit))
            {
                m_Weight += RedWeight;
                this.name = "[" + m_Weight + "]";
                m_ThiefsRed.Add(unit);
            }
        }
    }


#if UNITY_EDITOR
void OnDrawGizmos()
{
		if (m_Weight >= 40) {
			Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			UnityEditor.Handles.Label (pos, this.name);
		}
}
#endif

}
