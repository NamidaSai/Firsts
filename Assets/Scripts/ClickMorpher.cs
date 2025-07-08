using UnityEngine;
using UnityEngine.InputSystem;

public class ClickMorpher : MonoBehaviour
{
    [SerializeField] private int _targetClicks = 10;
    [SerializeField] private float _targetClickHold = 5f;
    [SerializeField] private float _resetCooldown = 2f;
    [SerializeField] private float _targetProgressMorphStart = 0.1f;
    [SerializeField] private PlayerData _holdPlayerDataTarget;
    [SerializeField] private PlayerData _clickPlayerDataTarget;
    [SerializeField] private float _timeToReverse = 2f;

    public float SeekWeight { private set; get; } = 0f;

    private int _timesClicked = 0;
    private float _timeClickHeld = 0f;
    private float _timeSinceLastRelease = 0f;
    private float _timeSinceLastClick = 0f;
    private bool _isFinished = false;

    private PlayerData _startPlayerData;
    private PlayerData _currentPlayerData;
    private PlayerAppearance _playerAppearance;

    private void Start()
    {
        _startPlayerData = new PlayerData(PlayerAppearance.DataInstance.Color);
        _currentPlayerData = new PlayerData(PlayerAppearance.DataInstance.Color);
        _playerAppearance = FindAnyObjectByType<PlayerAppearance>();
    }

    private void Update()
    {
        if (_isFinished) { return; }
        
        HandleMorphing();
        
        // Click and Hold
        if (Mouse.current.leftButton.isPressed && _timesClicked == 0)
        {
            _timeClickHeld += Time.deltaTime;
            return;
        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _timeSinceLastRelease = 0f;
            _timesClicked++;
            _timeSinceLastClick = 0f;
        }
        
        _timeSinceLastClick += Time.deltaTime;
        if (_timeSinceLastClick >= _resetCooldown)
        {
            _timesClicked = 0;
        }
       
        _timeSinceLastRelease += Time.deltaTime;
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
            return;
        }
        
        if (_timeClickHeld >= _targetClickHold * _targetProgressMorphStart)
        {
            HandleClickHoldMorphing();
            return;
        }
        
        HandleReverseMorphing();
    }

    private void HandleClickOnlyMorphing()
    {
        float lerpFactor = Mathf.Clamp01((float)_timesClicked / _targetClicks);
        _currentPlayerData.Color = Color.Lerp(_currentPlayerData.Color, _clickPlayerDataTarget.Color, lerpFactor);
        _playerAppearance.SetAppearanceForPlayerData(_currentPlayerData);

        SeekWeight = -lerpFactor;
        
        if (_timesClicked >= _targetClicks)
        {
            FinishMorph();
        }
    }

    private void HandleClickHoldMorphing()
    {
        float lerpFactor = Mathf.Clamp01(_timeClickHeld / _targetClickHold);
        _currentPlayerData.Color = Color.Lerp(_currentPlayerData.Color, _holdPlayerDataTarget.Color, lerpFactor);
        _playerAppearance.SetAppearanceForPlayerData(_currentPlayerData);

        SeekWeight = lerpFactor;
        
        if (_timeClickHeld >= _targetClickHold)
        {
            FinishMorph();
        }
    }
    
    private void HandleReverseMorphing()
    {
        float lerpFactor = Mathf.Clamp01(_timeSinceLastRelease / (_resetCooldown + _timeToReverse));
        _currentPlayerData.Color = Color.Lerp(_currentPlayerData.Color, _startPlayerData.Color, lerpFactor);
        _playerAppearance.SetAppearanceForPlayerData(_currentPlayerData);
        SeekWeight = 0f;
    }

    private void FinishMorph()
    {
        _isFinished = true;
        PlayerAppearance.DataInstance = _currentPlayerData;
        SceneLoader.Instance.LoadNextScene();
    }
}