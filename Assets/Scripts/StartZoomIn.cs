using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class StartZoomIn : MonoBehaviour
{
    [SerializeField] private float targetFov;
    [SerializeField] private float zoomDuration;
    [SerializeField] private float zoomOutDuration;

    private float _startFov;
    
    private void Start()
    {
        Camera thisCamera = GetComponent<Camera>();
        _startFov = thisCamera.orthographicSize;
        thisCamera.DOOrthoSize(targetFov, zoomDuration)
            .SetEase(Ease.InOutQuad);
    }

    public void ZoomOut()
    {
        Camera thisCamera = GetComponent<Camera>();
        thisCamera.DOOrthoSize(_startFov, zoomOutDuration)
            .SetEase(Ease.InOutQuad);
    }
}
