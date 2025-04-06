using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup; // Ссылка на CanvasGroup панели
    public float fadeDuration = 1f; // Длительность эффекта Fade In

    // Метод для вызова через Animation Event
    public void FadeIn()
    {
        if (panelCanvasGroup != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            Debug.LogWarning("CanvasGroup не назначен!");
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = panelCanvasGroup.alpha; // Начальное значение alpha
        float targetAlpha = 1f; // Конечное значение alpha (полностью видимый)

        // Делаем панель интерактивной
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        // Убедимся, что alpha точно равен 1
        panelCanvasGroup.alpha = targetAlpha;
    }
}