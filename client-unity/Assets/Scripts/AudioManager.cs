using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioSource Source;
    [SerializeField] AudioClip Headshot;

    public void PlayHeadshotSound(bool IsOwner) {
        if (IsOwner)
            Source.PlayOneShot(Headshot);
    }
}
