using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPointMap))]
public class WayPointMapInspector : Editor
{
    Transform m_HandleTransform;
    List<WayPointDBItem> m_InspectorWayPoints = new List<WayPointDBItem>();
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

            WayPointDBItem wpDB = new WayPointDBItem();
            wpDB.m_WayPoint = wp.GetComponent<WayPoint>();

            m_InspectorWayPoints.Add(wpDB);
            WriteWayPointsToMap();
        }

        GUILayout.Label("WayPoints");
        if (m_InspectorWayPoints.Count == 0)
        {
            GUILayout.Label("No Waypoints to inspect");
            return;
        }

        for (int i = 0; i < m_InspectorWayPoints.Count; ++i)
        {
            //GUILayout.Label("WayPoint " + i);
            m_InspectorWayPoints[i].m_Collapsed = EditorGUILayout.Foldout(m_InspectorWayPoints[i].m_Collapsed, "WayPoint " + i);
            
            bool needToSave = false;
            bool deleteCalled = false;
            
            if (m_InspectorWayPoints[i].m_Collapsed)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Delete"))
                {
                    needToSave = true;
                    GameObject temp = m_InspectorWayPoints[i].m_WayPoint.gameObject;
                    m_InspectorWayPoints.RemoveAt(i);
                    DestroyImmediate(temp);
                    deleteCalled = true;
                }

                if (!deleteCalled)
                {
                    if (GUILayout.Button("Select"))
                    {
                        m_CurrentSelection = i;
                    }

                    GUILayout.EndHorizontal();

                    m_InspectorWayPoints[i].m_WayPoint.transform.position = EditorGUILayout.Vector3Field("Position", m_InspectorWayPoints[i].m_WayPoint.transform.position);
                }

                if (needToSave || deleteCalled)
                {
                    WriteWayPointsToMap();
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        WayPointMap temp = (WayPointMap)target;
        if (m_Map != temp)
        {
            // Save
            WriteWayPointsToMap();
            // Clear the inspector
            m_InspectorWayPoints.Clear();
            m_Map = temp;
            // Read new map data
            ReadWayPointsToDB();
        }

        //ReadWayPointsToDB(); // THIS LINE OF CODE IDENTIFIES THE BUG!!!
        m_HandleTransform = m_Map.transform;

        for (int i = 0; i < m_InspectorWayPoints.Count; ++i)
        {
            if (ShowPoint(i))
            {
                m_CurrentSelection = i;
            }
        }
        m_Map.EditorSetSelected(m_CurrentSelection);
        Handles.Label(m_Map.transform.position, "Current Selection = " + m_CurrentSelection);
    }

    //returns true if manipulated
    private bool ShowPoint(int index)
    {
        //Debug.Log("inspector wp count = " + m_InspectorWayPoints.Count);
        //Debug.Log("inspector at index = " + m_InspectorWayPoints[index].ToString());
        //if (m_InspectorWayPoints[index].m_WayPoint == null)
        //{
        //    Debug.Log("Error drawing handles - inspector wp count: " + m_InspectorWayPoints.Count);
        //    return false;
        //}

        Vector3 point = m_HandleTransform.TransformPoint(m_InspectorWayPoints[index].m_WayPoint.transform.position);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, Quaternion.identity);
        Handles.Label(point, "Waypoint: " + index);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Map, "Move Way Point");
            EditorUtility.SetDirty(m_Map);
            m_InspectorWayPoints[index].m_WayPoint.transform.position = m_HandleTransform.InverseTransformPoint(point);
            return true;
        }
        return false;
    }

    private void ReadWayPointsToDB()
    {
        if (m_Map == null)
        {
            return;
        }
        
        int firstMapCount = m_Map.m_WayPoints.Count;
        int firstInspectorCount = m_InspectorWayPoints.Count;
        int finalMapCount = 0;
        int finalInspectorCount = 0;

        m_InspectorWayPoints.Clear();

        for (int i = 0; i < m_Map.m_WayPoints.Count; ++i)
        {
            WayPointDBItem newPoint = new WayPointDBItem();
            newPoint.m_WayPoint = m_Map.m_WayPoints[i];

            for (int j = 0; j < m_Map.m_WayPoints[i].m_Connections.Count; ++j)
            {
                int connection = 0;
                for (int k = 0; k < m_Map.m_WayPoints.Count; ++i)
                {
                    if (m_Map.m_WayPoints[i].m_Connections[j] == m_Map.m_WayPoints[k])
                    {
                        connection = k;
                        break;
                    }
                }
                newPoint.m_Connections.Add(connection);
            }

            m_InspectorWayPoints.Add(newPoint);
        }

        finalMapCount = m_Map.m_WayPoints.Count;
        finalInspectorCount = m_InspectorWayPoints.Count;
        Debug.Log("~~~ READING ~~~"
            + "\nMap Capacity: " + m_Map.m_WayPoints.Capacity
            + "\nFirst - Map count: " + firstMapCount
            + "\nFirst - Insp count: " + firstInspectorCount
            + "\nFinal - Map count: " + finalMapCount
            + "\nFinal - Insp count: " + finalInspectorCount + "\n\n");
    }

    private void WriteWayPointsToMap()
    {
        if (m_Map == null)
        {
            return;
        }

        int firstMapCount = m_Map.m_WayPoints.Count;
        int firstInspectorCount = m_InspectorWayPoints.Count;
        int finalMapCount = 0;
        int finalInspectorCount = 0;

        m_Map.m_WayPoints.Clear();
        m_Map.m_WayPoints.Capacity = m_InspectorWayPoints.Count;
        for (int i = 0; i < m_InspectorWayPoints.Count; ++i)
        {
            m_Map.m_WayPoints.Add(m_InspectorWayPoints[i].m_WayPoint);

            for (int j = 0; j < m_InspectorWayPoints[i].m_Connections.Count; ++i)
            {
                int connectedIndex = m_InspectorWayPoints[i].m_Connections[j];
                WayPoint connected = m_InspectorWayPoints[connectedIndex].m_WayPoint;
                m_Map.m_WayPoints[i].m_Connections.Add(connected);
            }
        }

        finalMapCount = m_Map.m_WayPoints.Count;
        finalInspectorCount = m_InspectorWayPoints.Count;

        Debug.Log("~~~ WRITING ~~~"
            + "\nMap Capacity: " + m_Map.m_WayPoints.Capacity
            + "\nFirst - Map count: " + firstMapCount
            + "\nFirst - Insp count: " + firstInspectorCount
            + "\nFinal - Map count: " + finalMapCount
            + "\nFinal - Insp count: " + finalInspectorCount + "\n\n");
    }
}
