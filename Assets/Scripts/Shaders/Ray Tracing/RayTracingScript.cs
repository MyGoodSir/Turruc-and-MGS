using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingScript : MonoBehaviour
{
    public ComputeShader RayTracingShader;

    public RenderTexture _target;

    public Texture skybox;

    private Camera _camera;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        
    }

    private void SetShaderParams()
    {
        RayTracingShader.SetTexture(0, "_Skybox", skybox);
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraIP", _camera.projectionMatrix.inverse);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParams();
        Render(destination);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if(_target != null)
            {
                _target.Release();
            }
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

            _target.enableRandomWrite = true;
            _target.Create();
        }


    }

    private void Render(RenderTexture dest)
    {
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", _target);
        int xThreadGroups = Mathf.CeilToInt(Screen.width / 8.0f);
        int yThreadGroups = Mathf.CeilToInt(Screen.height / 8.0f);

        RayTracingShader.Dispatch(0, xThreadGroups, yThreadGroups, 1);

        Graphics.Blit(_target, dest);
    }

}
