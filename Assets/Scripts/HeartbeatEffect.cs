using UnityEngine;
using UnityEngine.UI;

public class HeartbeatEffect : MonoBehaviour
{
    public AudioSource audioSource; // Ссылка на AudioSource
    public Image vignetteImage;     // Ссылка на Image с виньеткой
    public float blurIntensity = 0.1f; // Максимальная интенсивность размытия
    public float sensitivity = 1.5f;  // Чувствительность к громкости
    public float smoothSpeed = 5f;    // Скорость сглаживания эффекта

    private float[] audioSamples = new float[256]; // Массив для аудиоданных
    private float currentBlur = 0f; // Текущее значение размытия

    void Update()
    {
        // Получаем аудиоданные
        audioSource.GetOutputData(audioSamples, 0);

        // Рассчитываем среднюю громкость
        float averageVolume = 0f;
        foreach (float sample in audioSamples)
        {
            averageVolume += Mathf.Abs(sample);
        }
        averageVolume /= audioSamples.Length;

        // Рассчитываем интенсивность размытия на основе громкости
        float targetBlur = Mathf.Clamp(averageVolume * sensitivity, 0f, blurIntensity);

        // Сглаживаем изменение размытия
        currentBlur = Mathf.Lerp(currentBlur, targetBlur, Time.deltaTime * smoothSpeed);

        // Применяем размытие к виньетке
        vignetteImage.material.SetFloat("_BlurAmount", currentBlur);
    }
}