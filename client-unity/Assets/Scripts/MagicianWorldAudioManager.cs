using SpacetimeDB.Types;
using UnityEngine;

public class MagicianWorldAudioManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] AudioSource KinematicSource;

    [Header("Clips")]
    [SerializeField] AudioClip WalkingClip;
    [SerializeField] AudioClip SprintingClip;

    [Header("Tuning")]
    [SerializeField] float MaxAudibleDistance = 20f;
    MagicianController Controller;

    void Awake()
    {
        if (KinematicSource != null)
            KinematicSource.loop = true;
    }

    void OnDisable()
    {
        StopKinematicStateSound();
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
            return;
        }

        Magician player = Controller.Magician;
        KinematicInformation state = player.KinematicInformation;
        DbVector3 velocity = player.CorrectedVelocity;
        bool isMoving = velocity.X != 0f || velocity.Z != 0f;

        if (state.Crouched || !isMoving)
        {
            StopKinematicStateSound();
            return;
        }

        float distance = Vector3.Distance(
            Controller.transform.position,
            MatchManager.Instance.ClientMagician.transform.position
        );

        if (distance > MaxAudibleDistance)
        {
            StopKinematicStateSound();
            return;
        }

        AudioClip clip = state.Sprinting ? SprintingClip : WalkingClip;
        if (clip == null)
        {
            StopKinematicStateSound();
            return;
        }

        if (KinematicSource.clip != clip)
        {
            KinematicSource.Stop();
            KinematicSource.clip = clip;
        }

        if (!KinematicSource.isPlaying)
            KinematicSource.Play();
    }

    public void StopKinematicStateSound()
    {
        if (KinematicSource == null)
            return;

        KinematicSource.Stop();
        KinematicSource.clip = null;
    }
}
