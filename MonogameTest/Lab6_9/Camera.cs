using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    public class Camera
    {
        static Matrix GetAxonometric(float phi, float psi)
        {
            var cospsi = MathF.Cos(psi);
            var cosphi = MathF.Cos(phi);
            var sinpsi = MathF.Sin(psi);
            var sinphi = MathF.Sin(phi);
            var matrix = new Matrix()
            {
                M11 = cospsi,
                M12 = sinphi * sinpsi,
                M21 = 0,
                M22 = cosphi,
                M31 = sinpsi,
                M32 = -sinphi * cospsi,
                M44 = 1,
            };
            return matrix;
        }
        static Matrix GetPerspective(float scale)
        {
            var matrix = new Matrix()
            {
                M11 = 1,
                M22 = 1,
                M33 = 0,
                M34 = -1 / scale,
                M44 = 1,
            };
            return matrix;
        }
        public Matrix ProjectionMatrix { get; private set; } = Matrix.Identity;
        public Matrix RotationMatrix { get; private set; } = Matrix.Identity;
        public Vector4 CameraCoordinate { get; private set; } = new Vector4(0, 0, 0, 0);
        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-CameraCoordinate.X, -CameraCoordinate.Y, -CameraCoordinate.Z)) * RotationMatrix;
        }
        public float Scale = 1.0f;
        public void RotateX(float angle)
        {
            RotationMatrix *= Matrix.CreateRotationX(angle);

        }
        public void RotateY(float angle)
        {
            RotationMatrix *= Matrix.CreateRotationY(angle);
        }
        public void RotateZ(float angle)
        {
            RotationMatrix *= Matrix.CreateRotationZ(angle);
        }
        public void MoveForward(float step)
        {
            var c = new Vector4(0, 0, step, 1);
            var t = Vector4.Transform(c, Matrix.Invert(this.RotationMatrix));

            CameraCoordinate += t ;
        }
        public void MoveDown(float step)
        {
            var c = new Vector4(0, step, 0, 1);
            var t = Vector4.Transform(c, Matrix.Invert(this.RotationMatrix));

            CameraCoordinate += t;
        }
        public void MoveLeft(float step)
        {
            var c = new Vector4(step, 0, 0, 1);
            var t = Vector4.Transform(c, Matrix.Invert(this.RotationMatrix));

            CameraCoordinate += t;
        }
        static public Camera GetOrtogonal()
        {
            Camera cam = new Camera();
            cam.CameraCoordinate = new Vector4(0, 0, -10, 0);
            cam.ProjectionMatrix = GetAxonometric(0, 0);
            cam.Scale = 100.0f;

            return cam;
        }
        static public Camera GetPerspective()
        {
            Camera cam = new Camera();
            cam.CameraCoordinate = new Vector4(0, 0, -10, 0);
            cam.ProjectionMatrix = GetPerspective(1.3f);
            cam.Scale = 800.0f;
            return cam;
        }
    }
}
