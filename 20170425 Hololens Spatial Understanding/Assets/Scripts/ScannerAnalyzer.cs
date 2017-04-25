using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;

public class ScannerAnalyzer : MonoBehaviour, IInputClickHandler
{
    const int QueryResultMaxCount = 512;
    const int DisplayResultMaxCount = 32;

    private List<AnimatedBox> _lineBoxList = new List<AnimatedBox>();
    private SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];
    private LineData _lineData = new LineData();
    private string _spaceQueryDescription;
    private bool _searching = false;

    public TextMesh DebugDisplay;
    public static bool AnalyzerEnabled;
    public Material MaterialLine;

    void Update()
    {
        if (!AnalyzerEnabled || _searching) return;

        if (DebugDisplay != null)
            DebugDisplay.text = _spaceQueryDescription;

        LineDraw_Begin();
        var needsUpdate = false;
        needsUpdate |= Draw_LineBoxList();
        LineDraw_End(needsUpdate);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }
        _searching = true;
        var minWidthOfWallSpace = 1f;
        var minHeightAboveFloor = 1f;

        // Query
        var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
        var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(
            minWidthOfWallSpace, minHeightAboveFloor,
            _resultsTopology.Length, resultsTopologyPtr);

        var visDesc = "Find Positions On Floor";
        var boxFullDims = new Vector3(minWidthOfWallSpace, 0.025f, minHeightAboveFloor);
        var color = Color.red;
        ClearGeometry();

        // Add the line boxes (we may have more results than boxes - pick evenly across the results in that case)
        var lineInc = Mathf.CeilToInt((float)locationCount / (float)DisplayResultMaxCount);
        var boxesDisplayed = 0;
        for (var i = 0; i < locationCount; i += lineInc)
        {
            var timeDelay = (float)_lineBoxList.Count * AnimatedBox.DelayPerItem;
            _lineBoxList.Add(
                new AnimatedBox(
                    timeDelay,
                    _resultsTopology[i].position,
                    Quaternion.LookRotation(_resultsTopology[i].normal, Vector3.up),
                    color,
                    boxFullDims * 0.5f)
            );
            ++boxesDisplayed;
        }

        // Vis description
        if (locationCount == boxesDisplayed)
        {
            _spaceQueryDescription = string.Format("{0} ({1})", visDesc, locationCount);
        }
        else
        {
            _spaceQueryDescription = string.Format("{0} (found={1}, displayed={2})", visDesc, locationCount, boxesDisplayed);
        }
        _searching = false;
    }

    #region Line and Box Drawing

    protected void LineDraw_Begin()
    {
        _lineData.LineIndex = 0;
        for (var i = 0; i < _lineData.Lines.Count; ++i)
        {
            _lineData.Lines[i].isValid = false;
        }
    }

    private bool Draw_LineBoxList()
    {
        var needsUpdate = false;
        for (var i = 0; i < _lineBoxList.Count; ++i)
        {
            needsUpdate |= Draw_AnimatedBox(_lineBoxList[i]);
        }
        return needsUpdate;
    }

    protected void LineDraw_End(bool needsUpdate)
    {
        if (_lineData == null)
        {
            return;
        }

        // Check if we have any not dirty
        var i = 0;
        while (i < _lineData.Lines.Count)
        {
            if (!_lineData.Lines[i].isValid)
            {
                needsUpdate = true;
                _lineData.Lines.RemoveAt(i);
                continue;
            }
            ++i;
        }

        // Do the update (if needed)
        if (needsUpdate)
        {
            Lines_LineDataToMesh();
        }
    }

    private void Lines_LineDataToMesh()
    {
        // Alloc them up
        var verts = new Vector3[_lineData.Lines.Count * 8];
        var tris = new int[_lineData.Lines.Count * 12 * 3];
        var colors = new Color[verts.Length];

        // Build the data
        for (var i = 0; i < _lineData.Lines.Count; ++i)
        {
            // Base index calcs
            var vert = i * 8;
            var v0 = vert;
            var tri = i * 12 * 3;

            // Setup
            var dirUnit = (_lineData.Lines[i].p1 - _lineData.Lines[i].p0).normalized;
            var normX = Vector3.Cross((Mathf.Abs(dirUnit.y) >= 0.99f) ? Vector3.right : Vector3.up, dirUnit).normalized;
            var normy = Vector3.Cross(normX, dirUnit);

            // Verts
            verts[vert] = _lineData.Lines[i].p0 + normX * _lineData.Lines[i].lineWidth + normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c0; ++vert;
            verts[vert] = _lineData.Lines[i].p0 - normX * _lineData.Lines[i].lineWidth + normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c0; ++vert;
            verts[vert] = _lineData.Lines[i].p0 - normX * _lineData.Lines[i].lineWidth - normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c0; ++vert;
            verts[vert] = _lineData.Lines[i].p0 + normX * _lineData.Lines[i].lineWidth - normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c0; ++vert;

            verts[vert] = _lineData.Lines[i].p1 + normX * _lineData.Lines[i].lineWidth + normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c1; ++vert;
            verts[vert] = _lineData.Lines[i].p1 - normX * _lineData.Lines[i].lineWidth + normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c1; ++vert;
            verts[vert] = _lineData.Lines[i].p1 - normX * _lineData.Lines[i].lineWidth - normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c1; ++vert;
            verts[vert] = _lineData.Lines[i].p1 + normX * _lineData.Lines[i].lineWidth - normy * _lineData.Lines[i].lineWidth; colors[vert] = _lineData.Lines[i].c1; ++vert;

            // Indices
            tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 5); tris[tri + 2] = (v0 + 4); tri += 3;
            tris[tri + 0] = (v0 + 1); tris[tri + 1] = (v0 + 5); tris[tri + 2] = (v0 + 0); tri += 3;

            tris[tri + 0] = (v0 + 1); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 5); tri += 3;
            tris[tri + 0] = (v0 + 2); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 1); tri += 3;

            tris[tri + 0] = (v0 + 2); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 6); tri += 3;
            tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 2); tri += 3;

            tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 4); tri += 3;
            tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 4); tris[tri + 2] = (v0 + 0); tri += 3;

            tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 3); tris[tri + 2] = (v0 + 2); tri += 3;
            tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 2); tris[tri + 2] = (v0 + 1); tri += 3;

            tris[tri + 0] = (v0 + 5); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 7); tri += 3;
            tris[tri + 0] = (v0 + 5); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 4); tri += 3;
        }

        // Create up the components
        if (_lineData.Renderer == null)
        {
            _lineData.Renderer = gameObject.AddComponent<MeshRenderer>() ??
                                 gameObject.GetComponent<Renderer>() as MeshRenderer;
            _lineData.Renderer.material = MaterialLine;
        }

        if (_lineData.Filter == null)
        {
            _lineData.Filter = gameObject.AddComponent<MeshFilter>() ?? gameObject.GetComponent<MeshFilter>();
        }
        // Create or clear the mesh
        Mesh mesh;
        if (_lineData.Filter.mesh != null)
        {
            mesh = _lineData.Filter.mesh;
            mesh.Clear();
        }
        else
        {
            mesh = new Mesh { name = "Lines_LineDataToMesh" };
        }

        // Set them into the mesh
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        _lineData.Filter.mesh = mesh;

        // If no tris, hide it
        _lineData.Renderer.enabled = (_lineData.Lines.Count != 0);

        // Line index reset
        _lineData.LineIndex = 0;
    }

    protected bool Draw_AnimatedBox(AnimatedBox box)
    {
        // Update the time
        if (!box.Update(Time.deltaTime))
        {
            return false;
        }
        if (box.IsAnimationComplete)
        {
            // Animation is done, just pass through
            return Draw_Box(box.Center, box.Rotation, box.Color, box.HalfSize, box.LineWidth);
        }

        // Draw it using the current anim state
        return Draw_Box(
            box.AnimPosition.Evaluate(box.Time),
            box.Rotation * Quaternion.AngleAxis(360.0f * box.AnimRotation.Evaluate(box.Time), Vector3.up),
            box.Color,
            box.HalfSize * box.AnimScale.Evaluate(box.Time),
            box.LineWidth);
    }

    protected bool Draw_Box(Vector3 center, Quaternion rotation, Color color, Vector3 halfSize, float lineWidth = Line.DefaultLineWidth)
    {
        var needsUpdate = false;

        var basisX = rotation * Vector3.right;
        var basisY = rotation * Vector3.up;
        var basisZ = rotation * Vector3.forward;
        Vector3[] pts =
        {
            center + basisX * halfSize.x + basisY * halfSize.y + basisZ * halfSize.z,
            center + basisX * halfSize.x + basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x + basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x + basisY * halfSize.y + basisZ * halfSize.z,

            center + basisX * halfSize.x - basisY * halfSize.y + basisZ * halfSize.z,
            center + basisX * halfSize.x - basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x - basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x - basisY * halfSize.y + basisZ * halfSize.z
        };

        // Bottom
        needsUpdate |= Draw_Line(pts[0], pts[1], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[1], pts[2], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[2], pts[3], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[3], pts[0], color, color, lineWidth);

        // Top
        needsUpdate |= Draw_Line(pts[4], pts[5], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[5], pts[6], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[6], pts[7], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[7], pts[4], color, color, lineWidth);

        // Vertical lines
        needsUpdate |= Draw_Line(pts[0], pts[4], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[1], pts[5], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[2], pts[6], color, color, lineWidth);
        needsUpdate |= Draw_Line(pts[3], pts[7], color, color, lineWidth);

        return needsUpdate;
    }

    protected bool Draw_Line(Vector3 start, Vector3 end, Color colorStart, Color colorEnd, float lineWidth = Line.DefaultLineWidth)
    {
        // Create up a new line (unless it's already created)
        while (_lineData.LineIndex >= _lineData.Lines.Count)
        {
            _lineData.Lines.Add(new Line());
        }

        // Set it
        var needsUpdate = _lineData.Lines[_lineData.LineIndex].Set_IfDifferent(transform.InverseTransformPoint(start), transform.InverseTransformPoint(end), colorStart, colorEnd, lineWidth);

        // Inc out count
        ++_lineData.LineIndex;

        return needsUpdate;
    }

    public void ClearGeometry(bool clearAll = true)
    {
        _lineBoxList = new List<AnimatedBox>();
    }
    #endregion

}