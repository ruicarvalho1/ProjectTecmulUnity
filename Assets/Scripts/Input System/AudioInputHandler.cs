using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInputHandler : MonoBehaviour
{
    public AudioClip movePieceSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMoveSound()
    {
        if (movePieceSound != null)
        {
            Debug.Log("Som de movimento chamado!");
            audioSource.PlayOneShot(movePieceSound);
        }
    }
}