using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f; // �������� ������������
    public float mouseSensitivity = 100f; // ���������������� ����
    public Transform playerCamera; // ������ �� ������ (������ ���� �������� �������� ������)
    public float gravity = -9.81f; // ����������
    public float jumpHeight = 1.5f; // ������ ������
    public float maxLookAngle = 90f; // ������������ ���� ������ �����/����

    private CharacterController controller; // ������ �� CharacterController
    private Vector3 velocity; // �������� ������� (����������)
    private float verticalRotation = 0f; // ������� ���� �������� ������ �� ���������
    private bool isGrounded; // ��������, ��������� �� ����� �� �����

    public bool canLook = true; // ����, ����� �� ��������� �������

    private void Start()
    {
        // �������� ��������� CharacterController
        controller = GetComponent<CharacterController>();

        // �������� ������ � ��������� ��� � ������ ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // ���������, �� ����� �� �����
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ���������� �������� �������, ����� ����� �� "������������"
        }

        // ���������� ����� (������ ���� canLook == true)
        if (canLook)
        {
            HandleMouseLook();
        }

        // ���������� ���������
        HandleMovement();

        // ��������� ����������
        ApplyGravity();
    }

    private void HandleMouseLook()
    {
        // �������� �������� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ������� ��������� �� ����������� (������ ��� Y)
        transform.Rotate(Vector3.up * mouseX);

        // ��������� ������������ ���� ������
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        // ��������� �������� � ������
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        // �������� ���� � ����������
        float horizontal = Input.GetAxis("Horizontal"); // A � D
        float vertical = Input.GetAxis("Vertical");     // W � S

        // ������� ������ �������� ������������ ����������� ���������
        Vector3 movement = transform.right * horizontal + transform.forward * vertical;

        // ���������� ���������
        controller.Move(movement * moveSpeed * Time.deltaTime);

        // ������
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // ������� ��� ������
        }
    }

    private void ApplyGravity()
    {
        // ��������� ����������
        velocity.y += gravity * Time.deltaTime;

        // ���������� ��������� ����
        controller.Move(velocity * Time.deltaTime);
    }
}