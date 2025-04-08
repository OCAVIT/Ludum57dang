using UnityEngine;
using TMPro;

public class TextFadeByDistance : MonoBehaviour
{
    public TMP_Text text; // Ссылка на компонент TMP_Text
    public Transform player; // Ссылка на объект игрока
    public float fadeStartDistance = 10f; // Дистанция, с которой текст начинает становиться видимым
    public float fadeEndDistance = 5f; // Дистанция, на которой текст становится полностью видимым

    private Color originalColor;

    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }

        if (text != null)
        {
            // Сохраняем оригинальный цвет текста
            originalColor = text.color;
            // Устанавливаем текст полностью прозрачным в начале
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    void Update()
    {
        if (text == null || player == null) return;

        // Вычисляем расстояние между игроком и текстом
        float distance = Vector3.Distance(player.position, transform.position);

        // Вычисляем альфа-канал в зависимости от расстояния
        float alpha = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distance);

        // Применяем альфа-канал к тексту
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
}