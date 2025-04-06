using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup; // ������ �� CanvasGroup ������
    public float fadeDuration = 1f; // ������������ ������� Fade In

    // ����� ��� ������ ����� Animation Event
    public void FadeIn()
    {
        if (panelCanvasGroup != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            Debug.LogWarning("CanvasGroup �� ��������!");
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = panelCanvasGroup.alpha; // ��������� �������� alpha
        float targetAlpha = 1f; // �������� �������� alpha (��������� �������)

        // ������ ������ �������������
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        // ��������, ��� alpha ����� ����� 1
        panelCanvasGroup.alpha = targetAlpha;
    }
}