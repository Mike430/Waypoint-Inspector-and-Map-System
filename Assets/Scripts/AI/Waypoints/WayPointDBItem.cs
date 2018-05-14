using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WayPointDBItem
{
    public bool m_Collapsed;
    public List<int> m_Connections;
    public WayPoint m_WayPoint;

    public WayPointDBItem()
    {
        m_Collapsed = false;
        m_Connections = new List<int>();
    }
}
