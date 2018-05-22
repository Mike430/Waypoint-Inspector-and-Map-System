using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( WayPointMap ) )]
public class WayPointMapInspector : Editor
{
    List<WayPointInspectorItem> m_InspectorWayPoints = new List<WayPointInspectorItem>( );
    WayPointMap m_Map;
    int m_CurrentSelection;

    // TEXT
    // Util
    string m_strNewObjectName = "WayPoint";
    // Presentation
    string m_keyTitle = "WayPoint Management";
    string m_keyAddWaypoint = "Add WayPoint";
    string m_keyListName = "WayPoints";
    string m_keyNoWaypoints = "No Waypoints to inspect";
    string m_keyWaypointName = "WayPoint ";
    string m_KeyDelete = "Delete";
    string m_KeyExamine = "Examine";
    string m_KeyInspect = "Select";
    string m_keyPosition = "Position";
    string m_keyConnectionCountField = "Connection count:";


    private bool DrawConnectionListEditor( int waypointIndex )
    {
        bool saveAndRedrawRequired = false;
        int newCount = EditorGUILayout.IntField( m_keyConnectionCountField, m_InspectorWayPoints[ waypointIndex ].m_Connections.Count );
        if ( newCount != m_InspectorWayPoints[ waypointIndex ].m_Connections.Capacity )
        {
            saveAndRedrawRequired = true;
            // Increase
            if ( newCount > m_InspectorWayPoints[ waypointIndex ].m_Connections.Count )
            {
                m_InspectorWayPoints[ waypointIndex ].m_Connections.Capacity = newCount;
                while ( m_InspectorWayPoints[ waypointIndex ].m_Connections.Count < newCount )
                {
                    m_InspectorWayPoints[ waypointIndex ].m_Connections.Add( 0 );
                }
            }
            // Decrease
            else
            {
                while ( m_InspectorWayPoints[ waypointIndex ].m_Connections.Count > newCount )
                {
                    m_InspectorWayPoints[ waypointIndex ].m_Connections.RemoveAt( m_InspectorWayPoints[ waypointIndex ].m_Connections.Count - 1 );
                }
                m_InspectorWayPoints[ waypointIndex ].m_Connections.Capacity = newCount;
            }
        }

        for ( int j = 0; j < m_InspectorWayPoints[ waypointIndex ].m_Connections.Count; ++j )
        {
            int old = m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ];
            m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ] = EditorGUILayout.IntField( m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ] );
            if ( old != m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ] )
            {
                if ( m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ] > m_InspectorWayPoints.Count )
                {
                    m_InspectorWayPoints[ waypointIndex ].m_Connections[ j ] = 0;
                }

                saveAndRedrawRequired = true;
            }
        }

        return saveAndRedrawRequired;
    }

    private void DrawInterfaceForWaypoints()
    {
        bool needToSave = false;
        bool needRedraw = false;

        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            m_InspectorWayPoints[ i ].m_Collapsed = EditorGUILayout.Foldout( m_InspectorWayPoints[ i ].m_Collapsed, m_keyWaypointName + i );

            if ( m_InspectorWayPoints[ i ].m_Collapsed )
            {
                GUILayout.BeginHorizontal( );

                if ( GUILayout.Button( m_KeyDelete ) )
                {
                    WaypointDeletedSortConnections( i );
                    GameObject temp = m_InspectorWayPoints[ i ].m_WayPoint.gameObject;
                    m_InspectorWayPoints.RemoveAt( i );
                    DestroyImmediate( temp );

                    SaveWayPointsToCurrentMap( );
                    continue;
                }

                if ( GUILayout.Button( m_KeyExamine ) )
                {
                    m_CurrentSelection = i;
                    needRedraw = true;
                }

                if ( GUILayout.Button( m_KeyInspect ) )
                {
                    Selection.activeGameObject = m_InspectorWayPoints[ i ].m_WayPoint.gameObject;
                    return;
                }

                GUILayout.EndHorizontal( );

                m_InspectorWayPoints[ i ].m_WayPoint.transform.position = EditorGUILayout.Vector3Field( m_keyPosition, m_InspectorWayPoints[ i ].m_WayPoint.transform.position );

                if ( DrawConnectionListEditor( i ) )
                {
                    needRedraw = true;
                    needToSave = true;
                }
            }
        }

        if ( needRedraw )
        {
            EditorUtility.SetDirty( m_Map ); // This forces a scene redraw
        }

        if ( needToSave )
        {
            SaveWayPointsToCurrentMap( );
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label( m_keyTitle );

        if ( GUILayout.Button( m_keyAddWaypoint ) )
        {
            GameObject wp = new GameObject( );
            wp.AddComponent<WayPoint>( );
            wp.name = m_strNewObjectName;
            wp.transform.position = m_Map.transform.position + ( Vector3.up * 5 );

            WayPointInspectorItem wpDB = new WayPointInspectorItem( );
            wpDB.m_WayPoint = wp.GetComponent<WayPoint>( );

            m_InspectorWayPoints.Add( wpDB );
            SaveWayPointsToCurrentMap( );
        }

        GUILayout.Label( m_keyListName );
        if ( m_InspectorWayPoints.Count == 0 )
        {
            GUILayout.Label( m_keyNoWaypoints );
            return;
        }

        DrawInterfaceForWaypoints( );
    }

    // Unity calls this every time the 3D viewport updates or when the mouse moves over the window
    private void OnSceneGUI()
    {
        WayPointMap temp = ( WayPointMap ) target;
        if ( m_Map != temp )
        {
            SaveWayPointsToCurrentMap( );
            m_InspectorWayPoints.Clear( );
            m_Map = temp;
            ReadWayPointsIntoInspector( );
        }

        for ( int i = 0; i < m_InspectorWayPoints.Count; ++i )
        {
            PresentWayPointIn3DView( i );
        }
        m_Map.EditorSetSelected( m_CurrentSelection );
        Handles.Label( m_Map.transform.position, "Current Selection = " + m_CurrentSelection );
    }

    private void PresentWayPointIn3DView( int index )
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
            if ( i == index )
            {
                continue;
            }

            for ( int j = 0; j < m_InspectorWayPoints[ i ].m_Connections.Count; ++j )
            {
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

    private void ReadWayPointsIntoInspector()
    {
        if ( m_Map == null )
        {
            return;
        }

        m_InspectorWayPoints.Clear( );

        for ( int i = 0; i < m_Map.m_WayPoints.Count; ++i )
        {
            WayPointInspectorItem newPoint = new WayPointInspectorItem( );
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
    }

    private void SaveWayPointsToCurrentMap()
    {
        if ( m_Map == null )
        {
            return;
        }

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
    }
}
