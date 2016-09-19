using System;
using System.Collections.Generic;
using Urho;
using Urho.Gui;
using Urho.Holographics;
using Urho.Physics;

namespace Physics
{
	public class PhysicsSample : HoloApplication
	{
	    private Node _environmentNode;
	    private Material _spatialMaterial;
	    private Material _bucketMaterial;
	    private bool _surfaceIsValid;
	    private bool _positionIsSelected;
	    private Node _bucketNode;
	    private Node _textNode;

	    private const int MaxBalls = 5;
	    private readonly Queue<Node> _balls = new Queue<Node>();
	    private Text3D _text3D;

	    public PhysicsSample(string pak, bool emulator) : base(pak, emulator) { }

		protected override async void Start()
		{
			base.Start();
			_environmentNode = Scene.CreateChild();

			// Allow tap gesture
			EnableGestureTapped = true;

			// Create a bucket
			_bucketNode = Scene.CreateChild();
			_bucketNode.SetScale(0.15f);
			//var bucketBaseNode = bucketNode.CreateChild();

			// Create instructions
			_textNode = _bucketNode.CreateChild();
			_text3D = _textNode.CreateComponent<Text3D>();
			_text3D.HorizontalAlignment = HorizontalAlignment.Center;
			_text3D.VerticalAlignment = VerticalAlignment.Top;
			_text3D.ViewMask = 0x80000000; //hide from raycasts
			_text3D.Text = @"El Bruno - @elbruno
Place on a horizontal
surface and AirTap";
			_text3D.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
			_text3D.SetColor(Color.White);
			_textNode.Translate(new Vector3(0, 1f, -1.5f));

			// Model and Physics for the bucket
			var bucketModel = _bucketNode.CreateComponent<StaticModel>();
			_bucketMaterial = Material.FromColor(Color.Yellow);
			bucketModel.Model = ResourceCache.GetModel("bucket1.mdl");
			bucketModel.SetMaterial(_bucketMaterial);
			bucketModel.ViewMask = 0x80000000; //hide from raycasts
			var rigidBody = _bucketNode.CreateComponent<RigidBody>();
			var shape = _bucketNode.CreateComponent<CollisionShape>();
			shape.SetTriangleMesh(bucketModel.Model, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);

			// Material for spatial surfaces
			_spatialMaterial = new Material();
			_spatialMaterial.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);

			var allowed = await StartSpatialMapping(new Vector3(50, 50, 10), 1200);
		}

		protected override void OnUpdate(float timeStep)
		{
			if (_positionIsSelected)
				return;

			_textNode.LookAt(LeftCamera.Node.WorldPosition, new Vector3(0, 1, 0), TransformSpace.World);
			_textNode.Rotate(new Quaternion(0, 180, 0), TransformSpace.World);

			var cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);
			var result = Scene.GetComponent<Octree>().RaycastSingle
                (cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);
			if (result != null && result.Count > 0)
			{
				var angle = Vector3.CalculateAngle(new Vector3(0, 1, 0), result[0].Normal);
				_surfaceIsValid = angle < 0.3f; //allow only horizontal surfaces
				_bucketMaterial.SetShaderParameter("MatDiffColor", _surfaceIsValid ? Color.Gray : Color.Red);
				_bucketNode.Position = result[0].Position;
			}
			else
			{
				// no spatial surfaces found
				_surfaceIsValid = false;
				_bucketMaterial.SetShaderParameter("MatDiffColor", Color.Red);
			}
		}

		public override void OnGestureTapped(GazeInfo gaze)
		{
			if (_positionIsSelected)
				ThrowBall();

			if (_surfaceIsValid && !_positionIsSelected)
			{
				_positionIsSelected = true;
                _text3D.Text = @"El Bruno - @elbruno
Now throw some balls !
AirTap to throw";
            }

			base.OnGestureTapped(gaze);
		}

	    private void ThrowBall()
		{
			// Create a ball (will be cloned)
			var ballNode = Scene.CreateChild();
			ballNode.Position = RightCamera.Node.Position;
			ballNode.Rotation = RightCamera.Node.Rotation;
			ballNode.SetScale(0.15f);

			var ball = ballNode.CreateComponent<StaticModel>();
			ball.Model = CoreAssets.Models.Sphere;
			ball.SetMaterial(Material.FromColor(new Color(Randoms.Next(0.2f, 1f), 
                Randoms.Next(0.2f, 1f), Randoms.Next(0.2f, 1f))));
			ball.ViewMask = 0x80000000; //hide from raycasts

			var ballRigidBody = ballNode.CreateComponent<RigidBody>();
			ballRigidBody.Mass = 1f;
			ballRigidBody.RollingFriction = 0.5f;
			var ballShape = ballNode.CreateComponent<CollisionShape>();
			ballShape.SetSphere(1, Vector3.Zero, Quaternion.Identity);

			ball.GetComponent<RigidBody>().SetLinearVelocity
                (RightCamera.Node.Rotation * new Vector3(0f, 0.25f, 1f) * 9 /*velocity*/);

			_balls.Enqueue(ballNode);
			if (_balls.Count > MaxBalls)
				_balls.Dequeue().Remove();
		}

		public override void OnSurfaceAddedOrUpdated(string surfaceId, DateTimeOffset lastUpdateTimeUtc, 
			SpatialVertex[] vertexData, short[] indexData, 
			Vector3 boundsCenter, Quaternion boundsRotation)
		{

			var isNew = false;
			StaticModel staticModel = null;
			var node = _environmentNode.GetChild(surfaceId, false);
			if (node != null)
			{
				isNew = false;
				staticModel = node.GetComponent<StaticModel>();
			}
			else
			{
				isNew = true;
				node = _environmentNode.CreateChild(surfaceId);
				staticModel = node.CreateComponent<StaticModel>();
			}

			node.Position = boundsCenter;
			node.Rotation = boundsRotation;
			var model = CreateModelFromVertexData(vertexData, indexData);
			staticModel.Model = model;

			if (isNew)
				staticModel.SetMaterial(_spatialMaterial);

			var rigidBody = node.CreateComponent<RigidBody>();
			rigidBody.RollingFriction = 0.5f;
			rigidBody.Friction = 0.5f;
			var collisionShape = node.CreateComponent<CollisionShape>();
			collisionShape.SetTriangleMesh(model, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);
		}
	}
}