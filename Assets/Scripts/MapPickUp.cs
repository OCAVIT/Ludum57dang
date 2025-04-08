using UnityEngine;

public class MapInteraction : MonoBehaviour
{
    public GameObject map; // GameObject Map
    public GameObject cursor; // GameObject Cursor
    public GameObject handsMap; // GameObject HandsMap
    public GameObject wallBlock3; // GameObject WallBlock3
    public AudioClip pickupSound; // Звук при подборе карты

    public Camera playerCamera; // Камера игрока (назначается в инспекторе)
    public float interactionDistance = 3f; // Максимальная дистанция взаимодействия

    private bool isLookingAtMap = false; // Флаг, смотрит ли игрок на карту
    private AudioSource audioSource; // Источник звука
    private bool mapPickedUp = false; // Флаг, показывающий, была ли карта подобрана

    void Start()
    {
        // Проверяем, назначена ли камера
        if (playerCamera == null)
        {
            Debug.LogError("Камера не назначена! Убедитесь, что вы указали камеру в инспекторе.");
        }

        // Добавляем или находим компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Убедимся, что курсор настроен правильно
        if (cursor != null) cursor.SetActive(true);
    }

    void Update()
    {
        // Проверяем, активна ли камера игрока
        if (playerCamera == null || !playerCamera.gameObject.activeInHierarchy)
        {
            return; // Если камера неактивна, ничего не делаем
        }

        // Если карта уже подобрана, ничего не делаем
        if (mapPickedUp)
        {
            return;
        }

        // Создаем луч из позиции камеры в направлении её взгляда
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Проверяем, попадает ли луч в объект
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider != null && hit.collider.gameObject == map)
            {
                // Если игрок смотрит на карту
                if (!isLookingAtMap)
                {
                    Debug.Log("Смотрим на карту");
                    cursor.SetActive(false);
                    isLookingAtMap = true;
                }

                // Если игрок нажимает "E" и смотрит на карту
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Подбираем карту");
                    PickUpMap();
                }
            }
            else
            {
                // Если игрок перестал смотреть на карту
                ResetLookState();
            }
        }
        else
        {
            // Если луч никуда не попал, сбрасываем состояние
            ResetLookState();
        }
    }

    private void ResetLookState()
    {
        if (isLookingAtMap)
        {
            Debug.Log("Перестали смотреть на карту");
            cursor.SetActive(true);
            isLookingAtMap = false;
        }
    }

    private void PickUpMap()
    {
        // Делаем Map неактивным
        if (map != null) map.SetActive(false);

        // Включаем HandsMap
        if (handsMap != null) handsMap.SetActive(true);

        // Включаем Cursor
        if (cursor != null) cursor.SetActive(true);

        // Делаем WallBlock3 неактивным
        if (wallBlock3 != null) wallBlock3.SetActive(false);

        // Воспроизводим звук
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Устанавливаем флаг, что карта была подобрана
        mapPickedUp = true;
    }
}