using UnityEngine;

public class MapInteraction : MonoBehaviour
{
    public GameObject map; // GameObject Map
    public GameObject cursor; // GameObject Cursor
    public GameObject handsMap; // GameObject HandsMap
    public GameObject wallBlock3; // GameObject WallBlock3
    public AudioClip pickupSound; // ���� ��� ������� �����

    public Camera playerCamera; // ������ ������ (����������� � ����������)
    public float interactionDistance = 3f; // ������������ ��������� ��������������

    private bool isLookingAtMap = false; // ����, ������� �� ����� �� �����
    private AudioSource audioSource; // �������� �����
    private bool mapPickedUp = false; // ����, ������������, ���� �� ����� ���������

    void Start()
    {
        // ���������, ��������� �� ������
        if (playerCamera == null)
        {
            Debug.LogError("������ �� ���������! ���������, ��� �� ������� ������ � ����������.");
        }

        // ��������� ��� ������� ��������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ��������, ��� ������ �������� ���������
        if (cursor != null) cursor.SetActive(true);
    }

    void Update()
    {
        // ���������, ������� �� ������ ������
        if (playerCamera == null || !playerCamera.gameObject.activeInHierarchy)
        {
            return; // ���� ������ ���������, ������ �� ������
        }

        // ���� ����� ��� ���������, ������ �� ������
        if (mapPickedUp)
        {
            return;
        }

        // ������� ��� �� ������� ������ � ����������� � �������
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // ���������, �������� �� ��� � ������
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider != null && hit.collider.gameObject == map)
            {
                // ���� ����� ������� �� �����
                if (!isLookingAtMap)
                {
                    Debug.Log("������� �� �����");
                    cursor.SetActive(false);
                    isLookingAtMap = true;
                }

                // ���� ����� �������� "E" � ������� �� �����
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("��������� �����");
                    PickUpMap();
                }
            }
            else
            {
                // ���� ����� �������� �������� �� �����
                ResetLookState();
            }
        }
        else
        {
            // ���� ��� ������ �� �����, ���������� ���������
            ResetLookState();
        }
    }

    private void ResetLookState()
    {
        if (isLookingAtMap)
        {
            Debug.Log("��������� �������� �� �����");
            cursor.SetActive(true);
            isLookingAtMap = false;
        }
    }

    private void PickUpMap()
    {
        // ������ Map ����������
        if (map != null) map.SetActive(false);

        // �������� HandsMap
        if (handsMap != null) handsMap.SetActive(true);

        // �������� Cursor
        if (cursor != null) cursor.SetActive(true);

        // ������ WallBlock3 ����������
        if (wallBlock3 != null) wallBlock3.SetActive(false);

        // ������������� ����
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // ������������� ����, ��� ����� ���� ���������
        mapPickedUp = true;
    }
}