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
        float t = Mathf.Clamp01((float)_timesClicked / targetClicks);
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Square, t);
        SeekWeight = -t * seekFactor;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleClickHoldMorphing()
    {
        float t = Mathf.Clamp01(_timeClickHeld / targetClickHold);
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Triangle, t);
        SeekWeight = t * seekFactor;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleReverseMorphing()
    {
        float morphTime = _timeSinceLastRelease - resetCooldown;
        float t = Mathf.Clamp01(morphTime / timeToReverse);
        _playerAppearance.SetAppearanceFromTo(_playerAppearance.CurrentShape, PlayerShape.Circle, t);
        SeekWeight = 0f;
    }

    private void FinishMorph()
    {
        _isFinished = true;
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(winDelay);
        SceneLoader.Instance.LoadNextScene();
    }
}