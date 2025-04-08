using UnityEngine;
using TMPro;

public class PaperInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GameObject cursor;
    public GameObject opisanie;
    public TMP_Text opisanieName;
    public TMP_Text opisanieDescr;
    public LimpingFirstPersonController cameraController;
    public CharacterController playerController;
    public GameObject exitObject;

    [Header("Inspection Settings")]
    public Vector3 inspectionOffset = new Vector3(-0.5f, 0, 2f);
    public float maxInspectDistance = 3f;
    public float transitionSpeed = 5f;
    public float returnThreshold = 0.01f;

    [Header("Inspectable Objects")]
    public InspectableObject[] inspectableObjects;

    [Header("Audio Settings")]
    public AudioClip pickSound; // Звук при входе в инспект
    public AudioClip dropSound; // Звук при выходе из инспекта
    private AudioSource audioSource; // Источник звука
    public float audioVolume = 0.5f; // Громкость звука (уменьшена в 2 раза)

    private bool isInspecting = false;
    private bool isReturning = false;
    private Transform currentObject;
    private Vector3 originalObjectPosition;
    private Quaternion originalObjectRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool cursorInitiallyDisabled = false; // Новый флаг для отслеживания состояния курсора

    private void Start()
    {
        opisanie.SetActive(false);

        if (exitObject != null)
        {
            exitObject.SetActive(false);
        }

        // Отключаем курсор в начале сцены
        if (cursor != null)
        {
            cursor.SetActive(false);
            cursorInitiallyDisabled = true; // Устанавливаем флаг, что курсор был отключен
        }

        // Инициализируем AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = audioVolume; // Устанавливаем громкость для всех звуков
    }

    private void Update()
    {
        if (!isInspecting && !isReturning)
        {
            CheckForInspectableObject();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInspecting)
            {
                ExitInspectMode();
            }
            else if (currentObject != null)
            {
                EnterInspectMode();
            }

            // Деактивируем Textpapper для текущего объекта при нажатии на "E"
            foreach (var obj in inspectableObjects)
            {
                if (currentObject == obj.objectTransform && obj.textPaper != null)
                {
                    obj.textPaper.SetActive(false);
                }
            }
        }

        if (currentObject != null)
        {
            if (isInspecting)
            {
                currentObject.position = Vector3.Lerp(currentObject.position, targetPosition, Time.deltaTime * transitionSpeed);
                currentObject.rotation = Quaternion.Lerp(currentObject.rotation, targetRotation, Time.deltaTime * transitionSpeed);
            }
            else if (isReturning)
            {
                currentObject.position = Vector3.Lerp(currentObject.position, originalObjectPosition, Time.deltaTime * transitionSpeed);
                currentObject.rotation = Quaternion.Lerp(currentObject.rotation, originalObjectRotation, Time.deltaTime * transitionSpeed);

                if (Vector3.Distance(currentObject.position, originalObjectPosition) < returnThreshold &&
                    Quaternion.Angle(currentObject.rotation, originalObjectRotation) < returnThreshold)
                {
                    isReturning = false;

                    if (cameraController != null)
                    {
                        cameraController.canLook = true;
                    }
                    if (playerController != null)
                    {
                        playerController.enabled = true;
                    }
                }
            }
        }
    }

    private void CheckForInspectableObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (var obj in inspectableObjects)
            {
                if (hit.transform == obj.objectTransform)
                {
                    float distance = Vector3.Distance(mainCamera.transform.position, obj.objectTransform.position);
                    if (distance <= maxInspectDistance)
                    {
                        if (currentObject == obj.objectTransform)
                        {
                            return;
                        }

                        // Отключаем курсор, если объект найден
                        if (cursor != null)
                        {
                            cursor.SetActive(false);
                        }

                        currentObject = obj.objectTransform;

                        originalObjectPosition = currentObject.position;
                        originalObjectRotation = currentObject.rotation;

                        return;
                    }
                }
            }
        }

        // Включаем курсор только если он не был изначально отключен
        if (cursor != null && !cursorInitiallyDisabled)
        {
            cursor.SetActive(true);
        }

        currentObject = null;
    }

    private void EnterInspectMode()
    {
        isInspecting = true;

        if (cursor != null)
        {
            cursor.SetActive(false);
        }

        opisanie.SetActive(true);

        if (exitObject != null)
        {
            exitObject.SetActive(true);
        }

        foreach (var obj in inspectableObjects)
        {
            if (currentObject == obj.objectTransform)
            {
                opisanieName.text = obj.objectName;
                opisanieDescr.text = obj.objectDescription;
                break;
            }
        }

        if (cameraController != null)
        {
            cameraController.canLook = false;
        }
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        targetPosition = mainCamera.transform.position +
                         mainCamera.transform.forward * inspectionOffset.z +
                         mainCamera.transform.up * inspectionOffset.y +
                         mainCamera.transform.right * inspectionOffset.x;

        // Переворачиваем объект на 180 градусов по оси X (или другой оси, если нужно)
        targetRotation = Quaternion.LookRotation(mainCamera.transform.up, -mainCamera.transform.forward) *
                         Quaternion.Euler(0f, 180f, 0f); // Поворот на 180 градусов по оси X

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Воспроизводим звук при входе в инспект
        PlaySound(pickSound);
    }

    private void ExitInspectMode()
    {
        isInspecting = false;
        isReturning = true;

        opisanie.SetActive(false);

        if (exitObject != null)
        {
            exitObject.SetActive(false);
        }

        // Включаем курсор только если он не был изначально отключен
        if (cursor != null && !cursorInitiallyDisabled)
        {
            cursor.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Воспроизводим звук при выходе из инспекта
        PlaySound(dropSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // Воспроизводим звук с уменьшенной громкостью
            audioSource.PlayOneShot(clip, audioVolume);
        }
    }
}

[System.Serializable]
public class InspectableObject
{
    public Transform objectTransform;
    public string objectName;
    public string objectDescription;
    public GameObject textPaper; // Добавлено для хранения Textpapper
}