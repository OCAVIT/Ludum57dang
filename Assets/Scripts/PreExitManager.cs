using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public GameObject pixake; // Объект Pixake
    public GameObject pixakeHand; // Объект PixakeHand
    public GameObject shel; // Объект Shel

    public AudioClip pickUpIronClip; // Аудиоклип "pick up iron"
    public AudioClip kickShelClip; // Аудиоклип "KickShel"

    public Camera playerCamera; // Камера игрока
    public float interactionDistance = 5f; // Максимальная дистанция взаимодействия

    private AudioSource audioSource; // Аудиоисточник для воспроизведения звуков

    private void Start()
    {
        // Проверяем, есть ли AudioSource на объекте, и добавляем его, если отсутствует
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Убедимся, что PixakeHand выключен в начале
        if (pixakeHand != null)
        {
            pixakeHand.SetActive(false);
        }

        // Проверяем, назначена ли камера игрока
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        // Проверяем, смотрит ли игрок на Pixake
        if (IsLookingAtObject(pixake) && Input.GetKeyDown(KeyCode.E))
        {
            PickUpPixake();
        }

        // Проверяем, смотрит ли игрок на Shel и держит ли PixakeHand
        if (IsLookingAtObject(shel) && pixakeHand.activeSelf && Input.GetMouseButtonDown(0))
        {
            KickShel();
        }
    }

    private void PickUpPixake()
    {
        if (pixake != null)
        {
            pixake.SetActive(false); // Деактивируем Pixake
        }

        if (pixakeHand != null)
        {
            pixakeHand.SetActive(true); // Активируем PixakeHand
        }

        if (audioSource != null && pickUpIronClip != null)
        {
            audioSource.PlayOneShot(pickUpIronClip); // Воспроизводим звук "pick up iron"
        }
    }

    private void KickShel()
    {
        if (shel != null)
        {
            shel.SetActive(false); // Деактивируем Shel
        }

        if (pixakeHand != null)
        {
            pixakeHand.SetActive(false); // Деактивируем PixakeHand
        }

        if (audioSource != null && kickShelClip != null)
        {
            audioSource.PlayOneShot(kickShelClip); // Воспроизводим звук "KickShel"
        }
    }

    // Проверяем, смотрит ли камера игрока на объект
    private bool IsLookingAtObject(GameObject targetObject)
    {
        if (playerCamera == null || targetObject == null)
        {
            return false;
        }

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Луч из центра экрана
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == targetObject)
            {
                return true;
            }
        }

        return false;
    }
}