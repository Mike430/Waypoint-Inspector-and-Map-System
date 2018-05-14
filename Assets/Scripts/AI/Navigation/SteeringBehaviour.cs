using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviours
{
    public static class Steering
    {
        public enum behaviour
        {
            SEEK,
            FLEE,
            ARRIVE,
            AVOID,
            PURSUIT,
            EVADE
        };

        // Binary steering
        public static Vector3 Seek(Vector3 target, Vector3 me, float speed)
        {
            return (target - me).normalized * speed;
        }


        public static Vector3 Flee(Vector3 target, Vector3 me, float speed)
        {
            return (me - target).normalized * speed;
        }

        // Throttled steering
        public static Vector3 Arrive(Vector3 target, Vector3 me, float speed, float sensitivity)
        {
            Vector3 diff = target - me;
            float diffMag = diff.magnitude;

            if (diffMag < sensitivity)
            {
                Vector3 returnVec;

                returnVec = diff.normalized * (speed * (diffMag / sensitivity));

                return returnVec;
            }
            else
            {
                return Seek(target, me, speed);
            }
        }

        public static Vector3 Avoid(Vector3 target, Vector3 me, float speed, float sensitivity)
        {
            Vector3 diff = me - target;
            float diffMag = diff.magnitude;

            if (diffMag > sensitivity)
            {
                Vector3 returnVec;

                float outOfBoundMag = (sensitivity / (diffMag - sensitivity));
                returnVec = diff.normalized * (speed * (outOfBoundMag / sensitivity));

                return returnVec;
            }
            else
            {
                return Flee(target, me, speed);
            }
        }

        // Predictive steering
		/* None of these have actually bee correctly implemented as it wasn't needed for the prototype.
		 * If you do need to implement these refer to the Mat Buckland examples
		*/
        public static Vector3 Pursuit(Vector3 target, Vector3 me, float speed)
        {
            return (me - target).normalized;
        }

        public static Vector3 Evade(Vector3 target, Vector3 me, float speed)
        {
            return (me - target).normalized;
        }
    }
}
