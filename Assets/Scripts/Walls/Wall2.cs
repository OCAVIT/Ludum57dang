using UnityEngine;
using TMPro;

public class WallVisibilityControllerWithHurtedPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player; // ������ �� ������

    [Header("Wall Settings")]
    public float visibilityDistance = 5f; // ���������, �� ������� ����� ���������� ��������� �������
    public float fadeSpeed = 2f; // �������� ��������� ������������
    public Material wallMaterial; // �������� �����
    public float maxWallAlpha = 0.5f; // ������������ �������������� ����� (0 = ��������� ����������, 1 = ��������� ������������)

    [Header("Text Settings")]
    public TMP_Text[] textMeshProObjects; // ������ TMP_Text ��������
    public float maxTextAlpha = 1f; // ������������ �������������� ������ (0 = ��������� ����������, 1 = ��������� ������������)
    public float textAlphaInvisible = 0f; // �����-����� ������, ����� �� �������

    [Header("HurtedPlayer Settings")]
    public GameObject HurtedPlayer; // ������ �� ������ HurtedPlayer

    [Header("Additional Objects")]
    public GameObject BINT; // ������ �� ������ BINT
    public GameObject WallBlock2; // ������ �� ������ WallBlock2

    private float currentWallAlpha = 0f; // ������� ������������ �����
    private float currentTextAlpha = 0f; // ������� ������������ ������

    void Start()
    {
        // ������������� ��������� �����-����� �����
        if (wallMaterial != null)
        {
            SetWallAlpha(0f); // ������ ����� ���������� ���������
        }

        // ������������� ����� ���������� ���������
        if (textMeshProObjects != null && textMeshProObjects.Length > 0)
        {
            foreach (TMP_Text textMeshPro in textMeshProObjects)
            {
                SetTextAlpha(textMeshPro, textAlphaInvisible);
            }
        }
    }

    void Update()
    {
        if (HurtedPlayer == null) return;

        // ���� HurtedPlayer ���������, ������ �� ������
        if (!HurtedPlayer.activeSelf)
        {
            return; // ����, ���� HurtedPlayer ������ ��������
        }

        // ��������� ��������� BINT
        if (BINT != null && WallBlock2 != null)
        {
            if (!BINT.activeSelf)
            {
                WallBlock2.SetActive(false); // ������ WallBlock2 ����������
            }
        }

        // �������� ������ ������ �� ������
        if (player == null || wallMaterial == null || textMeshProObjects == null) return;

        // ������������ ���������� ����� ������� � ������
        float distance = Vector3.Distance(player.position, transform.position);

        // ������������ ������� ������������ � ����������� �� ����������
        float targetAlpha = Mathf.Clamp01(1f - (distance / visibilityDistance));

        // ������ �������� ������������ �����
        currentWallAlpha = Mathf.Lerp(currentWallAlpha, targetAlpha * maxWallAlpha, Time.deltaTime * fadeSpeed);
        SetWallAlpha(currentWallAlpha);

        // ������ �������� ������������ ������
        currentTextAlpha = Mathf.Lerp(currentTextAlpha, targetAlpha * maxTextAlpha, Time.deltaTime * fadeSpeed);
        foreach (TMP_Text textMeshPro in textMeshProObjects)
        {
            SetTextAlpha(textMeshPro, currentTextAlpha);
        }
    }

    // ����� ��� ��������� �����-������ �����
    private void SetWallAlpha(float alpha)
    {
        if (wallMaterial == null) return;

        // �������� ������� ���� ���������
        Color color = wallMaterial.color;

        // �������� ������ �����-�����, �������� ��������� ���������� ����� �����������
        color.a = alpha;

        // ��������� ���������� ���� ������� � ���������
        wallMaterial.color = color;
    }

    // ����� ��� ��������� �����-������ ������
    private void SetTextAlpha(TMP_Text textMeshPro, float alpha)
    {
        if (textMeshPro == null) return;

        // �������� ������� ���� ������
        Color color = textMeshPro.color;

        // �������� ������ �����-�����, �������� ��������� ���������� ����� �����������
        color.a = alpha;

        // ��������� ���������� ���� ������� � ������
        textMeshPro.color = color;
    }
}