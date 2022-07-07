/// ================================
/// Peter Phillips, 2022
/// ================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CustomFrustum : MonoBehaviour
{
    private static float customFrustumSideLength = 800f;

    private void Awake()
    {
        if (Application.isPlaying)
            Shader.EnableKeyword("ENABLEBENDING");
        else
            Shader.DisableKeyword("ENABLEBENDING");
    }

    private void OnEnable()
    {
        // When enabled, add our custom methods to the render pipeline.
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        // When disabled, remove our custom methods from the render pipeline.
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    public static void OnBeginCameraRendering(ScriptableRenderContext src, Camera cam)
    {
        // Define an orthographic frustum.
        cam.cullingMatrix = Matrix4x4.Ortho(-customFrustumSideLength, customFrustumSideLength, -customFrustumSideLength, customFrustumSideLength, .3f, customFrustumSideLength) * cam.worldToCameraMatrix;
    }
    public static void OnEndCameraRendering(ScriptableRenderContext src, Camera cam)
    {
        // Reset to default frustum.
        cam.ResetCullingMatrix();
    }
}
