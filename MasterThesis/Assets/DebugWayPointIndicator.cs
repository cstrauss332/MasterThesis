using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWayPointIndicator : MonoBehaviour
{

    [SerializeField]
    private GameObject m_Player;
    [SerializeField]
	GameObject m_Target;
	public GameObject Target { set { m_Target = value; m_Indicator.transform.position = value.transform.position;  } }
	[SerializeField]
	GameObject m_Indicator;

    [Space]
    [SerializeField]
    private float m_DistanceFromCenter = 25f;

    private void CalculateHUDPosition(GameObject target)
    {
        float angle = CalcAngle(m_Player.transform.position, target.transform.position);

        Vector3 v = new Vector3(0, Vector3.Distance(m_Player.transform.position, target.transform.position) * m_DistanceFromCenter, 0);

        v = Quaternion.Euler(angle - 90, 0, 0) * v;

        Vector2 vec = new Vector2(v.z, v.y);

        if (vec.x > (Screen.width / 2))
            vec.x = (Screen.width / 2) -24;

        if (vec.y > (Screen.height / 2))
            vec.y = (Screen.height / 2) -24;

        if (vec.x < -(Screen.width / 2))
            vec.x = -(Screen.width / 2);

        if (vec.y < -(Screen.height / 2))
            vec.y = -(Screen.height / 2);

        GUI.Box(new Rect(vec.x + (Screen.width / 2), vec.y + (Screen.height / 2), 24, 24), "" + "X");

        //GUI.Box(new Rect(v.z + (Screen.width / 2), v.y + (Screen.height / 2), 24, 24), "" + target.name.ToCharArray()[0]);

        //GUI.Label(new Rect((Screen.width - 200), (25), 200, 24), target.name + "[ X ]");
    }

    public float CalcAngle(Vector3 start, Vector3 end)
    {
        Vector3 myPos = start;
        myPos.y = 0;

        Vector3 targetPos = end;
        targetPos.y = 0;

        Vector3 toOther = (myPos - targetPos).normalized ;

        return (Mathf.Atan2(toOther.z, toOther.x) * Mathf.Rad2Deg) + Camera.main.transform.rotation.eulerAngles.y;
    }

    public void OnGUI()
    {
		CalculateHUDPosition(m_Target);
    }
}







/*

    atan2Approximation(toOther.z, toOther.x) * Mathf.Rad2Deg + 180;

    float atan2Approximation(float y, float x)
    {
        float t0, t1, t2, t3, t4;
        t3 = Mathf.Abs(x);
        t1 = Mathf.Abs(y);
        t0 = Mathf.Max(t3, t1);
        t1 = Mathf.Min(t3, t1);
        t3 = 1f / t0;
        t3 = t1 * t3;
        t4 = t3 * t3;
        t0 = -0.013480470f;
        t0 = t0 * t4 + 0.057477314f;
        t0 = t0 * t4 - 0.121239071f;
        t0 = t0 * t4 + 0.195635925f;
        t0 = t0 * t4 - 0.332994597f;
        t0 = t0 * t4 + 0.999995630f;
        t3 = t0 * t3;
        t3 = (Mathf.Abs(y) > Mathf.Abs(x)) ? 1.570796327f - t3 : t3;
        t3 = (x < 0) ? 3.141592654f - t3 : t3;
        t3 = (y < 0) ? -t3 : t3;
        return t3;
    }

*/

