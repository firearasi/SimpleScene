// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;

using System.Threading;
using System.Globalization;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using SimpleScene;
using SimpleScene.Demos;

namespace Simulation
{
	partial class Simulation : TestBenchBootstrap
	{
        SSInstancedMeshRenderer asteroidRingRenderer = null;
		Vector2 ringAngularVelocity = new Vector2 (0.03f, 0.01f);
        protected float localTime = 0f;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
		{
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Simulation game = new Simulation())
			{
				game.Run(30.0);
			}
		}

		public Simulation()
			: base("TestBench0: Particle Systems")
		{
		}


		/// <summary>
		/// Called when it is time to setup the next frame. Add you game logic here.
		/// </summary>
		/// <param name="e">Contains timing information for framerate independent logic.</param>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame (e);
            float timeElapsed = (float)e.Time;
            if (asteroidRingRenderer != null) {
				asteroidRingRenderer.EulerDegAngleOrient (ringAngularVelocity.X, ringAngularVelocity.Y);
			}

            if (timeElapsed <= 0f) return;

            // make the target drone move from side to side
            localTime += timeElapsed;

            //Adjust location
            satObj.Pos= new OpenTK.Vector3(20.0f*(float)Math.Sin(localTime/2.0),0f, 
                -20.0f*(float)Math.Cos(localTime/2.0));
             //Adjust Orientation
            //satObj.Orient(newForward,newUp);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            float timeElapsed = (float)e.Time;
            GL.Color3(System.Drawing.Color.Red);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(20.0f * (float)Math.Sin((localTime - timeElapsed) / 2.0),0f,
                -20.0f * (float)Math.Cos((localTime - timeElapsed) / 2.0));
            GL.Vertex3( 20.0f * (float)Math.Sin(localTime / 2.0),0f,
                -20.0f * (float)Math.Cos(localTime / 2.0));


            GL.End();


        }
    }
}