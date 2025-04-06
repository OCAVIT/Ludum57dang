using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public GameManager gameManager; // ������ �� GameManager

    public void PlayTransFallSound()
    {
        if (gameManager != null)
        {
            gameManager.PlayTransFallSound(); // �������� ����� �� GameManager
        }
        else
        {
            Debug.LogWarning("GameManager �� �������� � AnimationEventHandler!");
        }
    }
}