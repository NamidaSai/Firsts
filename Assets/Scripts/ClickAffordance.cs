using System;
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
    
    private bool _isTweening = false;
    private Vector3 _startScale;

    private void Start()
    {
        _startScale = transform.localScale;
    }

    private void Update()
    {
        if (_isTweening) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartPunch(punchScaleClick, 1f);
        }
        else if (Mouse.current.leftButton.isPressed)
        {
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
