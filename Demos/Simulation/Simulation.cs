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
using System.Collections.Generic;

namespace Simulation
{
   
	partial class Simulation : TestBenchBootstrap
	{
        List<SSObject> trajectories;
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
			: base("Simple Simulation")
		{
            trajectories = new List<SSObject>();
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
            var duration = 10.0f;
            localTime += timeElapsed;

            //Adjust location
            var newPos= new OpenTK.Vector3(20.0f*(float)Math.Sin(localTime/duration),0f, 
                -20.0f*(float)Math.Cos(localTime/duration));
            var dir = (newPos - satObj.Pos);
            var dirNormed = dir;

            dirNormed.Normalize();
            SSRay ray = new SSRay(satObj.Pos, dir);
            SSObjectRay objRay = new SSObjectRay(ray);
           
            // Drawing satellite trajectory
            trajectories.Add(objRay);
            main3dScene.AddObject(objRay);


            // Clean up redundant trajectory rays
            if(trajectories.Count>=1000)
            {
                var firstRayObj = trajectories[0];
                main3dScene.RemoveObject(firstRayObj);
                trajectories.RemoveAt(0);
            }

            //Adjust sat Position
            satObj.Pos = newPos;
            //Adjust Orientation
            satObj.Orient(dirNormed,Vector3.UnitY);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
          
        }
    }
}