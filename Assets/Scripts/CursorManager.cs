using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private float punchScale = 2f;
    [SerializeField] private float punchDuration = 1f;
    [SerializeField] private int punchVibrato = 10;
    [SerializeField] private float punchElasticity = 1f;
    
    private bool _isTweening = false;
    private Vector3 _startScale;

    private void Start()
    {
        _startScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (!Cursor.visible) { return; }
        if (_isTweening) { return; }
        
        _isTweening = true;
        
        Vector3 targetScale = new Vector3(punchScale, punchScale, 0f);
        transform.localScale = _startScale;
        transform
            .DOPunchScale(targetScale, punchDuration, punchVibrato, punchElasticity)
            .OnKill(() => _isTweening = false);
    }
    
    private void OnMouseDown()
    {
        if (!Cursor.visible) { return; }
        
        Cursor.visible = false;
        GetComponent<FollowMouse>().enabled = true;
        
        Vector3 targetScale = new Vector3(-punchScale, -punchScale, 0f);
        transform.localScale = _startScale;
        transform.DOPunchScale(targetScale, punchDuration, punchVibrato, punchElasticity);
    }

    private void Update()
    {
        if (Cursor.visible) { return; }
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.visible = true;
            GetComponent<FollowMouse>().enabled = false;
        }
    }
}