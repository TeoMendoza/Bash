using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    
    [SerializeField] AudioSource Source;
    [SerializeField] List<SoundEntry> SoundsList;

    Dictionary<string, AudioClip> Sounds;
    private bool UseAttackOneSound = true;
    private bool UseTakeDamageOneSound = true;

    void Awake()
    {
        Sounds = new Dictionary<string, AudioClip>();

        foreach (SoundEntry Sound in SoundsList) {
            Sounds[Sound.Name] = Sound.Clip;
        }
    }

    public void PlayHeadshotSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Headshot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayBodyshotSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Bodyshot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayAttackSound(bool IsOwner) {
        if (IsOwner) {
            switch (UseAttackOneSound) {
                case true:
                    if (Sounds.TryGetValue("Attack One", out AudioClip attack) && attack != null)
                        Source.PlayOneShot(attack);

                    UseAttackOneSound = !UseAttackOneSound;
                    break;
                
                case false:
                    if (Sounds.TryGetValue("Attack Two", out AudioClip attack_2) && attack_2 != null)
                        Source.PlayOneShot(attack_2); 

                    UseAttackOneSound = !UseAttackOneSound;
                    break;
            } 
        }
    }

    public void PlayTarotCastSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Tarot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayTarotReceiveSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Tarot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayDustCastSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Dust Cast", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayDustReceiveSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Dust Receive", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayHypnosisSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Hypnosis", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayCloakSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Cloak", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayStunnedSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Stunned", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip);  
        }
    }

    public void PlayTakeDamageSound(bool IsOwner) {
        if (IsOwner) {
            switch (UseTakeDamageOneSound) {
                case true:
                    if (Sounds.TryGetValue("Take Damage One", out AudioClip take_damage) && take_damage != null)
                        Source.PlayOneShot(take_damage);

                    UseTakeDamageOneSound = !UseTakeDamageOneSound;
                    break;
                
                case false:
                    if (Sounds.TryGetValue("Take Damage Two", out AudioClip take_damage_2) && take_damage_2 != null)
                        Source.PlayOneShot(take_damage_2); 

                    UseTakeDamageOneSound = !UseTakeDamageOneSound;
                    break;
            } 
        }
    }

}

[System.Serializable]
public class SoundEntry
{
    public string Name;
    public AudioClip Clip;
}
