using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform m_CameraYaw;
    [SerializeField]
    private Transform m_CameraPitch;
    [SerializeField]
    private float m_MaxCameraPitchAngle = 0.0f;
    [SerializeField]
    private float m_MinCameraPitchAngle = 0.0f;
    private float m_CameraYawAngle = 0.0f;
    private float m_CameraPitchAngle = 0.0f;
    private bool m_Jump;
    private Rigidbody m_Rigidbody;
    [SerializeField]
    private float m_WalkSpeed;
    [SerializeField]
    private float m_RunSpeed;
    [SerializeField]
    private float m_CameraSensitivity;
    [SerializeField]
    private float m_JumpForce;
    private Vector3 m_MoveForce;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_CameraYawAngle += Input.GetAxis("Mouse X") * m_CameraSensitivity;
        m_CameraPitchAngle += -Input.GetAxis("Mouse Y") * m_CameraSensitivity;

        if (m_CameraPitchAngle > m_MaxCameraPitchAngle)
            m_CameraPitchAngle = m_MaxCameraPitchAngle;

        if (m_CameraPitchAngle < m_MinCameraPitchAngle)
            m_CameraPitchAngle = m_MinCameraPitchAngle;

        m_CameraYaw.localEulerAngles = new Vector3(0.0f, m_CameraYawAngle, 0.0f);
        m_CameraPitch.localEulerAngles = new Vector3(m_CameraPitchAngle, 0.0f, 0.0f);
        m_CameraYawAngle = m_CameraYaw.localEulerAngles.y;

        m_MoveForce = Vector3.zero;
        m_MoveForce += m_CameraYaw.right * Input.GetAxis("Horizontal");
        m_MoveForce += m_CameraYaw.forward * Input.GetAxis("Vertical");

        m_MoveForce = m_MoveForce.normalized * (Input.GetKey(KeyCode.LeftShift) ? m_RunSpeed : m_WalkSpeed);

        m_Jump = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        if (m_Jump)
        {
            m_Rigidbody.AddForce(Vector3.up * m_JumpForce);
        }

        m_Rigidbody.AddForce(m_MoveForce);
        Debug.DrawLine(transform.position, transform.position + m_MoveForce, Color.red);
    }
}
