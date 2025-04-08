using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public GameObject pixake; // ������ Pixake
    public GameObject pixakeHand; // ������ PixakeHand
    public GameObject shel; // ������ Shel

    public AudioClip pickUpIronClip; // ��������� "pick up iron"
    public AudioClip kickShelClip; // ��������� "KickShel"

    public Camera playerCamera; // ������ ������
    public float interactionDistance = 5f; // ������������ ��������� ��������������

    private AudioSource audioSource; // ������������� ��� ��������������� ������

    private void Start()
    {
        // ���������, ���� �� AudioSource �� �������, � ��������� ���, ���� �����������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ��������, ��� PixakeHand �������� � ������
        if (pixakeHand != null)
        {
            pixakeHand.SetActive(false);
        }

        // ���������, ��������� �� ������ ������
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        // ���������, ������� �� ����� �� Pixake
        if (IsLookingAtObject(pixake) && Input.GetKeyDown(KeyCode.E))
        {
            PickUpPixake();
        }

        // ���������, ������� �� ����� �� Shel � ������ �� PixakeHand
        if (IsLookingAtObject(shel) && pixakeHand.activeSelf && Input.GetMouseButtonDown(0))
        {
            KickShel();
        }
    }

    private void PickUpPixake()
    {
        if (pixake != null)
        {
            pixake.SetActive(false); // ������������ Pixake
        }

        if (pixakeHand != null)
        {
            pixakeHand.SetActive(true); // ���������� PixakeHand
        }

        if (audioSource != null && pickUpIronClip != null)
        {
            audioSource.PlayOneShot(pickUpIronClip); // ������������� ���� "pick up iron"
        }
    }

    private void KickShel()
    {
        if (shel != null)
        {
            shel.SetActive(false); // ������������ Shel
        }

        if (pixakeHand != null)
        {
            pixakeHand.SetActive(false); // ������������ PixakeHand
        }

        if (audioSource != null && kickShelClip != null)
        {
            audioSource.PlayOneShot(kickShelClip); // ������������� ���� "KickShel"
        }
    }

    // ���������, ������� �� ������ ������ �� ������
    private bool IsLookingAtObject(GameObject targetObject)
    {
        if (playerCamera == null || targetObject == null)
        {
            return false;
        }

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // ��� �� ������ ������
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