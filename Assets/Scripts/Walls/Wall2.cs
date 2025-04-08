using UnityEngine;
using TMPro;

public class WallVisibilityControllerWithHurtedPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player; // Ссылка на игрока

    [Header("Wall Settings")]
    public float visibilityDistance = 5f; // Дистанция, на которой стена становится полностью видимой
    public float fadeSpeed = 2f; // Скорость изменения прозрачности
    public Material wallMaterial; // Материал стены
    public float maxWallAlpha = 0.5f; // Максимальная непрозрачность стены (0 = полностью прозрачная, 1 = полностью непрозрачная)

    [Header("Text Settings")]
    public TMP_Text[] textMeshProObjects; // Массив TMP_Text объектов
    public float maxTextAlpha = 1f; // Максимальная непрозрачность текста (0 = полностью прозрачный, 1 = полностью непрозрачный)
    public float textAlphaInvisible = 0f; // Альфа-канал текста, когда он невидим

    [Header("HurtedPlayer Settings")]
    public GameObject HurtedPlayer; // Ссылка на объект HurtedPlayer

    [Header("Additional Objects")]
    public GameObject BINT; // Ссылка на объект BINT
    public GameObject WallBlock2; // Ссылка на объект WallBlock2

    private float currentWallAlpha = 0f; // Текущая прозрачность стены
    private float currentTextAlpha = 0f; // Текущая прозрачность текста

    void Start()
    {
        // Устанавливаем начальный альфа-канал стены
        if (wallMaterial != null)
        {
            SetWallAlpha(0f); // Делаем стену изначально невидимой
        }

        // Устанавливаем текст изначально невидимым
        if (textMeshProObjects != null && textMeshProObjects.Length > 0)
        {
            foreach (TMP_Text textMeshPro in textMeshProObjects)
            {
                SetTextAlpha(textMeshPro, textAlphaInvisible);
            }
        }
    }

    void Update()
    {
        if (HurtedPlayer == null) return;

        // Если HurtedPlayer неактивен, ничего не делаем
        if (!HurtedPlayer.activeSelf)
        {
            return; // Ждем, пока HurtedPlayer станет активным
        }

        // Проверяем состояние BINT
        if (BINT != null && WallBlock2 != null)
        {
            if (!BINT.activeSelf)
            {
                WallBlock2.SetActive(false); // Делаем WallBlock2 неактивным
            }
        }

        // Основная логика работы со стеной
        if (player == null || wallMaterial == null || textMeshProObjects == null) return;

        // Рассчитываем расстояние между игроком и стеной
        float distance = Vector3.Distance(player.position, transform.position);

        // Рассчитываем целевую прозрачность в зависимости от расстояния
        float targetAlpha = Mathf.Clamp01(1f - (distance / visibilityDistance));

        // Плавно изменяем прозрачность стены
        currentWallAlpha = Mathf.Lerp(currentWallAlpha, targetAlpha * maxWallAlpha, Time.deltaTime * fadeSpeed);
        SetWallAlpha(currentWallAlpha);

        // Плавно изменяем прозрачность текста
        currentTextAlpha = Mathf.Lerp(currentTextAlpha, targetAlpha * maxTextAlpha, Time.deltaTime * fadeSpeed);
        foreach (TMP_Text textMeshPro in textMeshProObjects)
        {
            SetTextAlpha(textMeshPro, currentTextAlpha);
        }
    }

    // Метод для установки альфа-канала стены
    private void SetWallAlpha(float alpha)
    {
        if (wallMaterial == null) return;

        // Получаем текущий цвет материала
        Color color = wallMaterial.color;

        // Изменяем только альфа-канал, оставляя остальные компоненты цвета неизменными
        color.a = alpha;

        // Применяем измененный цвет обратно к материалу
        wallMaterial.color = color;
    }

    // Метод для установки альфа-канала текста
    private void SetTextAlpha(TMP_Text textMeshPro, float alpha)
    {
        if (textMeshPro == null) return;

        // Получаем текущий цвет текста
        Color color = textMeshPro.color;

        // Изменяем только альфа-канал, оставляя остальные компоненты цвета неизменными
        color.a = alpha;

        // Применяем измененный цвет обратно к тексту
        textMeshPro.color = color;
    }
}