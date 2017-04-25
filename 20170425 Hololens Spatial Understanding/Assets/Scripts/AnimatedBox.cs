using System;
using HoloToolkit.Unity;
using UnityEngine;

public class AnimatedBox
{
    public const float InitialPositionForwardMaxDistance = 2.0f;
    public const float AnimationTime = 2.5f;
    public const float DelayPerItem = 0.35f;

    public AnimatedBox(
        float timeDelay,
        Vector3 center,
        Quaternion rotation,
        Color color,
        Vector3 halfSize,
        float lineWidth = Line.DefaultLineWidth * 3.0f)
    {
        TimeDelay = timeDelay;
        Center = center;
        Rotation = rotation;
        Color = color;
        HalfSize = halfSize;
        LineWidth = lineWidth;

        // If no time delay, go ahead and lock the animation now
        if (TimeDelay <= 0.0f)
        {
            SetupAnimation();
        }
    }

    public bool Update(float deltaTime)
    {
        Time += deltaTime;

        // Delay animation setup until after the time delay
        if (!IsAnimationSetup &&
            (Time >= TimeDelay))
        {
            SetupAnimation();
        }

        return (Time >= TimeDelay);
    }

    private void SetupAnimation()
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        // Calc the forward distance for the animation start point
        Vector3 rayPos = Camera.main.transform.position;
        Vector3 rayVec = Camera.main.transform.forward * InitialPositionForwardMaxDistance;
        IntPtr raycastResultPtr = HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();
        HoloToolkit.Unity.SpatialUnderstandingDll.Imports.PlayspaceRaycast(
            rayPos.x, rayPos.y, rayPos.z, rayVec.x, rayVec.y, rayVec.z,
            raycastResultPtr);
        SpatialUnderstandingDll.Imports.RaycastResult rayCastResult = HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();
        Vector3 animOrigin = (rayCastResult.SurfaceType != HoloToolkit.Unity.SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid) ?
            rayPos + rayVec.normalized * Mathf.Max((rayCastResult.IntersectPoint - rayPos).magnitude - 0.3f, 0.0f) :
            rayPos + rayVec * InitialPositionForwardMaxDistance;

        // Create the animation (starting it on the ground in front of the camera
        SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
        SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();
        AnimPosition.AddKey(TimeDelay + 0.0f, new Vector3(animOrigin.x, alignment.FloorYValue, animOrigin.z));
        AnimPosition.AddKey(TimeDelay + AnimationTime * 0.5f, new Vector3(animOrigin.x, alignment.FloorYValue + 1.25f, animOrigin.z));
        AnimPosition.AddKey(TimeDelay + AnimationTime * 0.6f, new Vector3(animOrigin.x, alignment.FloorYValue + 1.0f, animOrigin.z));
        AnimPosition.AddKey(TimeDelay + AnimationTime * 0.95f, Center);
        AnimPosition.AddKey(TimeDelay + AnimationTime * 1.0f, Center);

        AnimScale.AddKey(TimeDelay + 0.0f, 0.0f);
        AnimScale.AddKey(TimeDelay + AnimationTime * 0.5f, 0.5f);
        AnimScale.AddKey(TimeDelay + AnimationTime * 0.8f, 1.0f);
        AnimScale.AddKey(TimeDelay + AnimationTime * 1.0f, 1.0f);

        AnimRotation.AddKey(TimeDelay + 0.0f, -1.5f);
        AnimRotation.AddKey(TimeDelay + AnimationTime * 0.2f, -0.5f);
        AnimRotation.AddKey(TimeDelay + AnimationTime * 0.9f, 0.0f);
        AnimRotation.AddKey(TimeDelay + AnimationTime * 1.0f, 0.0f);

        IsAnimationSetup = true;
    }
    public bool IsAnimationComplete { get { return IsAnimationSetup && (Time >= (AnimatedBox.AnimationTime + TimeDelay)); } }

    public Vector3 Center;
    public Quaternion Rotation;
    public Color Color;
    public Vector3 HalfSize;
    public float LineWidth;

    public bool IsAnimationSetup;
    public float Time;
    public float TimeDelay;
    public AnimationCurve AnimScale = new AnimationCurve();
    public AnimationCurve3 AnimPosition = new AnimationCurve3();
    public AnimationCurve AnimRotation = new AnimationCurve();

}