using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickMorpher : MonoBehaviour
{
    [Header("Morph Requirements")]
    [SerializeField] private int targetClicks = 10;
    [SerializeField] private float targetClickHold = 5f;
    [SerializeField] private float targetProgressMorphStart = 0.1f;

    [Header("Morph Timing")]
    [SerializeField] private float resetCooldown = 2f;
    [SerializeField] private float timeToReverse = 2f;
    [SerializeField] private float winDelay = 2f;

    [Header("Boid Settings")] 
    [SerializeField] private float seekFactor = 1f;

    public float SeekWeight { get; private set; } = 0f;

    private int _timesClicked = 0;
    private float _timeClickHeld = 0f;
    private float _timeSinceLastRelease = 0f;
    private float _timeSinceLastClick = 0f;
    private bool _isFinished = false;
    private PlayerShape? _lastMorphTarget = null;
    private float _lastMorphT = 0f;

    private PlayerAppearance _playerAppearance;

    private void Start()
    {
        _playerAppearance = FindAnyObjectByType<PlayerAppearance>();
    }

    private void Update()
    {
        if (_isFinished) return;

        HandleInput();
        HandleMorphing();
    }

    private void HandleInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            _timeSinceLastRelease = 0f;
            _timesClicked++;
            _timeSinceLastClick = 0f;
        }

        if (mouse.leftButton.isPressed)
        {
            _timeClickHeld += Time.deltaTime;
            return;
        }

        _timeSinceLastClick += Time.deltaTime;
        _timeSinceLastRelease += Time.deltaTime;

        if (_timeSinceLastClick >= resetCooldown)
        {
            _timesClicked = 0;
        }

        if (_timeSinceLastRelease >= resetCooldown)
        {
            _timeClickHeld = 0f;
        }
    }


    private void HandleMorphing()
    {
        if (_timesClicked >= targetClicks * targetProgressMorphStart)
        {
            HandleClickOnlyMorphing();
        }
        else if (_timeClickHeld >= targetClickHold * targetProgressMorphStart)
        {
            HandleClickHoldMorphing();
        }
        else if (_timeSinceLastRelease >= resetCooldown)
        {
            HandleReverseMorphing();
        }
    }

    private void HandleClickOnlyMorphing()
    {
        float progress = _timesClicked - targetClicks * targetProgressMorphStart;
        float duration = targetClicks - targetClicks * targetProgressMorphStart;
        float t = Mathf.Clamp01(progress / duration);
        
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Square, t, true);
        _lastMorphTarget = PlayerShape.Square;
        _lastMorphT = t;
        
        SeekWeight = -t * seekFactor;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleClickHoldMorphing()
    {
        float progress = _timeClickHeld - targetClickHold * targetProgressMorphStart;
        float duration = targetClickHold - targetClickHold * targetProgressMorphStart;
        float t = Mathf.Clamp01(progress / duration);
        
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Triangle, t, true);
        _lastMorphTarget = PlayerShape.Triangle;
        _lastMorphT = t;
        
        SeekWeight = t * seekFactor;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleReverseMorphing()
    {
        if (_lastMorphTarget is null or PlayerShape.Circle) return;

        float progress = _timeSinceLastRelease - resetCooldown;
        float duration = timeToReverse;
        float t = Mathf.Clamp01(progress / duration);
        float adjustedT = Mathf.Lerp(1-_lastMorphT, 1f, t);
        
        _playerAppearance.SetAppearanceFromTo(_lastMorphTarget.Value, PlayerShape.Circle, adjustedT);

        SeekWeight = 0f;

        if (t >= 1f)
        {
            _lastMorphTarget = null; // Reset after full reverse
        }
    }

    private void FinishMorph()
    {
        _isFinished = true;
        GetComponent<ClickAffordance>().enabled = false;
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        FindAnyObjectByType<StartZoomIn>().ZoomOut();
        yield return new WaitForSeconds(winDelay);
        SceneLoader.Instance.LoadNextScene();
    }
}