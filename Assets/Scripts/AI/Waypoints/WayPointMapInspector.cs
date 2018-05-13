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

    private void OnSceneGUI()
    {
        m_Map = (WayPointMap)target;
        m_HandleTransform = m_Map.transform;
        List<Vector3> wayPoints = m_Map.m_WayPoints;
        
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
        Vector3 point = m_HandleTransform.TransformPoint(m_Map.m_WayPoints[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, Quaternion.identity);
        Handles.Label(point, "Waypoint: " + index);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Map, "Move Way Point");
            EditorUtility.SetDirty(m_Map);
            m_Map.m_WayPoints[index] = m_HandleTransform.InverseTransformPoint(point);
            return true;
        }
        return false;
    }
}
