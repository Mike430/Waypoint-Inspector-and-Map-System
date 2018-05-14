﻿using System.Collections;
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
                    Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                    Gizmos.DrawCube(m_WayPoints[i].transform.position, Vector3.one);
                }
            }
        }
    }
}
