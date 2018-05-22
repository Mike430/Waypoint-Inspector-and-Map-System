using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WayPointInspectorItem
{
    public bool m_Collapsed;
    public List<int> m_Connections;
    public WayPoint m_WayPoint;

    public WayPointInspectorItem()
    {
        m_Collapsed = false;
        m_Connections = new List<int>( );
    }
}
