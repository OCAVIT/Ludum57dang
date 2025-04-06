using System.Collections;
using UnityEngine;

public class PanelFader : MonoBehaviour
{
    public CanvasGroup whitePanel;
    public CanvasGroup blackPanel;
    public float fadeDuration = 2f;

    public AudioClip kontuzClip;
    public AudioClip heartBeatClip; // ��������� HeartBeat
    public AudioClip shumClip; // ��������� Shum

    private AudioSource audioSource; // �������� AudioSource
    private AudioSource shumAudioSource; // ��������� AudioSource ��� Shum

    public GameObject playerAnim;
    public GameObject playerHurt;
    public GameObject cursor;
    public GameObject blood;

    private void Awake()
    {
        // �������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �������������� AudioSource ��� Shum
        shumAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // ��������������� ���������� ���������� (kontuzClip)
        if (kontuzClip != null && audioSource != null)
        {
            audioSource.clip = kontuzClip;
            audioSource.Play();
        }

        // FadeIn ����� ������
        yield return StartCoroutine(FadeCanvasGroup(whitePanel, 0f, 1f, fadeDuration));

        // ���������� ��������� ��������
        playerAnim.SetActive(false);
        playerHurt.SetActive(true);

        cursor.SetActive(true);
        blood.SetActive(true);

        blackPanel.alpha = 0f;
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = false;

        // FadeOut ����� ������ � �������� ����� �� 1 ������� �� ����������
        yield return StartCoroutine(FadeCanvasGroupWithAudio(whitePanel, 1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroupWithAudio(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        bool audioStarted = false; // ���� ��� ������� �����

        canvasGroup.alpha = startAlpha;

        canvasGroup.interactable = startAlpha > 0;
        canvasGroup.blocksRaycasts = startAlpha > 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // ������ ����� �� 1 ������� �� ���������� FadeOut
            if (!audioStarted && elapsedTime >= duration - 1f)
            {
                audioStarted = true;

                // ��������������� HeartBeat
                if (heartBeatClip != null && audioSource != null)
                {
                    audioSource.clip = heartBeatClip;
                    audioSource.Play();
                }

                // ��������������� Shum
                if (shumClip != null && shumAudioSource != null)
                {
                    shumAudioSource.clip = shumClip;
                    shumAudioSource.Play();
                }
            }

            // ���������� �����-������ ������
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        canvasGroup.interactable = endAlpha > 0;
        canvasGroup.blocksRaycasts = endAlpha > 0;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        canvasGroup.alpha = startAlpha;

        canvasGroup.interactable = startAlpha > 0;
        canvasGroup.blocksRaycasts = startAlpha > 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        canvasGroup.interactable = endAlpha > 0;
        canvasGroup.blocksRaycasts = endAlpha > 0;
    }
}