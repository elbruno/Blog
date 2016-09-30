using Urho;
using Urho.Holographics;
using Urho.Physics;
using Urho.Portable.Holographics;

namespace ElBruno.Urho04
{
    public class ShowHideGazeSample : HoloApplication
    {
        private Node _environmentNode;
        private Material _spatialMaterial;
        private Vector3 _envPositionBeforeManipulations;
        private bool _showGaze = true;
        private SpatialCursor _cursor;

        public ShowHideGazeSample(string pak, bool emulator) : base(pak, emulator) { }

        protected override async void Start()
        {
            base.Start();
            _environmentNode = Scene.CreateChild();
            EnableGestureTapped = true;
            _cursor = Scene.CreateComponent<SpatialCursor>();

            _spatialMaterial = new Material();
            _spatialMaterial.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);
            await StartSpatialMapping(new Vector3(50, 50, 10), 1200, onlyAdd: true);
        }


        public override void OnGestureTapped(GazeInfo gaze)
        {
            _showGaze = !_showGaze;
            _cursor.CursorEnabled = _showGaze;
            base.OnGestureTapped(gaze);
        }

        public override void OnSurfaceAddedOrUpdated(SpatialMeshInfo surface, Model generatedModel)
        {
            bool isNew;
            StaticModel staticModel;
            Node node = _environmentNode.GetChild(surface.SurfaceId, false);
            if (node != null)
            {
                isNew = false;
                staticModel = node.GetComponent<StaticModel>();
            }
            else
            {
                isNew = true;
                node = _environmentNode.CreateChild(surface.SurfaceId);
                staticModel = node.CreateComponent<StaticModel>();
            }

            node.Position = surface.BoundsCenter;
            node.Rotation = surface.BoundsRotation;
            staticModel.Model = generatedModel;

            if (isNew)
            {
                staticModel.SetMaterial(_spatialMaterial);
                var rigidBody = node.CreateComponent<RigidBody>();
                rigidBody.RollingFriction = 0.5f;
                rigidBody.Friction = 0.5f;
                var collisionShape = node.CreateComponent<CollisionShape>();
                collisionShape.SetTriangleMesh(generatedModel, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);
            }
            else
            {
                //Update Collision shape
            }
        }
        public override void OnGestureManipulationStarted()
        {
            _envPositionBeforeManipulations = _environmentNode.Position;
        }
        public override void OnGestureManipulationUpdated(Vector3 relativeHandPosition)
        {
            _environmentNode.Position = relativeHandPosition + _envPositionBeforeManipulations;
        }

    }
}