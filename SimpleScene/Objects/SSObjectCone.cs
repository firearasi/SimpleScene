// Yan
using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace SimpleScene
{ 
    public class SSObjectCone : SSObject
    {
        public Vector3 vertex;
        public Vector3 dir;
        public float halfAngle;
        public int n;
        private List<Vector3> Points;
        private Vector3 u;
        private Vector3 v;
        private float radius;
        //private List<Vector3> Normals;


        
        public SSObjectCone(Vector3 vertex, Vector3 dir, float halfAngle, int n) : base()
        {
            this.Points = new List<Vector3>();
            this.vertex = vertex;
            this.dir = dir;
            this.halfAngle = halfAngle;
            this.n = n;
            u = Vector3.Cross(Vector3.UnitY, dir);
            v = Vector3.Cross(dir, u);
            u.Normalize();
            v.Normalize();
            float tan = (float)Math.Tan(halfAngle);
            Console.WriteLine(tan);
            radius = (float) dir.Length * tan;



            //Point i = radius*(u*cos..+v*sin...)

            for(int i=0;i<n;i++)
            {
                int j = (i + 1) % n;
                Points.Add(this.vertex);
                Points.Add(vertex+dir+radius * ((float)Math.Cos(i * 2 * Math.PI / n) * u + (float)Math.Sin(i * 2 * Math.PI / n) * v));
                Points.Add(vertex+dir+radius * ((float)Math.Cos(j * 2 * Math.PI / n) * u + (float)Math.Sin(j * 2 * Math.PI / n) * v));

            }
            //Filling Points and Normals

        }
        public override void Render(SSRenderConfig renderConfig)
        {
            base.Render(renderConfig);
            GL.Begin(PrimitiveType.Triangles);

            for(int i=0;i<n;i++)
            {
                var a = Points[3 * i];
                var b = Points[3 * i + 1];
                var c = Points[3 * i + 2];

                GL.Color3(0.2f, 0.7f, 0.2f);
                GL.Normal3(Vector3.Cross(c-b,a-c).Normalized());
                GL.Vertex3(a.X, a.Y, a.Z);
                //GL.Color4(0.2f, 0.7f, 0.2f, 0.3f);
                GL.Vertex3(b.X, b.Y, b.Z);
               //GL.Color4(0.2f, 0.7f, 0.2f, 0.3f);
                GL.Vertex3(c.X, c.Y, c.Z);
            }

            GL.End();
        }
    }
}