using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSnake : MonoBehaviour
{
    private List<WayPoint> m_Path = new List<WayPoint>();
    private Vector3 m_Target;
    private Rigidbody m_Rigidbody;

    public float m_Speed = 100.0f;
    public float m_LookTorque = 10.0f;
    public float m_Sensitivity = 5.0f;
    public float m_WayPointSensitivity = 10.0f;
    public WayPointMap m_Map;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Target = m_Map.m_WayPoints[Random.Range(0, m_Map.m_WayPoints.Count - 1)].transform.position;
        m_Path = PathFinding.AStar.findPath(transform.position, m_Target, m_Map);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((transform.position - m_Target).magnitude < m_WayPointSensitivity)
        {
            m_Target = m_Map.m_WayPoints[Random.Range(0, m_Map.m_WayPoints.Count - 1)].transform.position;
            m_Path = PathFinding.AStar.findPath(transform.position, m_Target, m_Map);
        }
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (m_Path.Count > 0)
        {
            Vector3 heading = SteeringBehaviours.Steering.Seek(m_Path[m_Path.Count - 1].transform.position, transform.position, m_Speed);
            m_Rigidbody.AddForce( heading );
            Debug.DrawLine(transform.position, transform.position + heading * 10);
            TorqueLookRotation(heading);
            if ((m_Path[m_Path.Count - 1].transform.position - transform.position).magnitude < m_WayPointSensitivity)
            {
                m_Path.RemoveAt(m_Path.Count - 1);
            }
        }
        else
        {
            this.transform.position += SteeringBehaviours.Steering.Arrive(m_Target, this.transform.position, m_Speed, m_Sensitivity) * Time.deltaTime;
        }
    }


    private void TorqueLookRotation(Vector3 heading)
    {
        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.forward, heading);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.forward, heading);

        // apply torque along that axis according to the magnitude of the angle.
        m_Rigidbody.AddTorque(cross * angleDiff * m_LookTorque);
    }
}
