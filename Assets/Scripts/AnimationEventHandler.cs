using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public GameManager gameManager; // Ссылка на GameManager

    public void PlayTransFallSound()
    {
        if (gameManager != null)
        {
            gameManager.PlayTransFallSound(); // Вызываем метод из GameManager
        }
        else
        {
            Debug.LogWarning("GameManager не назначен в AnimationEventHandler!");
        }
    }
}