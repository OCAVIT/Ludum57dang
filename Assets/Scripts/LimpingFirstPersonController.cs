using UnityEngine;
using System.Collections;
using TMPro; // ��� ������ � TMP_Text
using UnityEngine.UI; // ��� ������ � CanvasGroup
using UnityEngine.Rendering; // ��� ������ � Global Volume
using UnityEngine.Rendering.Universal; // ��� ������ � VHSPro

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
    public GameObject cursor; // ������ �������
    public float interactionDistance = 3f; // ������������ ��������� ��������������
    public GameObject blood;
    public GameObject BINT; // ������ BINT
    public CanvasGroup BlackPanel; // ������ ��� ����������
    public AudioClip Regen; // ��������� ��� ��������������

    public AudioSource managerAudioSource; // ������������� � ������� Manager
    public Volume globalVolume; // ������ �� Global Volume

    private bool isLookingAtBint = false; // ����, ������� �� ����� �� BINT
    private bool isLookingAtFonarik = false; // ����, ������� �� ����� �� �������

    // �����
    public AudioSource audioSource; // �������� �����
    public AudioClip[] stepSounds; // ������ ������ �����
    public AudioClip pickUpSound; // ���� �������� ��������
    private bool isWalking = false; // ����, ���� �� �����
    private float stepTimer = 0f; // ������ ��� �����
    public float stepInterval = 0.5f; // �������� ����� ������
    public GameObject AUDIOPIZDA;

    private void Start()
    {
        // �������� ��������� CharacterController
        controller = GetComponent<CharacterController>();

        // �������� ������ � ��������� ��� � ������ ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ��������� �������� ��������� ������
        originalCameraPosition = playerCamera.localPosition;

        // ��������, ��� ������ ����� � ������
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

        // �������� �������������� � ���������
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

            // ��������������� ������ �����
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
            limpTimer = 0f; // ���������� ������, ���� ����� �� ��������
            isWalking = false;
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

    private void PickUpFonarik()
    {
        // ��������� ������ �������� (���, ������� ����� �� �����)
        FONARIK.SetActive(false);

        // �������� ��������� Light �� ������� FONARIKLOB
        Light lightComponent = FONARIKLOB.GetComponent<Light>();
        if (lightComponent != null)
        {
            lightComponent.enabled = true;
            Debug.Log("��������� Light �� FONARIKLOB �������");
        }
        else
        {
            Debug.LogError("�� ������� FONARIKLOB ����������� ��������� Light!");
        }

        // ������������� ���� ��������
        if (pickUpSound != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }
    }

    private void CheckInteraction()
    {
        // ������� ��� �� ������ ������
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // ���������, �������� �� ��� � ������
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == BINT && FONARIKLOB.activeSelf)
            {
                // ������ ��� BINT
                isLookingAtBint = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(UseBint());
                }
            }
            else if (hit.collider.gameObject == FONARIK)
            {
                // ������ ��� FONARIK
                isLookingAtFonarik = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpFonarik();
                }
            }
            else
            {
                // ���� ����� �� ������� �� �� BINT, �� �� FONARIK
                isLookingAtBint = false;
                isLookingAtFonarik = false;
            }
        }
        else
        {
            // ���� ��� ������ �� �����
            isLookingAtBint = false;
            isLookingAtFonarik = false;
        }
    }

    private IEnumerator UseBint()
    {
        // ��������� BINT
        BINT.SetActive(false);

        // ������������� ���� ��������������
        audioSource.PlayOneShot(Regen);

        // FadeIn ������ ������
        yield return StartCoroutine(FadeCanvasGroup(BlackPanel, 0f, 1f, 1f));

        // �������� ��������� �� ����� FadeOut
        moveSpeed = 4f; // ������������� �������� ������
        stepInterval = 1f; // ������������� �������� �����
        managerAudioSource.enabled = false; // ��������� ������������� Manager
        blood.SetActive(false); // ��������� ������ blood

        // ��������� ���� feedback � Global Volume
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

        // FadeOut ������ ������
        yield return StartCoroutine(FadeCanvasGroup(BlackPanel, 1f, 0f, 1f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        // ��������� ������ "Panel Fader" � ������ FadeOut
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
            // �������� ��������� ���� �� �������
            int randomIndex = Random.Range(0, stepSounds.Length);
            audioSource.PlayOneShot(stepSounds[randomIndex]);
        }
    }
}