using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("���������")]
    public Camera playerCamera; // ������ ������
    public float interactionDistance = 5f; // ������������ ��������� ��������������
    public AudioClip pickUpSound; // ���� "Pick Up"
    public GameObject[] interactableObjects; // ������ �������� ��� ��������������

    private AudioSource audioSource;

    void Start()
    {
        // ��������� ��� ������� ��������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ���������, ������ �� ������ "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForInteraction();
        }
    }

    void CheckForInteraction()
    {
        // ������� ��� �� ������ ������
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // ���������, ����� �� ��� � ������
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // ���������, �������� �� ������ � ������� interactableObjects
            foreach (GameObject obj in interactableObjects)
            {
                if (hit.collider.gameObject == obj)
                {
                    // ������������ ������
                    obj.SetActive(false);

                    // ������������� ����
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