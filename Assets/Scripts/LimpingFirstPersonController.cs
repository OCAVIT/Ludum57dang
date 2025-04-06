using UnityEngine;
using TMPro; // ��� ������ � TMP_Text

public class LimpingFirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f; // �������� ������������
    public float mouseSensitivity = 100f; // ���������������� ����
    public Transform playerCamera; // ������ �� ������ (������ ���� �������� �������� ������)
    public float gravity = -9.81f; // ����������
    public float maxLookAngle = 90f; // ������������ ���� ������ �����/����

    private CharacterController controller; // ������ �� CharacterController
    private Vector3 velocity; // �������� ������� (����������)
    private float verticalRotation = 0f; // ������� ���� �������� ������ �� ���������
    private bool isGrounded; // ��������, ��������� �� ����� �� �����

    public bool canLook = true; // ����, ����� �� ��������� �������

    // ��������� ��� ������� �������
    public float limpFrequency = 2f; // ������� �������
    public float limpAmplitude = 0.1f; // ��������� �������
    private float limpTimer = 0f; // ������ ��� �������� �������

    private Vector3 originalCameraPosition; // �������� ��������� ������

    // ����� ���������� ��� �������������� � ��������
    public GameObject FONARIK; // ������ ��������
    public GameObject FONARIKLOB; // ������, ���������� Light
    public GameObject TextText; // ������ � �������
    public GameObject cursor; // ������ �������
    public TMP_Text textComponent; // TMP_Text ��� ��������� ������
    public float interactionDistance = 3f; // ������������ ��������� ��������������

    private bool isLookingAtFonarik = false; // ����, ������� �� ����� �� �������

    private void Start()
    {
        // �������� ��������� CharacterController
        controller = GetComponent<CharacterController>();

        // �������� ������ � ��������� ��� � ������ ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ��������� �������� ��������� ������
        originalCameraPosition = playerCamera.localPosition;

        // ��������, ��� ����� � ������ ������ � ������
        TextText.SetActive(false);
        cursor.SetActive(true);
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

        // �������� �������
        AnimateLimping();

        // �������� �������������� � ��������
        CheckInteraction();
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

        // ��������� ������ �������, ���� ����� ��������
        if (movement.magnitude > 0)
        {
            limpTimer += Time.deltaTime * limpFrequency;
        }
        else
        {
            limpTimer = 0f; // ���������� ������, ���� ����� �� ��������
        }
    }

    private void ApplyGravity()
    {
        // ��������� ����������
        velocity.y += gravity * Time.deltaTime;

        // ���������� ��������� ����
        controller.Move(velocity * Time.deltaTime);
    }

    private void AnimateLimping()
    {
        if (limpTimer > 0)
        {
            // ��������� �������� ������ �� ���������
            float limpOffsetY = Mathf.Sin(limpTimer) * limpAmplitude; // ������������ ��������
            float limpOffsetX = Mathf.Cos(limpTimer * 0.5f) * limpAmplitude * 0.5f; // �������������� ��������

            // ��������� �������� � ��������� ��������� ������
            playerCamera.localPosition = originalCameraPosition + new Vector3(limpOffsetX, limpOffsetY, 0f);
        }
        else
        {
            // ������ ���������� ������ � �������� ���������, ���� ����� �� ��������
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }

    private void CheckInteraction()
    {
        // ������� ��� �� ������ ������
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // ���������, �������� �� ��� � ������ FONARIK
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == FONARIK)
            {
                // ���� ����� ������� �� �������
                isLookingAtFonarik = true;
                TextText.SetActive(true);
                cursor.SetActive(false);
                textComponent.text = "Press \"E\" To Pick Up";

                // ���� ����� �������� "E"
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