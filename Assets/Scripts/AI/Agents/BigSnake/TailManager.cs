using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public List<GameObject> _mTailBodies;
    public float minDistance = 1.0f;

    // Use this for initialization
    void Start()
    {
        //add checks
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for ( int i = 0; i < _mTailBodies.Count; ++i )
        {
            if (_mTailBodies[i] != null)
            {
                Follower foll = _mTailBodies[i].GetComponent<Follower>();
                if (foll != null)
                {
                    _mTailBodies[i].GetComponent<Follower>().CustomUpdate(minDistance);
                }
                _mTailBodies[i].GetComponent<Body>().CustomUpdate();
            }
        }
    }
}
