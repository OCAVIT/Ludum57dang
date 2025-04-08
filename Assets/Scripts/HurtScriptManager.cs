using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PanelFader : MonoBehaviour
{
    public CanvasGroup whitePanel;
    public CanvasGroup blackPanel;
    public float fadeDuration = 2f;

    public AudioClip kontuzClip;
    public AudioClip heartBeatClip; // Аудиоклип HeartBeat
    public AudioClip shumClip; // Аудиоклип Shum

    private AudioSource audioSource; // Основной AudioSource
    private AudioSource shumAudioSource; // Отдельный AudioSource для Shum

    public GameObject playerAnim;
    public GameObject playerHurt;
    public GameObject cursor;
    public GameObject blood;

    public Volume globalVolume; // Ссылка на Global Volume для включения Feedback

    private void Awake()
    {
        // Основной AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Дополнительный AudioSource для Shum
        shumAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Воспроизведение начального аудиоклипа (kontuzClip)
        if (kontuzClip != null && audioSource != null)
        {
            audioSource.clip = kontuzClip;
            audioSource.Play();
        }

        // FadeIn белой панели
        yield return StartCoroutine(FadeCanvasGroup(whitePanel, 0f, 1f, fadeDuration));

        // Обновление состояния объектов
        playerAnim.SetActive(false);
        playerHurt.SetActive(true);

        cursor.SetActive(true);
        blood.SetActive(true);

        blackPanel.alpha = 0f;
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = false;

        // FadeOut белой панели с запуском аудио и включением Feedback
        yield return StartCoroutine(FadeCanvasGroupWithAudioAndFeedback(whitePanel, 1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroupWithAudioAndFeedback(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        bool audioStarted = false; // Флаг для запуска аудио
        bool feedbackEnabled = false; // Флаг для включения Feedback

        canvasGroup.alpha = startAlpha;

        canvasGroup.interactable = startAlpha > 0;
        canvasGroup.blocksRaycasts = startAlpha > 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Запуск аудио за 1 секунду до завершения FadeOut
            if (!audioStarted && elapsedTime >= duration - 1f)
            {
                audioStarted = true;

                // Воспроизведение HeartBeat
                if (heartBeatClip != null && audioSource != null)
                {
                    audioSource.clip = heartBeatClip;
                    audioSource.Play();
                }

                // Воспроизведение Shum
                if (shumClip != null && shumAudioSource != null)
                {
                    shumAudioSource.clip = shumClip;
                    shumAudioSource.Play();
                }
            }

            // Включение Feedback через Global Volume
            if (!feedbackEnabled && globalVolume != null && globalVolume.profile != null)
            {
                if (globalVolume.profile.TryGet<VHSPro>(out VHSPro vhsPro))
                {
                    if (vhsPro.feedbackOn != null)
                    {
                        vhsPro.feedbackOn.value = true;
                        feedbackEnabled = true; // Устанавливаем флаг, чтобы включить Feedback только один раз
                    }
                }
            }

            // Обновление альфа-канала панели
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