using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public Transform playerCamera; // Ссылка на камеру (должна быть дочерним объектом игрока)
    public float gravity = -9.81f; // Гравитация
    public float jumpHeight = 1.5f; // Высота прыжка
    public float maxLookAngle = 90f; // Максимальный угол обзора вверх/вниз

    private CharacterController controller; // Ссылка на CharacterController
    private Vector3 velocity; // Скорость падения (гравитация)
    private float verticalRotation = 0f; // Текущий угол вращения камеры по вертикали
    private bool isGrounded; // Проверка, находится ли игрок на земле

    public bool canLook = true; // Флаг, можно ли управлять камерой

    private void Start()
    {
        // Получаем компонент CharacterController
        controller = GetComponent<CharacterController>();

        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Формула для прыжка
        }
    }

    private void ApplyGravity()
    {
        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;

        // Перемещаем персонажа вниз
        controller.Move(velocity * Time.deltaTime);
    }
}