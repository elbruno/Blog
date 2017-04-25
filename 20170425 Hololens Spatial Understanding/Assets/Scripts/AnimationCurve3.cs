using UnityEngine;

public class AnimationCurve3
{
    public void AddKey(float time, Vector3 pos)
    {
        CurveX.AddKey(time, pos.x);
        CurveY.AddKey(time, pos.y);
        CurveZ.AddKey(time, pos.z);
    }
    public Vector3 Evaluate(float time)
    {
        return new Vector3(CurveX.Evaluate(time), CurveY.Evaluate(time), CurveZ.Evaluate(time));
    }

    public AnimationCurve CurveX = new AnimationCurve();
    public AnimationCurve CurveY = new AnimationCurve();
    public AnimationCurve CurveZ = new AnimationCurve();
}