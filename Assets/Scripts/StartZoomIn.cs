using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class StartZoomIn : MonoBehaviour
{
    [SerializeField] private float targetFov;
    [SerializeField] private float zoomDuration;
    
    private void Start()
    {
        Camera thisCamera = GetComponent<Camera>();
        thisCamera.DOOrthoSize(targetFov, zoomDuration)
            .SetEase(Ease.InOutQuad);
    }
}
