using SpacetimeDB.Types;
using UnityEngine;

public class MagicianWorldAudioManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] AudioSource KinematicSource;
    [SerializeField] AudioSource OneShotSource;

    [Header("Clips")]
    [SerializeField] AudioClip WalkingClip;
    [SerializeField] AudioClip SprintingClip;
    [SerializeField] AudioClip JumpClip;
    [SerializeField] AudioClip LandClip;

    [Header("Loop Tuning")]
    [SerializeField] float MaxAudibleDistance = 20f;
    [SerializeField] float OwnerVolume = 0.08f;
    [SerializeField] float MaxWorldVolume = 0.45f;
    [SerializeField] float MinMovementSpeed = 0.08f;
    [SerializeField] float WalkingPitch = 0.75f;
    [SerializeField] float SprintingPitch = 0.75f;
    [SerializeField] float StopGraceTime = 0.15f;

    [Header("One Shot Tuning")]
    [SerializeField] float JumpOwnerVolume = 0.10f;
    [SerializeField] float JumpWorldVolume = 0.40f;
    [SerializeField] float JumpMaxAudibleDistance = 16f;
    [SerializeField] float LandOwnerVolume = 0.12f;
    [SerializeField] float LandWorldVolume = 0.50f;
    [SerializeField] float LandMaxAudibleDistance = 18f;

    MagicianController Controller;
    float InvalidKinematicTime;

    void Awake()
    {
        if (KinematicSource != null)
            KinematicSource.loop = true;

        if (OneShotSource != null)
            OneShotSource.loop = false;
    }

    void OnDisable()
    {
        StopKinematicStateSound();
        InvalidKinematicTime = 0f;
    }

    public void Initialize(MagicianController controller)
    {
        Controller = controller;
    }

    public void UpdateKinematicStateSound()
    {
        if (Controller == null || KinematicSource == null || MatchManager.Instance == null || MatchManager.Instance.ClientMagician == null)
        {
            StopKinematicStateSound();
            InvalidKinematicTime = 0f;
            return;
        }

        Magician Player = Controller.Magician;
        KinematicInformation State = Player.KinematicInformation;
        DbVector3 Velocity = Player.CorrectedVelocity;

        float PlanarSpeedSquared = (Velocity.X * Velocity.X) + (Velocity.Z * Velocity.Z);
        float MinMovementSpeedSquared = MinMovementSpeed * MinMovementSpeed;
        bool IsMoving = PlanarSpeedSquared > MinMovementSpeedSquared;
        bool IsCloaked = HasCloakEffect();

        if (IsCloaked)
        {
            StopKinematicStateSound();
            InvalidKinematicTime = 0f;
            return;
        }

        bool IsInvalidKinematicState = !State.Grounded || State.Crouched || !IsMoving;

        if (Controller.IsOwner == false)
        {
            float Distance = Vector3.Distance(
                Controller.transform.position,
                MatchManager.Instance.ClientMagician.transform.position
            );

            if (Distance > MaxAudibleDistance)
            {
                StopKinematicStateSound();
                InvalidKinematicTime = 0f;
                return;
            }
        }

        if (IsInvalidKinematicState)
        {
            InvalidKinematicTime += Time.deltaTime;

            if (InvalidKinematicTime >= StopGraceTime)
                StopKinematicStateSound();

            return;
        }

        InvalidKinematicTime = 0f;

        AudioClip DesiredClip = State.Sprinting ? SprintingClip : WalkingClip;
        if (DesiredClip == null)
        {
            StopKinematicStateSound();
            return;
        }

        KinematicSource.pitch = State.Sprinting ? SprintingPitch : WalkingPitch;

        float DesiredVolume;

        if (Controller.IsOwner)
        {
            DesiredVolume = OwnerVolume;
        }
        else
        {
            float Distance = Vector3.Distance(
                Controller.transform.position,
                MatchManager.Instance.ClientMagician.transform.position
            );

            float DistanceAlpha = 1f - Mathf.Clamp01(Distance / MaxAudibleDistance);
            DesiredVolume = MaxWorldVolume * DistanceAlpha;
        }

        KinematicSource.volume = DesiredVolume;

        if (KinematicSource.clip != DesiredClip)
        {
            KinematicSource.Stop();
            KinematicSource.clip = DesiredClip;
            KinematicSource.Play();
            return;
        }

        if (!KinematicSource.isPlaying)
            KinematicSource.Play();
    }

    public void PlayJumpSound()
    {
        PlayOneShot(JumpClip, JumpOwnerVolume, JumpWorldVolume, JumpMaxAudibleDistance);
    }

    public void PlayLandSound()
    {
        PlayOneShot(LandClip, LandOwnerVolume, LandWorldVolume, LandMaxAudibleDistance);
    }

    void PlayOneShot(AudioClip Clip, float OwnerOneShotVolume, float WorldOneShotVolume, float MaxDistance)
    {
        if (Controller == null || OneShotSource == null || MatchManager.Instance == null || MatchManager.Instance.ClientMagician == null)
            return;

        if (Clip == null)
            return;

        if (HasCloakEffect())
            return;

        float DesiredVolume;

        if (Controller.IsOwner)
        {
            DesiredVolume = OwnerOneShotVolume;
        }
        else
        {
            float Distance = Vector3.Distance(
                Controller.transform.position,
                MatchManager.Instance.ClientMagician.transform.position
            );

            if (Distance > MaxDistance)
                return;

            float DistanceAlpha = 1f - Mathf.Clamp01(Distance / MaxDistance);
            DesiredVolume = WorldOneShotVolume * DistanceAlpha;
        }

        OneShotSource.PlayOneShot(Clip, DesiredVolume);
    }

    bool HasCloakEffect()
    {
        foreach (PlayerEffect Effect in GameManager.Conn.Db.PlayerEffects.TargetId.Filter(Controller.Id))
        {
            if (Effect.EffectType is EffectType.Cloak)
                return true;
        }

        return false;
    }

    public void StopKinematicStateSound()
    {
        if (KinematicSource == null)
            return;

        KinematicSource.Stop();
    }
}