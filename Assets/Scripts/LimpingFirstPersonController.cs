using UnityEngine;
using TMPro; // Для работы с TMP_Text

public class LimpingFirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public Transform playerCamera; // Ссылка на камеру (должна быть дочерним объектом игрока)
    public float gravity = -9.81f; // Гравитация
    public float maxLookAngle = 90f; // Максимальный угол обзора вверх/вниз

    private CharacterController controller; // Ссылка на CharacterController
    private Vector3 velocity; // Скорость падения (гравитация)
    private float verticalRotation = 0f; // Текущий угол вращения камеры по вертикали
    private bool isGrounded; // Проверка, находится ли игрок на земле

    public bool canLook = true; // Флаг, можно ли управлять камерой

    // Параметры для эффекта хромоты
    public float limpFrequency = 2f; // Частота хромоты
    public float limpAmplitude = 0.1f; // Амплитуда хромоты
    private float limpTimer = 0f; // Таймер для анимации хромоты

    private Vector3 originalCameraPosition; // Исходное положение камеры

    // Новые переменные для взаимодействия с объектом
    public GameObject FONARIK; // Объект фонарика
    public GameObject FONARIKLOB; // Объект, содержащий Light
    public GameObject TextText; // Объект с текстом
    public GameObject cursor; // Объект курсора
    public TMP_Text textComponent; // TMP_Text для изменения текста
    public float interactionDistance = 3f; // Максимальная дистанция взаимодействия

    private bool isLookingAtFonarik = false; // Флаг, смотрит ли игрок на фонарик

    private void Start()
    {
        // Получаем компонент CharacterController
        controller = GetComponent<CharacterController>();

        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Сохраняем исходное положение камеры
        originalCameraPosition = playerCamera.localPosition;

        // Убедимся, что текст и курсор скрыты в начале
        TextText.SetActive(false);
        cursor.SetActive(true);
    }

    private void Update()
    {
        // Проверяем, на земле ли игрок
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Сбрасываем скорость падения, чтобы игрок не "проваливался"
        }

        // Управление мышью (только если canLook == true)
        if (canLook)
        {
            HandleMouseLook();
        }

        // Управление движением
        HandleMovement();

        // Применяем гравитацию
        ApplyGravity();

        // Анимация хромоты
        AnimateLimping();

        // Проверка взаимодействия с объектом
        CheckInteraction();
    }

    private void HandleMouseLook()
    {
        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращаем персонажа по горизонтали (вокруг оси Y)
        transform.Rotate(Vector3.up * mouseX);

        // Обновляем вертикальный угол камеры
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        // Применяем вращение к камере
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal"); // A и D
        float vertical = Input.GetAxis("Vertical");     // W и S

        // Создаем вектор движения относительно направления персонажа
        Vector3 movement = transform.right * horizontal + transform.forward * vertical;

        // Перемещаем персонажа
        controller.Move(movement * moveSpeed * Time.deltaTime);

        // Обновляем таймер хромоты, если игрок движется
        if (movement.magnitude > 0)
        {
            limpTimer += Time.deltaTime * limpFrequency;
        }
        else
        {
            limpTimer = 0f; // Сбрасываем таймер, если игрок не движется
        }
    }

    private void ApplyGravity()
    {
        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;

        // Перемещаем персонажа вниз
        controller.Move(velocity * Time.deltaTime);
    }

    private void AnimateLimping()
    {
        if (limpTimer > 0)
        {
            // Вычисляем смещение камеры по синусоиде
            float limpOffsetY = Mathf.Sin(limpTimer) * limpAmplitude; // Вертикальное смещение
            float limpOffsetX = Mathf.Cos(limpTimer * 0.5f) * limpAmplitude * 0.5f; // Горизонтальное смещение

            // Применяем смещение к исходному положению камеры
            playerCamera.localPosition = originalCameraPosition + new Vector3(limpOffsetX, limpOffsetY, 0f);
        }
        else
        {
            // Плавно возвращаем камеру в исходное положение, если игрок не движется
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }

    private void CheckInteraction()
    {
        // Создаем луч из центра камеры
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Проверяем, попадает ли луч в объект FONARIK
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == FONARIK)
            {
                // Если игрок смотрит на фонарик
                isLookingAtFonarik = true;
                TextText.SetActive(true);
                cursor.SetActive(false);
                textComponent.text = "Press \"E\" To Pick Up";

                // Если игрок нажимает "E"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    FONARIK.SetActive(false);
                    FONARIKLOB.GetComponent<Light>().enabled = true;
                    TextText.SetActive(false);
                    cursor.SetActive(true);
                }
            }
            else
            {
                isLookingAtFonarik = false;
                TextText.SetActive(false);
                cursor.SetActive(true);
            }
        }
        else
        {
            isLookingAtFonarik = false;
            TextText.SetActive(false);
            cursor.SetActive(true);
        }
    }
}