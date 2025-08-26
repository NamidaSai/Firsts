using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickAffordance : MonoBehaviour
{
    [SerializeField] private float punchScaleClick = 2f;
    [SerializeField] private float punchScaleHold = 2f;
    [SerializeField] private float punchDuration = 1f;
    [SerializeField] private int punchVibrato = 10;
    [SerializeField] private float punchElasticity = 1f;
    
    [Header("Object References")] 
    [SerializeField] private ParticleSystem particleSystemClick;
    [SerializeField] private ParticleSystem particleSystemHold;
    
    private bool _isTweening = false;
    private Vector3 _startScale;

    private void Start()
    {
        _startScale = transform.localScale;
        
        Color playerColor = PlayerAppearance.DataInstance.Color;
        ParticleSystem.MainModule clickModule = particleSystemClick.main;
        clickModule.startColor = new Color
        (
            playerColor.r,
            playerColor.g,
            playerColor.b,
            clickModule.startColor.color.a
        );
        ParticleSystem.MainModule holdModule = particleSystemHold.main;
        holdModule.startColor = new Color
        (
            playerColor.r,
            playerColor.g,
            playerColor.b,
            holdModule.startColor.color.a
        );
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            particleSystemHold.Stop();
        }
        
        if (_isTweening) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            particleSystemClick.Play();
            StartPunch(punchScaleClick, 1f);
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            if (!particleSystemHold.isPlaying)
            {
                particleSystemHold.Play();
            }
            StartPunch(punchScaleHold, -1f);
        }
    }

    private void StartPunch(float scaleMagnitude, float scaleDirection)
    {
        _isTweening = true;

        Vector3 targetScale = new Vector3(
            scaleMagnitude * scaleDirection,
            scaleMagnitude * scaleDirection,
            0f
        );

        transform.localScale = _startScale;
        transform
            .DOPunchScale(targetScale, punchDuration, punchVibrato, punchElasticity)
            .OnKill(() => _isTweening = false);
    }
}
