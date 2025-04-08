using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Настройки")]
    public Camera playerCamera; // Камера игрока
    public float interactionDistance = 5f; // Максимальная дистанция взаимодействия
    public AudioClip pickUpSound; // Звук "Pick Up"
    public GameObject[] interactableObjects; // Массив объектов для взаимодействия

    private AudioSource audioSource;

    void Start()
    {
        // Добавляем или находим компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Проверяем, нажата ли кнопка "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForInteraction();
        }
    }

    void CheckForInteraction()
    {
        // Создаем луч из центра камеры
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Проверяем, попал ли луч в объект
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Проверяем, является ли объект в массиве interactableObjects
            foreach (GameObject obj in interactableObjects)
            {
                if (hit.collider.gameObject == obj)
                {
                    // Деактивируем объект
                    obj.SetActive(false);

                    // Воспроизводим звук
                    if (pickUpSound != null)
                    {
                        audioSource.PlayOneShot(pickUpSound);
                    }

                    break;
                }
            }
        }
    }
}