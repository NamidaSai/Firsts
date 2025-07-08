using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private CanvasGroup fader;
    
    public static SceneLoader Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void LoadNextScene()
    {
        StartCoroutine(FadeToNextScene());
    }

    private IEnumerator FadeToNextScene()
    {
        float halfTransitionTime = transitionTime * 0.5f;
        
        fader.DOFade(1f, halfTransitionTime);
        yield return new WaitForSeconds(halfTransitionTime);
        fader.DOFade(0f, halfTransitionTime);
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        
    }
}
