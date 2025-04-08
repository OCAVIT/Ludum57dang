using UnityEngine;
using System.Collections;
using TMPro; // Для работы с TMP_Text
using UnityEngine.UI; // Для работы с CanvasGroup
using UnityEngine.Rendering; // Для работы с Global Volume
using UnityEngine.Rendering.Universal; // Для работы с VHSPro

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
    public GameObject cursor; // Объект курсора
    public float interactionDistance = 3f; // Максимальная дистанция взаимодействия
    public GameObject blood;
    public GameObject BINT; // Объект BINT
    public CanvasGroup BlackPanel; // Панель для затемнения
    public AudioClip Regen; // Аудиоклип для восстановления

    public AudioSource managerAudioSource; // Аудиоисточник в объекте Manager
    public Volume globalVolume; // Ссылка на Global Volume

    private bool isLookingAtBint = false; // Флаг, смотрит ли игрок на BINT
    private bool isLookingAtFonarik = false; // Флаг, смотрит ли игрок на фонарик

    // Аудио
    public AudioSource audioSource; // Источник звука
    public AudioClip[] stepSounds; // Массив звуков шагов
    public AudioClip pickUpSound; // Звук поднятия фонарика
    private bool isWalking = false; // Флаг, идет ли игрок
    private float stepTimer = 0f; // Таймер для шагов
    public float stepInterval = 0.5f; // Интервал между шагами
    public GameObject AUDIOPIZDA;

    private void Start()
    {
        // Получаем компонент CharacterController
        controller = GetComponent<CharacterController>();

        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Сохраняем исходное положение камеры
        originalCameraPosition = playerCamera.localPosition;

        // Убедимся, что курсор скрыт в начале
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

        // Проверка взаимодействия с объектами
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

            // Воспроизведение звуков шагов
            isWalking = true;
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayStepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            limpTimer = 0f; // Сбрасываем таймер, если игрок не движется
            isWalking = false;
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

    private void PickUpFonarik()
    {
        // Отключаем объект фонарика (тот, который лежит на земле)
        FONARIK.SetActive(false);

        // Включаем компонент Light на объекте FONARIKLOB
        Light lightComponent = FONARIKLOB.GetComponent<Light>();
        if (lightComponent != null)
        {
            lightComponent.enabled = true;
            Debug.Log("Компонент Light на FONARIKLOB включен");
        }
        else
        {
            Debug.LogError("На объекте FONARIKLOB отсутствует компонент Light!");
        }

        // Воспроизводим звук поднятия
        if (pickUpSound != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }
    }

    private void CheckInteraction()
    {
        // Создаем луч из центра камеры
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Проверяем, попадает ли луч в объект
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == BINT && FONARIKLOB.activeSelf)
            {
                // Логика для BINT
                isLookingAtBint = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(UseBint());
                }
            }
            else if (hit.collider.gameObject == FONARIK)
            {
                // Логика для FONARIK
                isLookingAtFonarik = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpFonarik();
                }
            }
            else
            {
                // Если игрок не смотрит ни на BINT, ни на FONARIK
                isLookingAtBint = false;
                isLookingAtFonarik = false;
            }
        }
        else
        {
            // Если луч никуда не попал
            isLookingAtBint = false;
            isLookingAtFonarik = false;
        }
    }

    private IEnumerator UseBint()
    {
        // Отключаем BINT
        BINT.SetActive(false);

        // Воспроизводим звук восстановления
        audioSource.PlayOneShot(Regen);

        // FadeIn черной панели
        yield return StartCoroutine(FadeCanvasGroup(BlackPanel, 0f, 1f, 1f));

        // Изменяем параметры во время FadeOut
        moveSpeed = 4f; // Устанавливаем скорость игрока
        stepInterval = 1f; // Устанавливаем интервал шагов
        managerAudioSource.enabled = false; // Отключаем аудиоисточник Manager
        blood.SetActive(false); // Отключаем объект blood

        // Отключаем флаг feedback в Global Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet<VHSPro>(out VHSPro vhsPro))
            {
                if (vhsPro.feedbackOn != null)
                {
                    vhsPro.feedbackOn.value = false;
                }
            }
        }

        // FadeOut черной панели
        yield return StartCoroutine(FadeCanvasGroup(BlackPanel, 1f, 0f, 1f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        // Отключаем скрипт "Panel Fader" в начале FadeOut
        if (startAlpha > endAlpha && canvasGroup.gameObject.TryGetComponent(out MonoBehaviour panelFader))
        {
            AUDIOPIZDA.SetActive(false);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    private void PlayStepSound()
    {
        if (stepSounds.Length > 0 && isWalking)
        {
            // Выбираем случайный звук из массива
            int randomIndex = Random.Range(0, stepSounds.Length);
            audioSource.PlayOneShot(stepSounds[randomIndex]);
        }
    }
}