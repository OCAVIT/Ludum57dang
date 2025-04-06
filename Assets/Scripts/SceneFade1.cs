using UnityEngine;

public class FadeOutPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Ссылка на CanvasGroup, который нужно анимировать
    public float fadeDuration = 1f; // Длительность анимации (в секундах)

    private void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup не назначен!");
            return;
        }

        // Запускаем корутину для выполнения FadeOut
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha; // Начальное значение прозрачности
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration); // Линейная интерполяция
            yield return null; // Ждем следующий кадр
        }

        canvasGroup.alpha = 0f; // Убедимся, что значение точно 0
        canvasGroup.interactable = false; // Отключаем взаимодействие
        canvasGroup.blocksRaycasts = false; // Отключаем блокировку кликов
    }
}