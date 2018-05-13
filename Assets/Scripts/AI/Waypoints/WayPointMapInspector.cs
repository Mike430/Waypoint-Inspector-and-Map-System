using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPointMap))]
public class WayPointMapInspector : Editor
{
    Transform m_HandleTransform;
    WayPointMap m_Map;
    int m_CurrentSelection;

    public override void OnInspectorGUI()
    {
        GUILayout.Label("WayPoint Management");

        if (GUILayout.Button("Add WayPoint"))
        {
            GameObject wp = new GameObject();
            wp.AddComponent<WayPoint>();
            wp.name = "WayPoint";
            m_Map.m_WayPoints.Add(wp.GetComponent<WayPoint>());
        }

        //base.OnInspectorGUI();

        GUILayout.Label("WayPoints");
        for (int i = 0; i < m_Map.m_WayPoints.Count; ++i)
        {
            GUILayout.Label("WayPoint " + i);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete"))
            {
                GameObject temp = m_Map.m_WayPoints[i].gameObject;
                m_Map.m_WayPoints.RemoveAt(i);
                DestroyImmediate(temp);
            }

            if (GUILayout.Button("Select"))
            {
                m_CurrentSelection = i;
            }

            GUILayout.EndHorizontal();

            m_Map.m_WayPoints[i].transform.position = EditorGUILayout.Vector3Field("Position", m_Map.m_WayPoints[i].transform.position);
        }
    }

    private void OnSceneGUI()
    {
        m_Map = (WayPointMap)target;
        m_HandleTransform = m_Map.transform;
        List<WayPoint> wayPoints = m_Map.m_WayPoints;
        
        for (int i = 0; i < wayPoints.Count; ++i)
        {
            if (ShowPoint(i))
            {
                m_CurrentSelection = i;
            }
        }
        m_Map.EditorSetSelected(m_CurrentSelection);
        Handles.Label(m_Map.transform.position, "Current Selection = " + m_CurrentSelection);
    }

    //returns try if manipulated
    private bool ShowPoint(int index)
    {
        Vector3 point = m_HandleTransform.TransformPoint(m_Map.m_WayPoints[index].transform.position);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, Quaternion.identity);
        Handles.Label(point, "Waypoint: " + index);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Map, "Move Way Point");
            EditorUtility.SetDirty(m_Map);
            m_Map.m_WayPoints[index].transform.position = m_HandleTransform.InverseTransformPoint(point);
            return true;
        }
        return false;
    }
}
