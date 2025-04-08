using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // ������ ������
    public Camera miniGameCamera; // ������ ����-����
    public GameObject player; // ������ ������
    public CanvasGroup blackPanel; // ������ ��� FadeIn/FadeOut
    public GameObject skull; // ������ Skull
    public Transform pointStart; // ��������� ����� ��� Skull
    public Transform pointEnd; // �������� ����� ��� Skull
    public AudioClip screamer; // ��������� Screamer
    public AudioSource audioSource; // ������������� ��� ������������ Screamer

    [Header("MiniGame Settings")]
    public List<Transform> points; // ������ ����� ��� ������
    public List<KeyCode[]> pointKeys; // ������ ������ ��� �������� ����� �������
    public float cameraMoveSpeed = 2f; // �������� ����������� ������
    public float cameraSensitivity = 0.5f; // ���������������� ������
    public float cameraResistance = 2f; // ���� ������������� ������ (��� ������, ��� ������� ������ ������������)
    public Vector3 defaultCameraRotation; // �������� ��������� ������ (���� � ��������)
    public float skullMoveSpeed = 2f; // �������� ����������� Skull

    private int currentPointIndex = 0; // ������� �����
    private bool isMiniGameActive = false; // ������� �� ����-����
    private bool isFading = false; // ���� �� �������� Fade
    private bool isSkullMoving = false; // ��������� �� Skull
    private Vector3 currentRotationOffset; // �������� ������ �� ��������� ���������

    private void Start()
    {
        // ���������, ������ �� �����
        if (points == null || points.Count == 0)
        {
            Debug.LogError("������ ����� (points) �� ����� ��� ����!");
        }

        if (pointKeys == null || pointKeys.Count == 0)
        {
            Debug.LogError("������ ������ (pointKeys) �� ����� ��� ����!");
        }

        // ������������� ������ ��� �������� ����� �������
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
        // ���������, ������� �� ����� �� ������ � ����� "Wall" � �������� "E"
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

        // ��������� ������ � �������� ����-����
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
        // �������� ���� �� ����
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

        // ��������� ������� �������� ������ �� ������ ����� ������
        currentRotationOffset.x -= mouseY;
        currentRotationOffset.y += mouseX;

        // ��������� �������������: ������ ��������� ��������� � ��������� ���������
        currentRotationOffset = Vector3.Lerp(
            currentRotationOffset,
            Vector3.zero, // �������� ��������� (��� ��������)
            Time.deltaTime * cameraResistance
        );

        // ��������� �������� � ������
        Vector3 targetRotation = defaultCameraRotation + currentRotationOffset;
        miniGameCamera.transform.rotation = Quaternion.Euler(targetRotation);
    }

    private void HandlePointTransition()
    {
        if (currentPointIndex >= points.Count) return;

        // ������� ����������� ������ � ������� �����
        Transform targetPoint = points[currentPointIndex];
        miniGameCamera.transform.position = Vector3.Lerp(miniGameCamera.transform.position, targetPoint.position, Time.deltaTime * cameraMoveSpeed);

        // ���������, ������ �� ������ ������� ��� �������� � ��������� �����
        if (AreKeysPressed(pointKeys[currentPointIndex]))
        {
            Debug.Log($"������� � ��������� �����: {currentPointIndex + 1}");
            currentPointIndex++;

            // ���� �������� ��������� �����, ��������� �������
            if (currentPointIndex == points.Count)
            {
                Debug.Log("���������� ��������� �����. ������ ��������.");
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

        // ���������, ������ �� ����� pointStart � pointEnd
        if (pointStart == null || pointEnd == null)
        {
            Debug.LogError("PointStart ��� PointEnd �� ������!");
            yield break;
        }

        // ���������� Skull � ��������� �����
        skull.transform.position = pointStart.position;

        // �������� ��������� Screamer
        if (screamer != null && audioSource != null)
        {
            audioSource.clip = screamer;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Screamer ��� AudioSource �� ���������!");
        }

        // ���������� Skull �� PointStart �� PointEnd
        while (Vector3.Distance(skull.transform.position, pointEnd.position) > 0.01f)
        {
            skull.transform.position = Vector3.MoveTowards(skull.transform.position, pointEnd.position, skullMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // ������������� ���������
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // ��������� FadeIn ������
        if (blackPanel != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackPanel, 0, 1, 1f));
        }
        else
        {
            Debug.LogError("BlackPanel �� ��������!");
        }

        isSkullMoving = false;

        // ���������� ������ � ������� ����� ����� ��������
        EndMiniGame();
    }

    private void EndMiniGame()
    {
        Debug.Log("����-���� ���������. ���������� ������ � ������� �����.");

        // ��������� ����-���� � ���������� ���������� ������
        isMiniGameActive = false;
        miniGameCamera.gameObject.SetActive(false);
        player.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ���������� ������� �����
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