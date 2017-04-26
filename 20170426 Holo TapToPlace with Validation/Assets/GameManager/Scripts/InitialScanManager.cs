using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

/// <summary>
/// The InitialScanManager class allows applications to scan the environment for a specified amount of time 
/// and then process the Spatial Mapping Mesh (find planes, remove vertices) after that time has expired.
/// </summary>
public class InitialScanManager : Singleton<InitialScanManager>
{
    [Tooltip("When checked, the SurfaceObserver will stop running after a specified amount of time.")]
    public bool LimitScanningByTime = true;

    [Tooltip("How much time (in seconds) that the SurfaceObserver will run after being started; used when 'Limit Scanning By Time' is checked.")]
    public float ScanTime = 30.0f;

    [Tooltip("Material to use when rendering Spatial Mapping meshes while the observer is running.")]
    public Material DefaultMaterial;

    [Tooltip("Optional Material to use when rendering Spatial Mapping meshes after the observer has been stopped.")]
    public Material SecondaryMaterial;

    [Tooltip("Minimum number of floor planes required in order to exit scanning/processing mode.")]
    public uint MinimumFloors = 1;

    [Tooltip("Minimum number of wall planes required in order to exit scanning/processing mode.")]
    public uint MinimumWalls = 1;

    [Tooltip("Main game player object of the scene")]
    public GameObject Player;

    [Tooltip("Text mesh useed during debug mode")]
    public TextMesh DebugTextMesh;

    private bool _meshesProcessed = false;

    private void Start()
    {
        SpatialMappingManager.Instance.DrawVisualMeshes = true;
        SpatialMappingManager.Instance.SetSurfaceMaterial(DefaultMaterial);
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        Debug("Start mapping");
        PlayerSetActive(false);
    }

    private void Update()
    {
        if (!_meshesProcessed && LimitScanningByTime)
        {
            var secLeft = ScanTime - (Time.time - SpatialMappingManager.Instance.StartTime);
            if (LimitScanningByTime && (Time.time - SpatialMappingManager.Instance.StartTime) < ScanTime)
            {
                Debug(string.Format(@"Scanning environment, {0:##.#} seconds left", secLeft));
            }
            else
            {
                // The user should be done scanning their environment, so start processing the spatial mapping data...
                Debug("Scan done");
                if (SpatialMappingManager.Instance.IsObserverRunning())
                {
                    SpatialMappingManager.Instance.StopObserver();
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;
                }
                CreatePlanes();
                _meshesProcessed = true;
            }
        }
    }

    /// <summary>
    /// Handler for the SurfaceMeshesToPlanes MakePlanesComplete event.
    /// </summary>
    /// <param name="source">Source of the event.</param>
    /// <param name="args">Args for the event.</param>
    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        // Collection of floor and table planes that we can use to set horizontal items on. 
        var horizontal = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table | PlaneTypes.Floor);
        var vertical = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);

        // Check to see if we have enough horizontal planes (minimumFloors) and vertical planes (minimumWalls), to set holograms on in the world.
        if (horizontal.Count >= MinimumFloors && vertical.Count >= MinimumWalls)
        {
            RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);
            SpatialMappingManager.Instance.SetSurfaceMaterial(SecondaryMaterial);

            // We are all done processing the mesh, so we can now initialize a collection of Placeable holograms in the world and use horizontal/vertical planes to set their starting positions. 
            Debug("Start game");
            PlayerSetActive(true);
        }
        else
        {
            Debug("We do not have enough floors/walls to place our holograms on... start scanning");
            SpatialMappingManager.Instance.StartObserver();
            _meshesProcessed = false;
        }
    }

    /// <summary>
    /// Creates planes from the spatial mapping surfaces.
    /// </summary>
    private void CreatePlanes()
    {
        // Generate planes based on the spatial map.
        var surfaceToPlanes = SurfaceMeshesToPlanes.Instance;
        if (surfaceToPlanes != null && surfaceToPlanes.enabled)
        {
            surfaceToPlanes.MakePlanes();
        }
    }

    /// <summary>
    /// Removes triangles from the spatial mapping surfaces.
    /// </summary>
    /// <param name="boundingObjects"></param>
    private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
    {
        var removeVerts = RemoveSurfaceVertices.Instance;
        if (removeVerts != null && removeVerts.enabled)
        {
            removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
        }
    }

    /// <summary>
    /// Called when the GameObject is unloaded.
    /// </summary>
    private void OnDestroy()
    {
        if (SurfaceMeshesToPlanes.Instance != null)
        {
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;
        }
    }

    private void Debug(string message)
    {
        if (DebugTextMesh != null)
            DebugTextMesh.text = message;
    }

    private void PlayerSetActive(bool active)
    {
        if (Player != null)
        {
            Player.SetActive(active);
        }
    }
}