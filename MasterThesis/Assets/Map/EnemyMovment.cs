using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    [SerializeField]
    private float m_DirectRadius = 18f;
    //---- WALKPATTERN ----
    public enum PatrolType { None, PointToPoint, Random, RandomArea }
    [SerializeField]
    private PatrolType m_PatrolType;
    [SerializeField]
    private List<GameObject> m_WayPointList;

    private int m_CurrentPointIndex = 0;
    private Vector3 m_TargetPoint;

    [SerializeField]
    private float m_PauseTime = 2f;
    private float m_CurrentWaitTime = 0f;

    private UnityEngine.AI.NavMeshAgent m_Nav;
    private bool IsAtDestination { get { return Vector3.Distance(m_Nav.destination, this.transform.position) <= m_Nav.stoppingDistance; } }
    private bool isReachable { get { return m_Nav.hasPath; } }

    void Awake()
    {
        m_Nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (m_WayPointList.Count != 0)
        {
            MoveTo(m_WayPointList[0].transform.position);
        }
    }

    void Update()
    {
        Patrol();
    }

    // ---- WALKPATTERN ----
    //WALK FROM POINT TO POINT 
    private void PointToPoint()
    {
        //If no points a setted
        if (m_WayPointList.Count != 0)
        {
            //at point go to next
            if ((IsAtDestination || !isReachable) && Wait(m_PauseTime))
            {
                m_CurrentPointIndex++;
                //Reset to Begin
                if (m_CurrentPointIndex >= m_WayPointList.Count)
                    m_CurrentPointIndex = 0;

                if (m_WayPointList.Count == 1)
                    m_Nav.Stop();
                else
                    MoveTo(m_WayPointList[m_CurrentPointIndex].transform.position);
            }
        }
    }

    //WALK FROM POINT TO ANOTHER RANDOM POINT
    private void WalkToRandomPoint()
    {
        if (m_WayPointList.Count > 0)
        {
            //at point go to next
            if ((IsAtDestination) && Wait(m_PauseTime))
            {
                m_CurrentPointIndex = UnityEngine.Random.Range(0, m_WayPointList.Count);
                MoveTo(m_WayPointList[m_CurrentPointIndex].transform.position);
            }
        }
    }

    //WALK RANDOM IN AN AREA
    private void WalkAroundInArea()
    {
        if (m_WayPointList.Count >= 2)
        {
            //at point go to next
            if ((IsAtDestination || !isReachable) && Wait(m_PauseTime))
            {
                MoveTo(new Vector3(UnityEngine.Random.Range(m_WayPointList[0].transform.position.x, m_WayPointList[1].transform.position.x), UnityEngine.Random.Range(m_WayPointList[0].transform.position.y, m_WayPointList[1].transform.position.y), UnityEngine.Random.Range(m_WayPointList[0].transform.position.z, m_WayPointList[1].transform.position.z)));
            }
        }
    }

    //WALK TO FIRST POINT
    private void FirstPoint()
    {
        //If no points a setted
        if (m_WayPointList.Count != 0)
        {
            //at point go to next
            if ((IsAtDestination ||!isReachable))
            {
                MoveTo(m_WayPointList[0].transform.position);
            }
        }
    }

    //WALK RANDOM AROUND THE MAP
    private void WalkAround()
    {
        Vector3 point1 = new Vector3(this.transform.position.x + m_DirectRadius, this.transform.position.y, this.transform.position.z + m_DirectRadius);
        Vector3 point2 = new Vector3(this.transform.position.x - m_DirectRadius, this.transform.position.y, this.transform.position.z - m_DirectRadius);

        if (IsAtDestination || !isReachable)
            MoveTo(new Vector3(UnityEngine.Random.Range(point1.x, point2.x), this.transform.position.y, UnityEngine.Random.Range(point1.z, point2.z)));
    }

    //EXECUTE WALKPATTERN
    private void Patrol()
    {
        switch (m_PatrolType)
        {
            case PatrolType.PointToPoint:
                PointToPoint();
                break;
            case PatrolType.Random:
                WalkToRandomPoint();
                break;
            case PatrolType.RandomArea:
                WalkAroundInArea();
                break;
            default:
                FirstPoint();
                break;
        }
    }

    public void MoveTo(Vector3 point)
    {
        m_TargetPoint = point;
        m_Nav.SetDestination(point);
        m_Nav.Resume();
    }

    public void StopMoving()
    {
        m_Nav.velocity = Vector3.zero;
        m_Nav.Stop();
    }

    // --- WaitFunction ---
    private bool Wait(float waitingTime)        //Wait returns true when the function has waited for the specified Time (Has to be execute once per frame)
    {                                           //resets when it reaches the specified time
        bool ret = false;

        ret = m_CurrentWaitTime >= waitingTime;

        if (!ret)
        {
            m_CurrentWaitTime += Time.deltaTime;
        }
        else
        {
            m_CurrentWaitTime = 0;
        }

        return ret;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        float height = 0;//transform.position.y + 0.02f;

        //DRAW PATROL TYPE
        if (m_WayPointList != null)
            switch (m_PatrolType)
            {
                case PatrolType.PointToPoint:

                    for (int i = 0; i < m_WayPointList.Count - 1; i++)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(m_WayPointList[i].transform.position + (Vector3.up * height), m_WayPointList[i + 1].transform.position + (Vector3.up * height));
                        Gizmos.color = Color.green;
                        GizmosDrawArrow(m_WayPointList[i].transform.position, m_WayPointList[i].transform.forward);
                    }
                    Gizmos.color = Color.white;

                    if (m_WayPointList.Count > 0)
                    {
                        Gizmos.DrawLine(m_WayPointList[m_WayPointList.Count - 1].transform.position + (Vector3.up * height), m_WayPointList[0].transform.position + (Vector3.up * height));
                        Gizmos.color = Color.green;
                        GizmosDrawArrow(m_WayPointList[m_WayPointList.Count - 1].transform.position, m_WayPointList[m_WayPointList.Count - 1].transform.forward);
                    }
                    break;
                case PatrolType.Random:
                    for (int i = 0; i < m_WayPointList.Count; i++)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(new Vector3(m_WayPointList[i].transform.position.x - 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z - 1), new Vector3(m_WayPointList[i].transform.position.x + 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z - 1));
                        Gizmos.DrawLine(new Vector3(m_WayPointList[i].transform.position.x + 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z - 1), new Vector3(m_WayPointList[i].transform.position.x + 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z + 1));

                        Gizmos.DrawLine(new Vector3(m_WayPointList[i].transform.position.x + 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z + 1), new Vector3(m_WayPointList[i].transform.position.x - 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z + 1));
                        Gizmos.DrawLine(new Vector3(m_WayPointList[i].transform.position.x - 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z + 1), new Vector3(m_WayPointList[i].transform.position.x - 1, m_WayPointList[i].transform.position.y, m_WayPointList[i].transform.position.z - 1));

                        Gizmos.color = Color.green;
                        GizmosDrawArrow(m_WayPointList[i].transform.position, m_WayPointList[i].transform.forward);
                    }
                    break;
                case PatrolType.RandomArea:
                    Gizmos.color = Color.yellow;
                    if (m_PatrolType == PatrolType.RandomArea && m_WayPointList.Count >= 2)
                    {
                        Gizmos.DrawLine(new Vector3(m_WayPointList[0].transform.position.x, m_WayPointList[0].transform.position.y, m_WayPointList[1].transform.position.z), m_WayPointList[0].transform.position + (Vector3.up * height));
                        Gizmos.DrawLine(new Vector3(m_WayPointList[0].transform.position.x, m_WayPointList[0].transform.position.y, m_WayPointList[1].transform.position.z), m_WayPointList[1].transform.position + (Vector3.up * height));

                        Gizmos.DrawLine(new Vector3(m_WayPointList[1].transform.position.x, m_WayPointList[1].transform.position.y, m_WayPointList[0].transform.position.z), m_WayPointList[0].transform.position + (Vector3.up * height));
                        Gizmos.DrawLine(new Vector3(m_WayPointList[1].transform.position.x, m_WayPointList[1].transform.position.y, m_WayPointList[0].transform.position.z), m_WayPointList[1].transform.position + (Vector3.up * height));
                    }
                    break;
                default:
                    if (m_WayPointList != null && m_WayPointList.Count > 0)
                    {
                        Gizmos.color = Color.green;
                        GizmosDrawArrow(m_WayPointList[0].transform.position, m_WayPointList[0].transform.forward);
                    }
                    break;
            }

        //DRAW PLAYER FORWORD
        Gizmos.color = Color.white;
        GizmosDrawArrow(this.gameObject.transform.position, this.gameObject.transform.forward);


        if (m_TargetPoint != null && m_TargetPoint != Vector3.zero)
        {
            //DRAW TARGETPOINT
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(new Vector3(m_TargetPoint.x - 1, m_TargetPoint.y, m_TargetPoint.z - 1), new Vector3(m_TargetPoint.x + 1, m_TargetPoint.y, m_TargetPoint.z - 1));
            Gizmos.DrawLine(new Vector3(m_TargetPoint.x + 1, m_TargetPoint.y, m_TargetPoint.z - 1), new Vector3(m_TargetPoint.x + 1, m_TargetPoint.y, m_TargetPoint.z + 1));

            Gizmos.DrawLine(new Vector3(m_TargetPoint.x + 1, m_TargetPoint.y, m_TargetPoint.z + 1), new Vector3(m_TargetPoint.x - 1, m_TargetPoint.y, m_TargetPoint.z + 1));
            Gizmos.DrawLine(new Vector3(m_TargetPoint.x - 1, m_TargetPoint.y, m_TargetPoint.z + 1), new Vector3(m_TargetPoint.x - 1, m_TargetPoint.y, m_TargetPoint.z - 1));

            GizmosDrawArrow(m_TargetPoint, this.gameObject.transform.forward);

            //DRAW LINE TO TARGETPOINT
            Gizmos.color = Color.white;
            Gizmos.DrawLine(this.transform.position + (Vector3.up * height), m_TargetPoint);
        }
    }

    private void GizmosDrawArrow(Vector3 startPoint, Vector3 direction)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(startPoint, direction);
        Gizmos.DrawRay(startPoint + direction, right * 0.5f);
        Gizmos.DrawRay(startPoint + direction, left * 0.5f);
    }
#endif
}
