using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // Камера игрока
    public Camera miniGameCamera; // Камера мини-игры
    public GameObject player; // Объект игрока
    public CanvasGroup blackPanel; // Панель для FadeIn/FadeOut
    public GameObject skull; // Объект Skull
    public Transform pointStart; // Начальная точка для Skull
    public Transform pointEnd; // Конечная точка для Skull
    public AudioClip screamer; // Аудиоклип Screamer
    public AudioSource audioSource; // Аудиоисточник для проигрывания Screamer

    [Header("MiniGame Settings")]
    public List<Transform> points; // Список точек для камеры
    public List<KeyCode[]> pointKeys; // Список клавиш для перехода между точками
    public float cameraMoveSpeed = 2f; // Скорость перемещения камеры
    public float cameraSensitivity = 0.5f; // Чувствительность камеры
    public float cameraResistance = 2f; // Сила сопротивления камеры (чем больше, тем сильнее камера возвращается)
    public Vector3 defaultCameraRotation; // Исходное положение камеры (углы в градусах)
    public float skullMoveSpeed = 2f; // Скорость перемещения Skull

    private int currentPointIndex = 0; // Текущая точка
    private bool isMiniGameActive = false; // Активна ли мини-игра
    private bool isFading = false; // Идет ли анимация Fade
    private bool isSkullMoving = false; // Двигается ли Skull
    private Vector3 currentRotationOffset; // Смещение камеры от исходного положения

    private void Start()
    {
        // Проверяем, заданы ли точки
        if (points == null || points.Count == 0)
        {
            Debug.LogError("Список точек (points) не задан или пуст!");
        }

        if (pointKeys == null || pointKeys.Count == 0)
        {
            Debug.LogError("Список клавиш (pointKeys) не задан или пуст!");
        }

        // Инициализация клавиш для перехода между точками
        pointKeys = new List<KeyCode[]>
        {
            new KeyCode[] { KeyCode.W }, // 1 -> 2
            new KeyCode[] { KeyCode.A }, // 2 -> 3
            new KeyCode[] { KeyCode.D }, // 3 -> 4
            new KeyCode[] { KeyCode.S }, // 4 -> 5
            new KeyCode[] { KeyCode.D, KeyCode.S }, // 5 -> 6
            new KeyCode[] { KeyCode.S, KeyCode.W }, // 6 -> 7
            new KeyCode[] { KeyCode.A, KeyCode.W, KeyCode.D }, // 7 -> 8
            new KeyCode[] { KeyCode.A, KeyCode.W, KeyCode.S }, // 8 -> 9
            new KeyCode[] { KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.D } // 9 -> 10
        };
    }

    private void Update()
    {
        if (isMiniGameActive)
        {
            HandleCameraMovement();
            HandlePointTransition();
        }
        else
        {
            CheckForMiniGameActivation();
        }
    }

    private void CheckForMiniGameActivation()
    {
        // Проверяем, смотрит ли игрок на объект с тегом "Wall" и нажимает "E"
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Wall") && Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(StartMiniGame());
            }
        }
    }

    private IEnumerator StartMiniGame()
    {
        if (isFading) yield break;

        isFading = true;

        // FadeIn
        yield return StartCoroutine(FadeCanvasGroup(blackPanel, 0, 1, 1f));

        // Отключаем игрока и включаем мини-игру
        player.SetActive(false);
        miniGameCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // FadeOut
        yield return StartCoroutine(FadeCanvasGroup(blackPanel, 1, 0, 1f));

        isMiniGameActive = true;
        isFading = false;
    }

    private void HandleCameraMovement()
    {
        // Получаем ввод от мыши
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

        // Обновляем текущее смещение камеры на основе ввода игрока
        currentRotationOffset.x -= mouseY;
        currentRotationOffset.y += mouseX;

        // Применяем сопротивление: камера стремится вернуться к исходному положению
        currentRotationOffset = Vector3.Lerp(
            currentRotationOffset,
            Vector3.zero, // Исходное положение (без смещения)
            Time.deltaTime * cameraResistance
        );

        // Применяем смещение к камере
        Vector3 targetRotation = defaultCameraRotation + currentRotationOffset;
        miniGameCamera.transform.rotation = Quaternion.Euler(targetRotation);
    }

    private void HandlePointTransition()
    {
        if (currentPointIndex >= points.Count) return;

        // Плавное перемещение камеры к текущей точке
        Transform targetPoint = points[currentPointIndex];
        miniGameCamera.transform.position = Vector3.Lerp(miniGameCamera.transform.position, targetPoint.position, Time.deltaTime * cameraMoveSpeed);

        // Проверяем, нажаты ли нужные клавиши для перехода к следующей точке
        if (AreKeysPressed(pointKeys[currentPointIndex]))
        {
            Debug.Log($"Переход к следующей точке: {currentPointIndex + 1}");
            currentPointIndex++;

            // Если достигли последней точки, запускаем скример
            if (currentPointIndex == points.Count)
            {
                Debug.Log("Достигнута последняя точка. Запуск скримера.");
                StartCoroutine(StartSkullSequence());
            }
        }
    }

    private bool AreKeysPressed(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
        {
            if (!Input.GetKey(key)) return false;
        }
        return true;
    }

    private IEnumerator StartSkullSequence()
    {
        if (isSkullMoving) yield break;

        isSkullMoving = true;

        // Проверяем, заданы ли точки pointStart и pointEnd
        if (pointStart == null || pointEnd == null)
        {
            Debug.LogError("PointStart или PointEnd не заданы!");
            yield break;
        }

        // Перемещаем Skull к начальной точке
        skull.transform.position = pointStart.position;

        // Включаем аудиоклип Screamer
        if (screamer != null && audioSource != null)
        {
            audioSource.clip = screamer;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Screamer или AudioSource не настроены!");
        }

        // Перемещаем Skull от PointStart до PointEnd
        while (Vector3.Distance(skull.transform.position, pointEnd.position) > 0.01f)
        {
            skull.transform.position = Vector3.MoveTowards(skull.transform.position, pointEnd.position, skullMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // Останавливаем аудиоклип
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Запускаем FadeIn панели
        if (blackPanel != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackPanel, 0, 1, 1f));
        }
        else
        {
            Debug.LogError("BlackPanel не настроен!");
        }

        isSkullMoving = false;

        // Возвращаем игрока в обычный режим после скримера
        EndMiniGame();
    }

    private void EndMiniGame()
    {
        Debug.Log("Мини-игра завершена. Возвращаем игрока в обычный режим.");

        // Отключаем мини-игру и возвращаем управление игроку
        isMiniGameActive = false;
        miniGameCamera.gameObject.SetActive(false);
        player.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Сбрасываем текущую точку
        currentPointIndex = 0;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}