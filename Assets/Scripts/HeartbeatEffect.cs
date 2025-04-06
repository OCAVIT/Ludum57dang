using UnityEngine;
using UnityEngine.UI;

public class HeartbeatEffect : MonoBehaviour
{
    public AudioSource audioSource; // ������ �� AudioSource
    public Image vignetteImage;     // ������ �� Image � ���������
    public float blurIntensity = 0.1f; // ������������ ������������� ��������
    public float sensitivity = 1.5f;  // ���������������� � ���������
    public float smoothSpeed = 5f;    // �������� ����������� �������

    private float[] audioSamples = new float[256]; // ������ ��� �����������
    private float currentBlur = 0f; // ������� �������� ��������

    void Update()
    {
        // �������� �����������
        audioSource.GetOutputData(audioSamples, 0);

        // ������������ ������� ���������
        float averageVolume = 0f;
        foreach (float sample in audioSamples)
        {
            averageVolume += Mathf.Abs(sample);
        }
        averageVolume /= audioSamples.Length;

        // ������������ ������������� �������� �� ������ ���������
        float targetBlur = Mathf.Clamp(averageVolume * sensitivity, 0f, blurIntensity);

        // ���������� ��������� ��������
        currentBlur = Mathf.Lerp(currentBlur, targetBlur, Time.deltaTime * smoothSpeed);

        // ��������� �������� � ��������
        vignetteImage.material.SetFloat("_BlurAmount", currentBlur);
    }
}