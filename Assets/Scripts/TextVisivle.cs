using UnityEngine;
using TMPro;

public class TextFadeByDistance : MonoBehaviour
{
    public TMP_Text text; // ������ �� ��������� TMP_Text
    public Transform player; // ������ �� ������ ������
    public float fadeStartDistance = 10f; // ���������, � ������� ����� �������� ����������� �������
    public float fadeEndDistance = 5f; // ���������, �� ������� ����� ���������� ��������� �������

    private Color originalColor;

    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }

        if (text != null)
        {
            // ��������� ������������ ���� ������
            originalColor = text.color;
            // ������������� ����� ��������� ���������� � ������
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    void Update()
    {
        if (text == null || player == null) return;

        // ��������� ���������� ����� ������� � �������
        float distance = Vector3.Distance(player.position, transform.position);

        // ��������� �����-����� � ����������� �� ����������
        float alpha = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distance);

        // ��������� �����-����� � ������
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
}