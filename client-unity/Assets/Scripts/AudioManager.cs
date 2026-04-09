using System.Collections;
using System.Collections.Generic;
using SpacetimeDB.Types;
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
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayBodyshotSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Bodyshot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayAttackSound(bool IsOwner) {
        if (IsOwner) {
            switch (UseAttackOneSound) {
                case true:
                    if (Sounds.TryGetValue("Attack One", out AudioClip attack) && attack != null)
                        Source.PlayOneShot(attack, volumeScale: 0.8f);

                    UseAttackOneSound = !UseAttackOneSound;
                    break;
                
                case false:
                    if (Sounds.TryGetValue("Attack Two", out AudioClip attack_2) && attack_2 != null)
                        Source.PlayOneShot(attack_2, volumeScale: 0.5f); 

                    UseAttackOneSound = !UseAttackOneSound;
                    break;
            } 
        }
    }

    public void PlayTarotCastSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Tarot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayTarotReceiveSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Tarot", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayDustCastSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Dust Cast", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayHypnosisSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Hypnosis", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, 0.5f);  
        }
    }

    public void PlayCloakSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Cloak", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.25f);  
        }
    }

    public void PlayStunnedSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Stunned", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayUnavailableActionSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Unavailable Action", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.25f);  
        }
    }

    public void PlayTakeDamageSound(bool IsOwner) {
        if (IsOwner) {
            switch (UseTakeDamageOneSound) {
                case true:
                    if (Sounds.TryGetValue("Take Damage One", out AudioClip take_damage) && take_damage != null)
                        Source.PlayOneShot(take_damage, volumeScale: 0.25f);

                    UseTakeDamageOneSound = !UseTakeDamageOneSound;
                    break;
                
                case false:
                    if (Sounds.TryGetValue("Take Damage Two", out AudioClip take_damage_2) && take_damage_2 != null)
                        Source.PlayOneShot(take_damage_2, volumeScale: 0.25f); 

                    UseTakeDamageOneSound = !UseTakeDamageOneSound;
                    break;
            } 
        }
    }

    public void PlayJumpSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Jump", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.5f);  
        }
    }

    public void PlayLandSound(bool IsOwner) {
        if (IsOwner) {
            if (Sounds.TryGetValue("Land", out AudioClip clip) && clip != null)
                Source.PlayOneShot(clip, volumeScale: 0.05f);  
        }
    }

}

[System.Serializable]
public class SoundEntry
{
    public string Name;
    public AudioClip Clip;
}
