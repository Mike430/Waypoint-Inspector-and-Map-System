using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    Vector3 _mTail;
    float _mTailLength;
    // Use this for initialization
    void Start()
    {
        _mTailLength = 1.5f;
    }

    // Update is called by a TailManager once each frame
    public void CustomUpdate()
    {
        _mTail = transform.position - (transform.forward * _mTailLength);
    }

    public Vector3 GetTailPosition()
    {
        return _mTail;
    }
}
