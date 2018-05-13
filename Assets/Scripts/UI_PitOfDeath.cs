using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode()]
[RequireComponent(typeof(BoxCollider))]
public class UI_PitOfDeath : MonoBehaviour
{
    private BoxCollider m_CollisionVolume;
    // Use this for initialization
    void Start()
    {
        m_CollisionVolume = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        if (m_CollisionVolume == null)
        {
            m_CollisionVolume = GetComponent<BoxCollider>();
        }

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        Gizmos.DrawWireCube(transform.position + m_CollisionVolume.center, transform.localScale);

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        Gizmos.DrawCube(transform.position + m_CollisionVolume.center, transform.localScale);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_CollisionVolume == null)
        {
            m_CollisionVolume = GetComponent<BoxCollider>();
        }

        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        Gizmos.DrawWireCube(transform.position + m_CollisionVolume.center, transform.localScale);

        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.1f);
        Gizmos.DrawCube(transform.position + m_CollisionVolume.center, transform.localScale);
    }
}
