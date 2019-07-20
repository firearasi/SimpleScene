// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

using SimpleScene;
using SimpleScene.Demos;

namespace Simulation
{
	partial class Simulation : TestBenchBootstrap
	{
        SSObject satObj;
        protected SSpaceMissileVisualParameters satObjMissileParams;

        protected override void setupHUD()
		{
			base.setupHUD();

			// HUD text....
			//var testDisplay = new SSObject2DSurface_AGGText();
			//testDisplay.backgroundColor = Color.Transparent;
			//testDisplay.alphaBlendingEnabled = true;
			//testDisplay.Label = "TEST AGG";
			//hud2dScene.AddObject(testDisplay);
			//testDisplay.Pos = new Vector3(50f, 100f, 0f);
			//testDisplay.Scale = new Vector3(1.0f);
		}

		protected override void setupScene()
		{
			base.setupScene();

			var mesh = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./sats/cloudsat/cloudsat_v19.obj");
           //var mesh = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./earth4/Model/Globe.obj");
            // add drone
           satObj = new SSObjectMesh(mesh);
			main3dScene.AddObject(satObj);
            satObj.renderState.lighted = true;
            //satObj.EulerDegAngleOrient(-10.0f,0.0f);
            satObj.Orient(Vector3.UnitX, Vector3.UnitY);
            satObj.Pos = new OpenTK.Vector3(0f, 0f, -20f);
            satObj.Scale = new OpenTK.Vector3(1f, 1f, 1f);

			satObj.Name = "drone 1";

            satObjMissileParams = new SSpaceMissileVisualParameters();
            satObjMissileParams.ejectionMaxRotationVel = 0.05f;
            satObjMissileParams.ejectionVelocity = 30f;
            satObjMissileParams.pursuitActivationTime = 0.1f;
            satObjMissileParams.ejectionSmokeDuration = 0.5f;
            satObjMissileParams.ejectionSmokeSizeMax = 5f;

            // add second drone
#if true
            SSObject earthObj = new SSObjectMesh(
				SSAssetManager.GetInstance<SSMesh_wfOBJ>("./earth5/Earth.obj"));
			main3dScene.AddObject(earthObj);
			earthObj.renderState.lighted = true;
			//drone2Obj.EulerDegAngleOrient(0f, 0f);
			earthObj.Pos = new OpenTK.Vector3(0f, 0f, 0f);
            earthObj.Scale = new OpenTK.Vector3(6f, 6f, 6f);
			earthObj.Name = "earth";
#endif
            // instanced asteroid ring
            if (false)
			{
				var roidmesh = SSAssetManager.GetInstance<SSMesh_wfOBJ> ("simpleasteroid/asteroid.obj");
				var ringGen = new BodiesRingGenerator (
					120f, 50f,
					Vector3.Zero, Vector3.UnitY, 250f,
					0f, (float)Math.PI*2f,
					1f, 3f, 1f, 0.5f
					//10f, 30f, 1f, 20f
				);

				var ringEmitter = new SSBodiesFieldEmitter (ringGen);
				ringEmitter.particlesPerEmission = 10000;
				//ringEmitter.ParticlesPerEmission = 10;

				var ps = new SSParticleSystemData(10000);
				ps.addEmitter(ringEmitter);
				Console.WriteLine ("Packing 10k asteroids into a ring. This may take a second...");
				ps.emitAll();
				asteroidRingRenderer = new SSInstancedMeshRenderer (ps, roidmesh, BufferUsageHint.StaticDraw);
				asteroidRingRenderer.simulateOnUpdate = false;
				asteroidRingRenderer.Name = "instanced asteroid renderer";
				asteroidRingRenderer.renderState.castsShadow = true;
				asteroidRingRenderer.renderState.receivesShadows = true;
                asteroidRingRenderer.selectable = true;
                asteroidRingRenderer.useBVHForIntersections = true;
				main3dScene.AddObject (asteroidRingRenderer);
			}

			// particle system test
			// particle systems should be drawn last (if it requires alpha blending)
			if (false)
			{
				// setup an emitter
				var box = new ParticlesSphereGenerator (new Vector3(0f, 0f, 0f), 10f);
				var emitter = new SSParticlesFieldEmitter (box);
				//emitter.EmissionDelay = 5f;
				emitter.particlesPerEmission = 1;
				emitter.emissionInterval = 0.5f;
				emitter.life = 1000f;
				emitter.colorOffsetComponentMin = new Color4 (0.5f, 0.5f, 0.5f, 1f);
				emitter.colorOffsetComponentMax = new Color4 (1f, 1f, 1f, 1f);
				emitter.velocityComponentMax = new Vector3 (.3f);
				emitter.velocityComponentMin = new Vector3 (-.3f);
				emitter.angularVelocityMin = new Vector3 (-0.5f);
				emitter.angularVelocityMax = new Vector3 (0.5f);
				emitter.dragMin = 0f;
				emitter.dragMax = .1f;
				RectangleF[] uvRects = new RectangleF[18*6];
				float tileWidth = 1f / 18f;
				float tileHeight = 1f / 6f;
				for (int r = 0; r < 6; ++r) {
					for (int c = 0; c < 18; ++c) {
						uvRects [r*18 + c] = new RectangleF (tileWidth * (float)r, 
							tileHeight * (float)c,
							tileWidth, 
							tileHeight);
					}
				}
				emitter.spriteRectangles = uvRects;

				var periodicExplosiveForce = new SSPeriodicExplosiveForceEffector ();
				periodicExplosiveForce.effectInterval = 3f;
				periodicExplosiveForce.explosiveForceMin = 1000f;
				periodicExplosiveForce.explosiveForceMax = 5000f;
				periodicExplosiveForce.effectDelay = 5f;
				periodicExplosiveForce.centerMin = new Vector3 (-30f, -30f, -30f);
				periodicExplosiveForce.centerMax = new Vector3 (+30f, +30f, +30f);
				//periodicExplosiveForce.Center = new Vector3 (10f);

				// make a particle system
				SSParticleSystemData cubesPs = new SSParticleSystemData (1000);
				cubesPs.addEmitter(emitter);
				cubesPs.addEffector (periodicExplosiveForce);

				// test a renderer
				var tex = SSAssetManager.GetInstance<SSTextureWithAlpha>("elements.png");
				var cubesRenderer = new SSInstancedMeshRenderer (cubesPs, SSTexturedNormalCube.Instance);
				cubesRenderer.Pos = new Vector3 (0f, 0f, -30f);
				cubesRenderer.alphaBlendingEnabled = false;
				cubesRenderer.Name = "cube particle renderer";
				cubesRenderer.renderState.castsShadow = true;
				cubesRenderer.renderState.receivesShadows = true;
				cubesRenderer.textureMaterial = new SSTextureMaterial() {
					ambientTex = tex
				};
                cubesRenderer.selectable = true;
				main3dScene.AddObject(cubesRenderer);
				//cubesRenderer.renderState.visible = false;

				// test explositons
				//if (false)
				{
					SExplosionRenderer aer = new SExplosionRenderer (100);
					aer.Pos = cubesRenderer.Pos;
                    alpha3dScene.AddObject (aer);

					periodicExplosiveForce.explosionEventHandlers += (pos, force) => { 
						aer.showExplosion(pos, force/periodicExplosiveForce.explosiveForceMin*1.5f); 
					};
				}
			}
		}

     

    }
}