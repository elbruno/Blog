using UnityEngine;

public class Line
{
    public const float DefaultLineWidth = 0.001f;

    // Functions
    public Line()
    {
    }
    public Line(Vector3 _p0, Vector3 _p1, Color _c0, Color _c1, float _lineWidth = DefaultLineWidth)
    {
        p0 = _p0;
        p1 = _p1;
        c0 = _c0;
        c1 = _c1;
        lineWidth = _lineWidth;
        isValid = true;
    }
    public bool Set_IfDifferent(Vector3 _p0, Vector3 _p1, Color _c0, Color _c1, float _lineWidth)
    {
        isValid = true;
        if ((p0 != _p0) || (p1 != _p1) || (c0 != _c0) || (c1 != _c1) || (lineWidth != _lineWidth))
        {
            p0 = _p0;
            p1 = _p1;
            c0 = _c0;
            c1 = _c1;
            lineWidth = _lineWidth;
            return true;
        }
        return false;
    }

    // Data
    public Vector3 p0;
    public Vector3 p1;
    public Color c0;
    public Color c1;
    public float lineWidth;
    public bool isValid;
}