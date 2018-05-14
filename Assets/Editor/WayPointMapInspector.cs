﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPointMap))]
public class WayPointMapInspector : Editor
{
    List<WayPointDBItem> m_InspectorWayPoints = new List<WayPointDBItem>();
    WayPointMap m_Map;
    int m_CurrentSelection;
    bool m_DebugLoggingEnabled = false;

    // UI and function code for the inspector
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

        m_DebugLoggingEnabled = GUILayout.Toggle(m_DebugLoggingEnabled, "Enable Debug Logging");

        GUILayout.Label("WayPoints");
        if (m_InspectorWayPoints.Count == 0)
        {
            GUILayout.Label("No Waypoints to inspect");
            return;
        }

        for (int i = 0; i < m_InspectorWayPoints.Count; ++i)
        {
            m_InspectorWayPoints[i].m_Collapsed = EditorGUILayout.Foldout(m_InspectorWayPoints[i].m_Collapsed, "WayPoint " + i);

            bool needToSave = false;
            bool deleteCalled = false;
            bool needRedraw = false;

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
                        needRedraw = true;
                    }

                    GUILayout.EndHorizontal();

                    m_InspectorWayPoints[i].m_WayPoint.transform.position = EditorGUILayout.Vector3Field("Position", m_InspectorWayPoints[i].m_WayPoint.transform.position);

                    int newCount = EditorGUILayout.IntField("Connection count:", m_InspectorWayPoints[i].m_Connections.Count);
                    if (newCount != m_InspectorWayPoints[i].m_Connections.Capacity)
                    {
                        needRedraw = true;
                        needToSave = true;
                        // Increase
                        if (newCount > m_InspectorWayPoints[i].m_Connections.Count)
                        {
                            m_InspectorWayPoints[i].m_Connections.Capacity = newCount;
                            while (m_InspectorWayPoints[i].m_Connections.Count < newCount)
                            {
                                m_InspectorWayPoints[i].m_Connections.Add(0);
                            }
                        }
                        // Decrease
                        else
                        {
                            while (m_InspectorWayPoints[i].m_Connections.Count > newCount)
                            {
                                m_InspectorWayPoints[i].m_Connections.RemoveAt(m_InspectorWayPoints[i].m_Connections.Count - 1);
                            }
                            m_InspectorWayPoints[i].m_Connections.Capacity = newCount;
                        }
                    }

                    for (int j = 0; j < m_InspectorWayPoints[i].m_Connections.Count; ++j)
                    {
                        int old = m_InspectorWayPoints[i].m_Connections[j];
                        m_InspectorWayPoints[i].m_Connections[j] = EditorGUILayout.IntField(m_InspectorWayPoints[i].m_Connections[j]);
                        if (old != m_InspectorWayPoints[i].m_Connections[j])
                        {
                            if (m_InspectorWayPoints[i].m_Connections[j] > m_InspectorWayPoints.Count)
                            {
                                m_InspectorWayPoints[i].m_Connections[j] = 0;
                            }

                            needToSave = true;
                            needRedraw = true;
                        }
                    }
                }

                if (needRedraw)
                {
                    EditorUtility.SetDirty(m_Map); // force scene redraw
                }

                if (needToSave || deleteCalled)
                {
                    WriteWayPointsToMap();
                }
            }
        }
    }

    // Run every time the 3D viewport updates
    private void OnSceneGUI()
    {
        WayPointMap temp = (WayPointMap)target;
        if (m_Map != temp)
        {
            // Save
            WriteWayPointsToMap();
            // Clear the inspector
            m_InspectorWayPoints.Clear();
            // Get new waypoint map
            m_Map = temp;
            // Read new map data
            ReadWayPointsToDB();
        }

        for (int i = 0; i < m_InspectorWayPoints.Count; ++i)
        {
            Vector3 point = m_InspectorWayPoints[i].m_WayPoint.transform.position;
            Handles.Label(point, "Waypoint: " + i);
        }
        m_Map.EditorSetSelected(m_CurrentSelection);
        Handles.Label(m_Map.transform.position, "Current Selection = " + m_CurrentSelection);
    }

    // Reads in the waypoint system from m_Map
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
                for (int k = 0; k < m_Map.m_WayPoints.Count; ++k)
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

        if (m_DebugLoggingEnabled)
        {
            finalMapCount = m_Map.m_WayPoints.Count;
            finalInspectorCount = m_InspectorWayPoints.Count;
            Debug.Log("~~~ READING ~~~"
                + "\nMap Capacity: " + m_Map.m_WayPoints.Capacity
                + "\nFirst - Map count: " + firstMapCount
                + "\nFirst - Insp count: " + firstInspectorCount
                + "\nFinal - Map count: " + finalMapCount
                + "\nFinal - Insp count: " + finalInspectorCount + "\n\n");
        }
    }

    // Writes out the currect waypoint system to m_Map
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
            m_Map.m_WayPoints[i].m_Connections.Clear();

            if (m_InspectorWayPoints[i].m_Connections.Count > 0)
            {
                for (int j = 0; j < m_InspectorWayPoints[i].m_Connections.Count; ++j)
                {
                    int connectedIndex = m_InspectorWayPoints[i].m_Connections[j];

                    if (connectedIndex < m_InspectorWayPoints.Count)
                    {
                        WayPoint connected = m_InspectorWayPoints[connectedIndex].m_WayPoint;
                        m_Map.m_WayPoints[i].m_Connections.Add(connected);
                    }
                }
            }
        }

        if (m_DebugLoggingEnabled)
        {
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
}