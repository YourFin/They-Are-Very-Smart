using UnityEngine;
using System;

namespace Evolution
{
    public class PolarVector
    {
        public float direction;
        public float magnitude;

        public PolarVector(float direction, float magnitude)
        {
            this.direction = direction;
            this.magnitude = magnitude;
        }

        public PolarVector(float direction)
        {
            this.direction = direction;
            magnitude = 0;
        }

        public PolarVector()
        {
            direction = 0;
            magnitude = 0;
        }

        public static double DegToRad(float degrees)
        {
            double CONVERSION_FACTOR = (Math.PI / 180);
            return degrees * CONVERSION_FACTOR;
        }

        public PolarVector makeRelative(PolarVector toTransform)
        {
            return new PolarVector(toTransform.direction, toTransform.magnitude);
        }

        public float Rotate(float degrees_to_rotate)
        {
            direction += degrees_to_rotate;
            return direction;
        }

        private static readonly Vector3 E1 = new Vector3(1, 0, 0);
        public static PolarVector fromVector3(Vector3 vv)
        {
            Vector3 withoutY = new Vector3(vv.x, 0, vv.z);
            var angle = Vector3.Angle(E1, withoutY);
            if (withoutY.z < 0) angle = -angle;
            return new PolarVector(angle, withoutY.magnitude);
        }

        public Vector3 toVector3()
        {
            return new Vector3(
                    (float)Math.Cos(DegToRad(direction)) * magnitude,
                    0.0f,
                    (float)Math.Sin(DegToRad(direction)) * magnitude
                    );
        }

        public static Vector3 toVector3(PolarVector vv)
        {
            return vv.toVector3();
        }

        public override string ToString()
        {
            return $"({this.direction} deg, {magnitude})";
        }
    }
}