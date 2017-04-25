using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.UI;

public class GameStartScanner : MonoBehaviour
{
    public float MinAreaForStats = 5.0f;
    public float MinAreaForComplete = 20.0f;
    public float MinHorizAreaForComplete = 10.0f;
    public float MinWallAreaForComplete = 5.0f;
    public TextMesh DebugDisplay;

    public static bool SpatialMappingScanCompleted;

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
            SpatialMappingScanCompleted = true;
        }
    }

    void Update()
    {
        if (DebugDisplay != null && !SpatialMappingScanCompleted)
            DebugDisplay.text = PrimaryText;

        if (SpatialMappingScanCompleted || !DoesScanMeetMinBarForCompletion) return;

        SpatialUnderstanding.Instance.RequestFinishScan();
        SpatialMappingScanCompleted = true;

        // hide mesh
        var customMesh = SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>();
        customMesh.DrawProcessedMesh = false;
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
        ScannerAnalyzer.AnalyzerEnabled = true;
    }

    public string PrimaryText
    {
        get
        {
            var returnText = "";

            // Scan state
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                switch (SpatialUnderstanding.Instance.ScanState)
                {
                    case SpatialUnderstanding.ScanStates.Scanning:
                        break;
                    case SpatialUnderstanding.ScanStates.Finishing:
                        returnText = "Finalizing scan (please wait)";
                        break;
                    case SpatialUnderstanding.ScanStates.Done:
                        returnText = "Scan complete - Now go back to work!";
                        break;
                    case SpatialUnderstanding.ScanStates.None:
                        break;
                    case SpatialUnderstanding.ScanStates.ReadyToScan:
                        break;
                }
                returnText += Environment.NewLine + @"ScanState = " + SpatialUnderstanding.Instance.ScanState;
            }

            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            if (stats != null && stats.TotalSurfaceArea > MinAreaForStats)
            {
                var subDisplayText = string.Format("totalArea={0:0.0}, horiz={1:0.0}, wall={2:0.0}", stats.TotalSurfaceArea, stats.HorizSurfaceArea, stats.WallSurfaceArea);
                subDisplayText += string.Format("\nnumFloorCells={0}, numCeilingCells={1}, numPlatformCells={2}", stats.NumFloor, stats.NumCeiling, stats.NumPlatform);
                subDisplayText += string.Format("\npaintMode={0}, seenCells={1}, notSeen={2}", stats.CellCount_IsPaintMode, stats.CellCount_IsSeenQualtiy_Seen + stats.CellCount_IsSeenQualtiy_Good, stats.CellCount_IsSeenQualtiy_None);
                returnText += Environment.NewLine + subDisplayText;
            }
            return returnText;
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
