using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.stickyCursorLock = false;
        // setting stickyCursorLock to false keeps Cursor.lockState in sync with browser cursor lock state
#endif
    }
}