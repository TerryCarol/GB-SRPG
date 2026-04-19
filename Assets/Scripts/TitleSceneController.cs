using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup fadePanel;

    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            StartCoroutine(FadeCoroutine(fadePanel, 1f, 0f, 1f));
        }
        startButton?.onClick.AddListener(OnStartGame);
        exitButton?.onClick.AddListener(OnExitGame);
    }

    private IEnumerator FadeCoroutine(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvas.alpha = endAlpha;
    }

    private void OnStartGame()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeCoroutine(fadePanel, 0f, 1f, 0.5f));
        }
        GameSceneManager.Instance.LoadGameScene();
    }

    private void OnExitGame()
    {
        Application.Quit();

        // 유니티 에디터 테스트용
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}