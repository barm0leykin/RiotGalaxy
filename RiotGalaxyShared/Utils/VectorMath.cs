using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RiotGalaxy.Objects;
using CocosSharp;

namespace RiotGalaxy
{
    public class VectorMath
    {
        public CCPoint Rotate(CCPoint point, float angle)
        {
            CCPoint newpoint;
            newpoint.X = point.X * (float)Math.Cos(angle) - point.Y * (float)Math.Sin(angle);
            newpoint.Y = point.X * (float)Math.Sin(angle) + point.Y * (float)Math.Cos(angle);
            return newpoint;
        }
        public double CalcAngle(CCPoint point1, CCPoint point2)//мои попытки решить самостоятельно)
        {
            double sideX = point2.X - point1.X;
            double sideY = point2.Y - point1.Y;
            //float gipotenuza = (float)Math.Sqrt(sideX * sideX + sideY * sideY);
            double tangens = sideY / sideX; // поменять местами?
            double angleRad = (float)Math.Atan(tangens);
            double angleGrad = angleRad * (180 / Math.PI);
            return angleGrad;
        }
        // This function assumes that it is contained in a CCNode-inheriting object
        public float AimAngle(CCPoint objPos, CCPoint target)
        {
            // Calculate the offset - the target's position relative to "this"
            float xOffset = target.X - objPos.X;
            float yOffset = target.Y - objPos.Y;

            // Make sure the target isn't the same point as "this". If so,
            // then rotation cannot be calculated.
            if (target.X != objPos.X || target.Y != objPos.Y)
            {

                // Call Atan2 to get the radians representing the angle from 
                // "this" to the target
                float radiansToTarget = (float)System.Math.Atan2(yOffset, xOffset);

                // Since CCNode uses degrees for its rotation, we need to convert
                // from radians
                float degreesToTarget = CCMathHelper.ToDegrees(radiansToTarget);

                // The direction that the entity faces when unrotated. In this case
                // the entity is facing "up", which is 90 degrees 
                const float forwardAngle = 90;

                // Adjust the angle we want to rotate by subtracting the
                // forward angle.
                float adjustedForDirecitonFacing = degreesToTarget - forwardAngle;

                // Invert the angle since CocosSharp uses clockwise rotation
                float cocosSharpAngle = adjustedForDirecitonFacing * -1;

                // Finally assign the rotation
                //obj.sprite.Rotation = cocosSharpAngle;
                return cocosSharpAngle;
            }
            return 0;
        }

        // This function assumes that it is contained in a CCNode-inheriting object
        public void FacePoint(GameObject obj, CCPoint target)
        {
            obj.Rotation = AimAngle(obj.Position, target);
        }

        // Rotates the argument vector by degrees specified by
        // cocosSharpDegrees. In other words, the rotation
        // value is expected to be clockwise.
        // The vector parameter is modified, so it is both an in and out value
        public void RotateVector(ref CCVector2 vector, float cocosSharpDegrees)
        {
            // Invert the rotation to get degrees as is normally
            // used in math (counterclockwise)
            float mathDegrees = -cocosSharpDegrees;

            // Convert the degrees to radians, as the System.Math
            // object expects arguments in radians
            float radians = CCMathHelper.ToRadians(mathDegrees);

            // Calculate the "up" and "right" vectors. This is essentially
            // a 2x2 matrix that we'll use to rotate the vector
            float xAxisXComponent = (float)System.Math.Cos(radians);
            float xAxisYComponent = (float)System.Math.Sin(radians);
            float yAxisXComponent = (float)System.Math.Cos(radians + CCMathHelper.Pi / 2.0f);
            float yAxisYComponent = (float)System.Math.Sin(radians + CCMathHelper.Pi / 2.0f);

            // Store the original vector values which will be used
            // below to perform the final operation of rotation.
            float originalX = vector.X;
            float originalY = vector.Y;

            // Use the axis values calculated above (the matrix values)
            // to rotate and assign the vector.
            vector.X = originalX * xAxisXComponent + originalY * yAxisXComponent;
            vector.Y = originalX * xAxisYComponent + originalY * yAxisYComponent;
        }
        public float GetVectorLength(CCPoint p1, CCPoint p2)
        {
            CCVector2 vector;
            vector.X = p2.X - p1.X;
            vector.Y = p2.Y - p1.Y;
            return vector.Length();

            //int length = (p1.X * 2) + (p2)
        }
    }
}