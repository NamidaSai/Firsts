using UnityEngine;

[RequireComponent(typeof(PlayerAppearance))]
public class ChangeAppearance : MonoBehaviour
{
    [SerializeField] private float timeRequired = 3f;

    [Header("Morph Timing")]
    [SerializeField] private float resetCooldown = 2f;
    [SerializeField] private float timeToReverse = 2f;

    private PlayerAppearance _playerAppearance;

    private bool _isFinished = false;
    private PlayerShape? _lastMorphTarget = null;
    private float _lastMorphT = 0f;
    private float _lastTimeElapsed = 0f;
    private float _timeElapsed = 0f;
    private float _timeSincePlayerVisit = 0f;
    private bool _playerWithinRange = false;

    private void Awake()
    {
        _playerAppearance = GetComponent<PlayerAppearance>();
    }

    private void Update()
    {
        if (_isFinished || _playerWithinRange) { return; }
        _timeSincePlayerVisit += Time.deltaTime;
        HandleReverseMorphing();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isFinished) { return; }
        if (!other.CompareTag("Player")) { return; }
        
        _timeElapsed += Time.deltaTime;
        _lastTimeElapsed = _timeElapsed;
        HandleMorphing();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        _playerWithinRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        _playerWithinRange = false;
        _timeSincePlayerVisit = 0f;
    }

    private void HandleMorphing()
    {
        float t = Mathf.Clamp01(_timeElapsed / timeRequired);
        
        _playerAppearance.SetAppearanceFromTo(PlayerShape.Circle, PlayerAppearance.DataInstance.Shape, t);
        _lastMorphTarget = PlayerAppearance.DataInstance.Shape;
        _lastMorphT = t;
        
        if (t >= 1f)
        {
            _isFinished = true;
        }
    }

    private void HandleReverseMorphing()
    {
        if (_lastMorphTarget is null or PlayerShape.Circle) return;

        float progress = _timeSincePlayerVisit - resetCooldown;
        float duration = timeToReverse;
        float t = Mathf.Clamp01(progress / duration);
        float adjustedT = Mathf.Lerp(1-_lastMorphT, 1f, t);
        
        _timeElapsed = Mathf.Lerp(_lastTimeElapsed, 0f, t);
        
        _playerAppearance.SetAppearanceFromTo(_lastMorphTarget.Value, PlayerShape.Circle, adjustedT);

        if (t >= 1f)
        {
            _lastMorphTarget = null; // Reset after full reverse
        }
    }
}