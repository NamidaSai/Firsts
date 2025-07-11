using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickMorpher : MonoBehaviour
{
    [Header("Morph Requirements")]
    [SerializeField] private int _targetClicks = 10;
    [SerializeField] private float _targetClickHold = 5f;
    [SerializeField] private float _targetProgressMorphStart = 0.1f;

    [Header("Morph Timing")]
    [SerializeField] private float _resetCooldown = 2f;
    [SerializeField] private float _timeToReverse = 2f;
    [SerializeField] private float _winDelay = 2f;

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

        if (_timeSinceLastClick >= _resetCooldown)
        {
            _timesClicked = 0;
        }

        if (_timeSinceLastRelease >= _resetCooldown)
        {
            _timeClickHeld = 0f;
        }
    }


    private void HandleMorphing()
    {
        if (_timesClicked >= _targetClicks * _targetProgressMorphStart)
        {
            HandleClickOnlyMorphing();
        }
        else if (_timeClickHeld >= _targetClickHold * _targetProgressMorphStart)
        {
            HandleClickHoldMorphing();
        }
        else if (_timeSinceLastRelease >= _resetCooldown)
        {
            HandleReverseMorphing();
        }
    }

    private void HandleClickOnlyMorphing()
    {
        float t = Mathf.Clamp01((float)_timesClicked / _targetClicks);
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Square, t);
        SeekWeight = -t;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleClickHoldMorphing()
    {
        float t = Mathf.Clamp01(_timeClickHeld / _targetClickHold);
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerShape.Triangle, t);
        SeekWeight = t;

        if (t >= 1f)
        {
            FinishMorph();
        }
    }

    private void HandleReverseMorphing()
    {
        float morphTime = _timeSinceLastRelease - _resetCooldown;
        float t = Mathf.Clamp01(morphTime / _timeToReverse);
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
        yield return new WaitForSeconds(_winDelay);
        SceneLoader.Instance.LoadNextScene();
    }
}