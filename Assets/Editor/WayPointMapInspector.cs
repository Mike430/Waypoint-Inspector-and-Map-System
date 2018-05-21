using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( WayPointMap ) )]
public class WayPointMapInspector : Editor
{
    List<WayPointDBItem> m_InspectorWayPoints = new List<WayPointDBItem>( );
    WayPointMap m_Map;
    int m_CurrentSelection;
    bool m_DebugLoggingEnabled = false;

    // TEXT
    // Util
    string m_strNewObjectName = "WayPoint";
    // Presentation
    string m_keyTitle = "WayPoint Management";
    string m_keyAddWaypoint = "Add WayPoint";
    string m_keyDebugLogging = "Enable Debug Logging";
    string m_keyListName = "WayPoints";
    string m_keyNoWaypoints = "No Waypoints to inspect";
    string m_keyWaypointName = "WayPoint ";
    string m_KeyDelete = "Delete";
    string m_KeyExamine = "Examine";
    string m_KeyInspect = "Select";
    string m_keyPosition = "Position";
    string m_keyConnectionCountField = "Connection count:";

    // UI and function code for the inspector
    public override void OnInspectorGUI()
    {
        GUILayout.Label( m_keyTitle );

        if ( GUILayout.Button( m_keyAddWaypoint ) )
        {
            GameObject wp = new GameObject( );
            wp.AddComponent<WayPoint>( );
            wp.name = m_strNewObjectName;
            wp.transform.position = m_Map.transform.position + (Vector3.up * 5);

            WayPointDBItem wpDB = new WayPointDBItem( );
            wpDB.m_WayPoint = wp.GetComponent<WayPoint>( );

            m_InspectorWayPoints.Add( wpDB );
            WriteWayPointsToMap( );
        }

        m_DebugLoggingEnabled = GUILayout.Toggle( m_DebugLoggingEnabled, m_keyDebugLogging );

        GUILayout.Label( m_keyListName );
        if ( m_InspectorWayPoints.Count == 0 )
        {
            GUILayout.Label( m_keyNoWaypoints );
            return;
        }

        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            m_InspectorWayPoints[ i ].m_Collapsed = EditorGUILayout.Foldout( m_InspectorWayPoints[ i ].m_Collapsed, m_keyWaypointName + i );

            bool needToSave = false;
            bool deleteCalled = false;
            bool needRedraw = false;

            if ( m_InspectorWayPoints[ i ].m_Collapsed )
            {
                GUILayout.BeginHorizontal( );

                if ( GUILayout.Button( m_KeyDelete ) )
                {
                    WaypointDeletedSortConnections( i );
                    needToSave = true;
                    GameObject temp = m_InspectorWayPoints[ i ].m_WayPoint.gameObject;
                    m_InspectorWayPoints.RemoveAt( i );
                    DestroyImmediate( temp );
                    deleteCalled = true;
                }

                if ( !deleteCalled )
                {
                    if ( GUILayout.Button( m_KeyExamine ) )
                    {
                        m_CurrentSelection = i;
                        needRedraw = true;
                    }

                    if ( GUILayout.Button( m_KeyInspect ) )
                    {
                        Selection.activeGameObject = m_InspectorWayPoints[i].m_WayPoint.gameObject;
                        return;
                    }

                    GUILayout.EndHorizontal( );

                    m_InspectorWayPoints[ i ].m_WayPoint.transform.position = EditorGUILayout.Vector3Field( m_keyPosition, m_InspectorWayPoints[ i ].m_WayPoint.transform.position );

                    int newCount = EditorGUILayout.IntField( m_keyConnectionCountField, m_InspectorWayPoints[ i ].m_Connections.Count );
                    if ( newCount != m_InspectorWayPoints[ i ].m_Connections.Capacity )
                    {
                        needRedraw = true;
                        needToSave = true;
                        // Increase
                        if ( newCount > m_InspectorWayPoints[ i ].m_Connections.Count )
                        {
                            m_InspectorWayPoints[ i ].m_Connections.Capacity = newCount;
                            while ( m_InspectorWayPoints[ i ].m_Connections.Count < newCount )
                            {
                                m_InspectorWayPoints[ i ].m_Connections.Add( 0 );
                            }
                        }
                        // Decrease
                        else
                        {
                            while ( m_InspectorWayPoints[ i ].m_Connections.Count > newCount )
                            {
                                m_InspectorWayPoints[ i ].m_Connections.RemoveAt( m_InspectorWayPoints[ i ].m_Connections.Count - 1 );
                            }
                            m_InspectorWayPoints[ i ].m_Connections.Capacity = newCount;
                        }
                    }

                    for ( int j = 0; j < m_InspectorWayPoints[ i ].m_Connections.Count; ++j )
                    {
                        int old = m_InspectorWayPoints[ i ].m_Connections[ j ];
                        m_InspectorWayPoints[ i ].m_Connections[ j ] = EditorGUILayout.IntField( m_InspectorWayPoints[ i ].m_Connections[ j ] );
                        if ( old != m_InspectorWayPoints[ i ].m_Connections[ j ] )
                        {
                            if ( m_InspectorWayPoints[ i ].m_Connections[ j ] > m_InspectorWayPoints.Count )
                            {
                                m_InspectorWayPoints[ i ].m_Connections[ j ] = 0;
                            }

                            needToSave = true;
                            needRedraw = true;
                        }
                    }
                }

                if ( needRedraw )
                {
                    EditorUtility.SetDirty( m_Map ); // force scene redraw
                }

                if ( needToSave || deleteCalled )
                {
                    WriteWayPointsToMap( );
                }
            }
        }
    }

    // Run every time the 3D viewport updates
    private void OnSceneGUI()
    {
        WayPointMap temp = ( WayPointMap ) target;
        if ( m_Map != temp )
        {
            // Save
            WriteWayPointsToMap( );
            // Clear the inspector
            m_InspectorWayPoints.Clear( );
            // Get new waypoint map
            m_Map = temp;
            // Read new map data
            ReadWayPointsToDB( );
        }

        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            //Vector3 point = m_InspectorWayPoints[ i ].m_WayPoint.transform.position;
            //Handles.Label( point, "Waypoint: " + i );
            ShowPoint(i);
        }
        m_Map.EditorSetSelected( m_CurrentSelection );
        Handles.Label( m_Map.transform.position, "Current Selection = " + m_CurrentSelection );
    }

    private void ShowPoint( int index )
    {
        if ( m_CurrentSelection == index )
        {
            Vector3 point = m_InspectorWayPoints[ index ].m_WayPoint.transform.position;
            EditorGUI.BeginChangeCheck( );
            point = Handles.DoPositionHandle( point, Quaternion.identity );
            Handles.Label( point, "Waypoint: " + index );

            if ( EditorGUI.EndChangeCheck( ) )
            {
                // no point recording for undo functionality currently :(
                //Undo.RecordObject( m_Map, "Move Way Point" );
                EditorUtility.SetDirty( m_Map );
                m_InspectorWayPoints[ index ].m_WayPoint.transform.position = point;
            }
        }
        else
        {
            Vector3 point = m_InspectorWayPoints[ index ].m_WayPoint.transform.position;
            Handles.Label( point, "Waypoint: " + index );
        }
    }

    private void WaypointDeletedSortConnections( int index )
    {
        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            if ( i != index )
            {
                for ( int j = 0; j < m_InspectorWayPoints[ i ].m_Connections.Count; ++j )
                {
                    /*if ( i == index - 1 &&
                        m_InspectorWayPoints[ i ].m_Connections[ j ] == index &&
                        index != m_InspectorWayPoints.Count - 1)
                    {
                        // This is the one circumstance in which we do not wish to drop the waypoint index
                        // wp0 ---> wp1 ---> wp2
                        // delete wp1
                        // wp0 ---> wp2
                    }*/
                    if ( m_InspectorWayPoints[ i ].m_Connections[ j ] == index )
                    {
                        m_InspectorWayPoints[ i ].m_Connections.RemoveAt( j );
                        --j;// re evaluate this index for the next if statement
                    }
                    else if ( m_InspectorWayPoints[ i ].m_Connections[ j ] >= index )
                    {
                        m_InspectorWayPoints[ i ].m_Connections[ j ] -= 1;
                    }
                }
            }
        }
    }

    // Reads in the waypoint system from m_Map
    private void ReadWayPointsToDB()
    {
        if ( m_Map == null )
        {
            return;
        }

        int firstMapCount = m_Map.m_WayPoints.Count;
        int firstInspectorCount = m_InspectorWayPoints.Count;
        int finalMapCount = 0;
        int finalInspectorCount = 0;

        m_InspectorWayPoints.Clear( );

        for ( int i = 0; i < m_Map.m_WayPoints.Count; ++i )
        {
            WayPointDBItem newPoint = new WayPointDBItem( );
            newPoint.m_WayPoint = m_Map.m_WayPoints[ i ];

            for ( int j = 0; j < m_Map.m_WayPoints[ i ].m_Connections.Count; ++j )
            {
                int connection = 0;
                for ( int k = 0; k < m_Map.m_WayPoints.Count; ++k )
                {
                    if ( m_Map.m_WayPoints[ i ].m_Connections[ j ] == m_Map.m_WayPoints[ k ] )
                    {
                        connection = k;
                        break;
                    }
                }
                newPoint.m_Connections.Add( connection );
            }

            m_InspectorWayPoints.Add( newPoint );
        }

        if ( m_DebugLoggingEnabled )
        {
            finalMapCount = m_Map.m_WayPoints.Count;
            finalInspectorCount = m_InspectorWayPoints.Count;
            Debug.Log( "~~~ READING ~~~"
                + "\nMap Capacity: " + m_Map.m_WayPoints.Capacity
                + "\nFirst - Map count: " + firstMapCount
                + "\nFirst - Insp count: " + firstInspectorCount
                + "\nFinal - Map count: " + finalMapCount
                + "\nFinal - Insp count: " + finalInspectorCount + "\n\n" );
        }
    }

    // Writes out the currect waypoint system to m_Map
    private void WriteWayPointsToMap()
    {
        if ( m_Map == null )
        {
            return;
        }

        int firstMapCount = m_Map.m_WayPoints.Count;
        int firstInspectorCount = m_InspectorWayPoints.Count;
        int finalMapCount = 0;
        int finalInspectorCount = 0;

        m_Map.m_WayPoints.Clear( );
        m_Map.m_WayPoints.Capacity = m_InspectorWayPoints.Count;
        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            m_Map.m_WayPoints.Add( m_InspectorWayPoints[ i ].m_WayPoint );
            m_Map.m_WayPoints[ i ].m_Connections.Clear( );

            if ( m_InspectorWayPoints[ i ].m_Connections.Count > 0 )
            {
                for ( int j = 0; j < m_InspectorWayPoints[ i ].m_Connections.Count; ++j )
                {
                    int connectedIndex = m_InspectorWayPoints[ i ].m_Connections[ j ];

                    if ( connectedIndex < m_InspectorWayPoints.Count )
                    {
                        WayPoint connected = m_InspectorWayPoints[ connectedIndex ].m_WayPoint;
                        m_Map.m_WayPoints[ i ].m_Connections.Add( connected );
                    }
                }
            }
        }

        if ( m_DebugLoggingEnabled )
        {
            finalMapCount = m_Map.m_WayPoints.Count;
            finalInspectorCount = m_InspectorWayPoints.Count;
            Debug.Log( "~~~ WRITING ~~~"
                + "\nMap Capacity: " + m_Map.m_WayPoints.Capacity
                + "\nFirst - Map count: " + firstMapCount
                + "\nFirst - Insp count: " + firstInspectorCount
                + "\nFinal - Map count: " + finalMapCount
                + "\nFinal - Insp count: " + finalInspectorCount + "\n\n" );
        }
    }
}
