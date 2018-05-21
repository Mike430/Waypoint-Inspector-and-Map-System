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


    public WayPoint ClosestWaypoint(Vector3 position)
    {
        float nearestDistance = float.MaxValue;
        WayPoint nearestWaypoint = null;

        foreach (WayPoint wp in m_WayPoints)
        {
            float dist = (position - wp.transform.position).magnitude;
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestWaypoint = wp;
            }
        }
        return nearestWaypoint;
    }

    public void EditorSetSelected(int index)
    {
        m_SelectedWaypoint = index;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.1f, 0.5f, 0.75f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(0.75f, 2.0f, 0.75f));
        Gizmos.DrawSphere(transform.position + Vector3.up * 2.5f, 0.4f);

        if (m_WayPoints.Count > 0)
        {
            for (int i = 0; i < m_WayPoints.Count; ++i)
            {
                if (m_WayPoints[i] == null)
                {
                    m_WayPoints.RemoveAt(i);
                    break;
                }

                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                for (int j = 0; j < m_WayPoints[i].m_Connections.Count; ++j)
                {
                    Gizmos.DrawLine(m_WayPoints[i].transform.position, m_WayPoints[i].m_Connections[j].transform.position);
                }

                Gizmos.color = new Color(0.5f, 0.0f, 0.0f, 0.75f);
                Gizmos.DrawCube(m_WayPoints[i].transform.position, Vector3.one);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1.0f, 0.2f, 1.0f, 1.0f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(0.75f, 2.0f, 0.75f));
        Gizmos.DrawSphere(transform.position + Vector3.up * 2.5f, 0.4f);

        if (m_WayPoints.Count > 0)
        {
            for (int i = 0; i < m_WayPoints.Count; ++i)
            {
                if (m_WayPoints[i] == null)
                {
                    m_WayPoints.RemoveAt(i);
                    break;
                }

                if (i != m_SelectedWaypoint)
                {
                    Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    Gizmos.DrawCube(m_WayPoints[i].transform.position, Vector3.one);
                }
                else
                {
                    for (int j = 0; j < m_WayPoints[i].m_Connections.Count; ++j)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(m_WayPoints[i].transform.position, m_WayPoints[i].m_Connections[j].transform.position);
                    }

                    Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                    Gizmos.DrawCube(m_WayPoints[i].transform.position, Vector3.one * 2);
                }
            }
        }
    }
}
