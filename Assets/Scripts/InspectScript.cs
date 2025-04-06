using UnityEngine;
using TMPro;

public class PaperInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GameObject cursor;
    public GameObject inspectText;
    public GameObject opisanie;
    public TMP_Text opisanieName;
    public TMP_Text opisanieDescr;
    public FirstPersonController cameraController;
    public CharacterController playerController;
    public GameObject exitObject;

    [Header("Inspection Settings")]
    public Vector3 inspectionOffset = new Vector3(-0.5f, 0, 2f);
    public float maxInspectDistance = 3f;
    public float transitionSpeed = 5f;
    public float returnThreshold = 0.01f;

    [Header("Inspectable Objects")]
    public InspectableObject[] inspectableObjects;

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
        inspectText.SetActive(false);
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

                        inspectText.SetActive(true);

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

        inspectText.SetActive(false);

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

        inspectText.SetActive(false);
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

        targetRotation = Quaternion.LookRotation(mainCamera.transform.up, -mainCamera.transform.forward);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    }
}

[System.Serializable]
public class InspectableObject
{
    public Transform objectTransform;
    public string objectName;
    public string objectDescription;
}