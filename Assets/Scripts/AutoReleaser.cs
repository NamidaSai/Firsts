using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CursorManager))]
public class AutoReleaser : MonoBehaviour
{
       [SerializeField] private float _delay = 3f;
       
       private CursorManager _cursorManager;
       
       private void OnValidate()
       {
              if (_cursorManager) { return; }
              _cursorManager = GetComponent<CursorManager>();
       }

       private IEnumerator Start()
       {
              if (_cursorManager == null) { yield break; }
              yield return new WaitForSeconds(_delay);
              _cursorManager.ReleaseCursor();
       }
}