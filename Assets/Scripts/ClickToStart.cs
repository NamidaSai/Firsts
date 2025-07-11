using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ClickToStart : MonoBehaviour
{
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private CanvasGroup titleCanvasGroup;

    private void OnMouseDown()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        titleCanvasGroup.DOFade(0f, startDelay);
        yield return new WaitForSeconds(startDelay);
        SceneLoader.Instance.LoadNextScene();
    }
}