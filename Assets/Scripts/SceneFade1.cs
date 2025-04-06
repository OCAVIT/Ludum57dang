using UnityEngine;

public class FadeOutPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup; // ������ �� CanvasGroup, ������� ����� �����������
    public float fadeDuration = 1f; // ������������ �������� (� ��������)

    private void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup �� ��������!");
            return;
        }

        // ��������� �������� ��� ���������� FadeOut
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha; // ��������� �������� ������������
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration); // �������� ������������
            yield return null; // ���� ��������� ����
        }

        canvasGroup.alpha = 0f; // ��������, ��� �������� ����� 0
        canvasGroup.interactable = false; // ��������� ��������������
        canvasGroup.blocksRaycasts = false; // ��������� ���������� ������
    }
}