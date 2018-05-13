using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class WayPointMap : MonoBehaviour
{
    // Editor
    private int m_SelectedWaypoint;

    //public List<Vector3> m_WayPoints = new List<Vector3>();
    public List<WayPoint> m_WayPoints = new List<WayPoint>();

    // Use this for initialization
    void Start()
    {

    }

    public void EditorSetSelected(int index)
    {
        //Debug.Log("Setting selected to " + index);
        m_SelectedWaypoint = index;
    }

    private void OnDrawGizmos()
    {
        if (m_WayPoints.Count > 0)
        {
            foreach (WayPoint point in m_WayPoints)
            {
                Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                Gizmos.DrawSphere(transform.position + point.transform.position, 1.0f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_WayPoints.Count > 0)
        {
            for (int i = 0; i < m_WayPoints.Count; ++i)
            {
                if (i != m_SelectedWaypoint)
                {
                    Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                    Gizmos.DrawSphere(transform.position + m_WayPoints[i].transform.position, 1.0f);
                }
                else
                {
                    Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                    Gizmos.DrawSphere(transform.position + m_WayPoints[i].transform.position, 1.0f);
                }
            }
        }
    }
}
