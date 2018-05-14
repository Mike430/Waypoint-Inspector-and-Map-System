using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteeringBehaviours;

public class Follower : MonoBehaviour
{
    [SerializeField]
    private Body _mBodyTarget = null;
    [SerializeField]
    private Transform _mTarget = null;

    [SerializeField]
    private float followDistance = 2.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called by a TailManager once each frame
    public void CustomUpdate(float minDistance)
    {
        if (_mBodyTarget != null)
        {
            Vector3 look = _mBodyTarget.GetTailPosition() - transform.position;
            transform.rotation = Quaternion.LookRotation(look, Vector3.up);

            if ((transform.position - _mBodyTarget.GetTailPosition()).magnitude > minDistance)
            {
                //transform.position = _mBodyTarget.GetTailPosition( ) - transform.forward * 0.5f;
                transform.position += Steering.Arrive(_mBodyTarget.GetTailPosition(), transform.position, 5, 50);
            }
        }
        else if (_mTarget != null)
        {
            Vector3 look = _mTarget.position - transform.position;
            transform.rotation = Quaternion.LookRotation(look, Vector3.up);

            if ((transform.position - _mTarget.position).magnitude > minDistance)
            {
                //transform.position = _mTarget.position - transform.forward * followDistance;
                Vector3 spacer = _mTarget.position + ((transform.position - _mTarget.position).normalized);
                transform.position += Steering.Arrive(spacer, transform.position, 5, 50);
            }
        }
    }
}
