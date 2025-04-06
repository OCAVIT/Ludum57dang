using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3[] cameraPositions;
    public float moveSpeed = 2f;
    public Light spotLight;

    public AudioClip[] hitSounds;
    public AudioClip lightOnSound;
    public AudioClip lightOffSound;
    public AudioClip screamSound;
    public AudioClip transFallSound;
    private AudioSource audioSource;
    public GameObject cursor;

    public TMP_Text hintText;

    public GameObject player;
    public GameObject animPlayer;

    public GameObject hurtScriptManager;

    private int currentPositionIndex = 0;
    private bool isMoving = false;

    private bool isFadingOut = false;
    public float blinkSpeed = 1f;

    void Start()
    {
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (cursor != null)
        {
            cursor.SetActive(false);
        }

        if (hintText != null)
        {
            hintText.text = "Press \"W\" To Move";

            Color visibleColor = hintText.color;
            visibleColor.a = 1f;
            hintText.color = visibleColor;
        }

        if (animPlayer != null)
        {
            animPlayer.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMoving)
        {

            if (currentPositionIndex < cameraPositions.Length - 1)
            {
                currentPositionIndex++;
                StartCoroutine(MoveCameraToPosition(cameraPositions[currentPositionIndex]));
            }

            if (currentPositionIndex == cameraPositions.Length - 1)
            {
                OnPlayerReachedEnd();
            }

            if (currentPositionIndex == 6)
            {
                StartCoroutine(SimulateLightFlicker());
            }
        }

        if (hintText != null)
        {
            Color currentColor = hintText.color;

            if (isFadingOut)
            {
                currentColor.a -= blinkSpeed * Time.deltaTime;
                if (currentColor.a <= 0f)
                {
                    currentColor.a = 0f;
                    isFadingOut = false;
                }
            }
            else
            {
                currentColor.a += blinkSpeed * Time.deltaTime;
                if (currentColor.a >= 1f)
                {
                    currentColor.a = 1f;
                    isFadingOut = true;
                }
            }

            hintText.color = currentColor;
        }
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPosition)
    {
        isMoving = true;

        if (currentPositionIndex < cameraPositions.Length - 1)
        {
            PlayRandomHitSound();
        }

        Vector3 startPosition = cameraTransform.position;
        float journeyProgress = 0f;

        float duration = Vector3.Distance(startPosition, targetPosition) / moveSpeed;

        while (journeyProgress < 1f)
        {
            journeyProgress += Time.deltaTime / duration;
            float smoothedProgress = Mathf.SmoothStep(0f, 1f, journeyProgress);

            float bobbingFrequency = 2f;
            float verticalBobbingAmplitude = 0.05f;
            float horizontalBobbingAmplitude = 0.03f;

            float verticalOffset = Mathf.Sin(journeyProgress * Mathf.PI * bobbingFrequency) * verticalBobbingAmplitude;
            float horizontalOffset = Mathf.Cos(journeyProgress * Mathf.PI * bobbingFrequency) * horizontalBobbingAmplitude;

            Vector3 bobbingOffset = new Vector3(horizontalOffset, verticalOffset, 0f);

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, smoothedProgress) + bobbingOffset;
            cameraTransform.position = newPosition;

            yield return null;
        }

        cameraTransform.position = targetPosition;
        isMoving = false;
    }

    private IEnumerator SimulateLightFlicker()
    {
        if (spotLight != null)
        {
            Debug.Log("Фонарик замыкает!");

            float flickerDuration = 1f;
            float flickerInterval = 0.1f;
            float elapsedTime = 0f;

            if (hintText != null)
            {
                hintText.text = "Keep Press \"W\" To Move";
            }

            while (elapsedTime < flickerDuration)
            {
                spotLight.enabled = !spotLight.enabled;

                if (spotLight.enabled)
                {
                    PlaySound(lightOnSound);
                }
                else
                {
                    PlaySound(lightOffSound);
                }

                yield return new WaitForSeconds(flickerInterval);
                elapsedTime += flickerInterval;
            }

            spotLight.enabled = false;
            PlaySound(lightOffSound);
            Debug.Log("Фонарик перегорел!");

            if (hintText != null)
            {
                hintText.text = "Keep Press \"W\" To Move";
            }
        }
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[randomIndex], 0.5f);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnPlayerReachedEnd()
    {
        if (player != null)
        {
            player.SetActive(false);
        }

        if (animPlayer != null)
        {
            animPlayer.SetActive(true);
        }

        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }

        if (audioSource != null && screamSound != null)
        {
            audioSource.PlayOneShot(screamSound);
        }

        Debug.Log("Игрок достиг конца!");
    }

    public void PlayTransFallSound()
    {
        if (transFallSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transFallSound);
            Debug.Log("TransFall звук воспроизведен!");
            StartCoroutine(WaitForSoundToEnd());
        }
    }

    private IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        this.enabled = false;

        if (hurtScriptManager != null)
        {
            hurtScriptManager.gameObject.SetActive(true);
        }
    }
}