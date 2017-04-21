using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.UI;

public class GameStartScanner : MonoBehaviour
{
    public float MinAreaForComplete = 30.0f;
    public float MinHorizAreaForComplete = 20.0f;
    public float MinWallAreaForComplete = 5.0f;
    private bool _scanComplete = false;
    public TextMesh DebugDisplay;

    void Start()
    {
        SpatialUnderstanding.Instance.ScanStateChanged += Instance_ScanStateChanged;
        SpatialUnderstanding.Instance.RequestBeginScanning();
    }

    private void Instance_ScanStateChanged()
    {
        if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            && SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            _scanComplete = true;
        }
    }

    void Update()
    {
        if (DebugDisplay != null)
            DebugDisplay.text = PrimaryText;

        if (_scanComplete || !DoesScanMeetMinBarForCompletion) return;

        SpatialUnderstanding.Instance.RequestFinishScan();
        _scanComplete = true;

        // hide mesh
        var customMesh = SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>();
        customMesh.DrawProcessedMesh = false;
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
    }

    public string PrimaryText
    {
        get
        {
            // Scan state
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                switch (SpatialUnderstanding.Instance.ScanState)
                {
                    case SpatialUnderstanding.ScanStates.Scanning:
                        // Get the scan stats
                        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                        {
                            return "playspace stats query failed";
                        }

                        // The stats tell us if we could potentially finish
                        if (DoesScanMeetMinBarForCompletion)
                        {
                            return "When ready, air tap to finalize your playspace";
                        }
                        return @"Bruno it's time to walk ! 
Move around and scan in your playspace";
                    case SpatialUnderstanding.ScanStates.Finishing:
                        return "Finalizing scan (please wait)";
                    case SpatialUnderstanding.ScanStates.Done:
                        return "Scan complete - Now go back to work!";
                    default:
                        return @"I'm working, 
ScanState = " + SpatialUnderstanding.Instance.ScanState.ToString();
                }
            }
            return "";
        }
    }


    public bool DoesScanMeetMinBarForCompletion
    {
        get
        {
            // Only allow this when we are actually scanning
            if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
                (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                return false;
            }

            // Query the current playspace stats
            var statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
            if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
            {
                return false;
            }
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

            // Check our preset requirements
            if ((stats.TotalSurfaceArea > MinAreaForComplete) ||
                (stats.HorizSurfaceArea > MinHorizAreaForComplete) ||
                (stats.WallSurfaceArea > MinWallAreaForComplete))
            {
                return true;
            }
            return false;
        }
    }

}
