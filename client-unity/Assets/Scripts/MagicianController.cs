using SpacetimeDB;
using UnityEngine;
using SpacetimeDB.Types;
using Unity.Cinemachine;
using System;

public class MagicianController : MonoBehaviour
{
    private static readonly int UnavailableActionHash = Animator.StringToHash("UnavailableAction");
    [Header("Scene References")]
    [SerializeField] CinemachineCamera ThirdPersonCam;
    [SerializeField] GameObject ThirdPersonCamPivot;
    [SerializeField] Canvas Crosshair;
    [SerializeField] MagicianHUDController MagicianHud;
    [SerializeField] MagicianOutwardHudController MagicianOutwardHud;
    [SerializeField] MagicianWorldAudioManager WorldAudioManager;

    [Header("Tuning")]
    public float SendRateHz = 30f;
    public float AimYawThresholdDegrees = 0.75f;
    public float AimPitchThresholdDegrees = 0.75f;

    readonly float SensX = 2f;
    readonly float SensY = 1f;
    readonly float MinPitch = -50f;
    readonly float MaxPitch = 75f;

    readonly float PitchSmooth = 0.08f;
    readonly float SpeedBlendTime = 0.15f;

    public Magician Magician;
    Camera MainCamera;

    public bool IsOwner;
    public bool InputEnabled = false;

    public Identity Identity;
    public ulong Id;
    public string Name;
    public uint MatchId;

    public Vector3 TargetPosition;
    public DbRotation2 TargetRotation = new(0,0);
    public KinematicInformation KinematicInformation;
    public Animator Animator;

    float Yaw;
    float Pitch;

    float PitchCurrent;
    float PitchVel;

    float TargetForwardSpeed;
    float TargetHorizontalSpeed;

    float SendTimer = 0f;
    bool PendingJump = false;

    public void DisableInput() => InputEnabled = false;
    public void EnableInput() => InputEnabled = true;

    public void Initalize(Magician Character)
    {
        BindCharacter(Character);
        ConfigureOwnershipAndCameras();
        ConfigureOwnerHud();
        SubscribeDbHandlers();

        WorldAudioManager.Initialize(this);
    }

    void BindCharacter(Magician Character)
    {
        Magician = Character;

        Identity = Character.Identity;
        Id = Character.Id;
        Name = Character.Name;
        MatchId = Character.GameId;

        transform.position = Character.Position;
        TargetPosition = Character.Position;

        transform.rotation = Quaternion.Euler(0, Character.Rotation.Yaw, 0);
        TargetRotation = Character.Rotation;

        Yaw = Character.Rotation.Yaw;
        Pitch = Character.Rotation.Pitch;
        PitchCurrent = Character.Rotation.Pitch;

        KinematicInformation = Character.KinematicInformation;
    }

    void ConfigureOwnershipAndCameras()
    {
        IsOwner = Identity.Equals(GameManager.LocalIdentity);

        if (ThirdPersonCam != null)
            ThirdPersonCam.gameObject.SetActive(IsOwner);

        if (IsOwner)
        {
            MainCamera = FindFirstObjectByType<CinemachineBrain>()?.OutputCamera
                ?? throw new Exception("No Main Camera Brain OutputCamera");
        }
    }

    void ConfigureOwnerHud()
    {
        if (!IsOwner)
            return;

        if (Crosshair != null)
        {
            Crosshair.gameObject.SetActive(true);
            Crosshair.worldCamera = MainCamera;
        }

        if (MagicianHud != null)
        {
            MagicianHud.gameObject.SetActive(true);
            MagicianHud.HudCanvas.worldCamera = MainCamera;

            foreach (PlayerEffect Effect in GameManager.Conn.Db.PlayerEffects.TargetId.Filter(Id))
            {
                if (Effect.EffectType is EffectType.Invincible)
                    MagicianHud.HandleEffectAsTarget(Effect);
                    MagicianOutwardHud.SetInvincible();
            }
        }
    }

    void SubscribeDbHandlers()
    {
        GameManager.Conn.Db.Magician.OnUpdate += HandleMagicianUpdate;
        GameManager.Conn.Db.Magician.OnUpdate += HandleHudUpdate;

        GameManager.Conn.Db.PlayerEffects.OnInsert += HandleMagicianEffectInsert;
        GameManager.Conn.Db.PlayerEffects.OnUpdate += HandleMagicianEffectUpdate;
        GameManager.Conn.Db.PlayerEffects.OnDelete += HandleMagicianEffectDelete;

        GameManager.Conn.Db.UnavailableRequestEvent.OnInsert += HandleUnavailableRequest;
        GameManager.Conn.Db.UnavailableRequestInterruptEvent.OnInsert += HandleUnavailableInterruptRequest;
    }

    public void HandleUnavailableRequest(EventContext ctx, UnavailableRequestEvent request)
    {
        if (IsOwner && request.Identity == Identity) {
            Animator.SetTrigger(UnavailableActionHash);
            MatchManager.Instance.AudioManager.PlayUnavailableActionSound(IsOwner);
        }
    }

    public void HandleUnavailableInterruptRequest(EventContext ctx, UnavailableRequestInterruptEvent request)
    {
        if (IsOwner && request.Identity == Identity)
            Animator.SetTrigger("UnavailableActionInterrupt");  
    }

    void UnsubscribeDbHandlers()
    {
        if (GameManager.Conn?.Db == null)
            return;

        GameManager.Conn.Db.Magician.OnUpdate -= HandleMagicianUpdate;
        GameManager.Conn.Db.Magician.OnUpdate -= HandleHudUpdate;

        GameManager.Conn.Db.PlayerEffects.OnInsert -= HandleMagicianEffectInsert;
        GameManager.Conn.Db.PlayerEffects.OnUpdate -= HandleMagicianEffectUpdate;
        GameManager.Conn.Db.PlayerEffects.OnDelete -= HandleMagicianEffectDelete;

        GameManager.Conn.Db.UnavailableRequestEvent.OnInsert -= HandleUnavailableRequest;
        GameManager.Conn.Db.UnavailableRequestInterruptEvent.OnInsert -= HandleUnavailableInterruptRequest;
    }

    void Update()
    {
        WorldAudioManager.UpdateKinematicStateSound();

        if (!IsOwner || !InputEnabled)
            return;

        AccumulateMouseInput();

        if (Input.GetKeyDown(KeyCode.Space)) PendingJump = true;

        SendTimer += Time.deltaTime;
        if (SendTimer >= 1f / SendRateHz)
        {
            SendTimer = 0f;
            HandleMovementRequests();
        }

        HandleCameraReliantActions();
        HandleNormalActions();
    }

    void AccumulateMouseInput()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        Yaw = Mathf.Repeat(Yaw + MouseX * SensX, 360f);
        Pitch = Mathf.Clamp(Pitch - MouseY * SensY, MinPitch, MaxPitch);
    }

    void HandleMovementRequests()
    {
        MovementRequest CurrentRequest = BuildMovementRequest();
        GameManager.Conn.Reducers.HandleMovementRequestMagician(CurrentRequest);
    }

    void HandleCameraReliantActions()
    {
        bool Hypnosised = IsPermissionOccupied(Magician, "Hypnosised");
        bool AttackHeld = Input.GetMouseButton(0);
        bool DustHeld = Input.GetKeyDown(KeyCode.E);

        if (!AttackHeld && !DustHeld && Hypnosised is false)
            return;

        Vector2 Reticle = new(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray AimRay = MainCamera.ScreenPointToRay(Reticle);
        Vector3 ClientReticleDirection = AimRay.direction.normalized;

        Vector3 CameraWorldPosition = MainCamera.transform.position;
        Vector3 CharacterWorldPosition = transform.position;
        Vector3 CameraWorldDelta = CameraWorldPosition - CharacterWorldPosition;

        Quaternion MagicianRotation = Quaternion.Euler(Pitch, Yaw, 0f);
        Vector3 LocalDir = Quaternion.Inverse(MagicianRotation) * ClientReticleDirection;

        float CameraYawOffset = Mathf.Atan2(LocalDir.x, LocalDir.z);
        float CameraPitchOffset = Mathf.Asin(Mathf.Clamp(LocalDir.y, -1f, 1f));

        float CameraYawRadians = (Yaw * Mathf.Deg2Rad) + CameraYawOffset;
        float CameraPitchRadians = (Pitch * Mathf.Deg2Rad) + CameraPitchOffset;

        Quaternion CameraRotation =
            Quaternion.Euler(0f, CameraYawRadians * Mathf.Rad2Deg, 0f) *
            Quaternion.Euler(CameraPitchRadians * Mathf.Rad2Deg, 0f, 0f);

        Vector3 CameraOffsetLocal = Quaternion.Inverse(CameraRotation) * CameraWorldDelta;

        DbVector3 CameraOffsetLocalDb = new(CameraOffsetLocal.x, CameraOffsetLocal.y, CameraOffsetLocal.z);

        if (AttackHeld)
        {
            GameManager.Conn.Reducers.HandleActionChangeRequestMagician(
                new ActionRequestMagician(
                    State: MagicianState.Attack,
                    new AttackInformation(
                        CameraPositionOffset: CameraOffsetLocalDb,
                        CameraYawOffset: CameraYawOffset,
                        CameraPitchOffset: CameraPitchOffset,
                        SpawnPointOffset: new(0f, 1.3f, 0.4f),
                        MaxDistance: 100f
                    ),
                    new ReloadInformation(),
                    new DustInformation(),
                    new CloakInformation(),
                    new HypnosisInformation()
                )
            );
        }

        if (DustHeld)
        {
            GameManager.Conn.Reducers.HandleActionChangeRequestMagician(
                new ActionRequestMagician(
                    State: MagicianState.Dust,
                    new AttackInformation(),
                    new ReloadInformation(),
                    new DustInformation(
                        CameraPositionOffset: CameraOffsetLocalDb,
                        CameraYawOffset: CameraYawOffset,
                        CameraPitchOffset: CameraPitchOffset,
                        SpawnPointOffset: new(0f, 1.3f, 0.4f),
                        MaxDistance: 2.5f,
                        ConeHalfAngleDegrees: 20f
                    ),
                    new CloakInformation(),
                    new HypnosisInformation()
                )
            );
        }

        if (Hypnosised is true)
        {
            GameManager.Conn.Reducers.Hypnotise(
                new HypnosisCameraInformation(
                    CameraPositionOffset: CameraOffsetLocalDb,
                    CameraYawOffset: CameraYawOffset,
                    CameraPitchOffset: CameraPitchOffset,
                    SpawnPointOffset: new DbVector3(0f, 1.65f, 0.15f),
                    MaxDistance: 12f
                )
            );
        }
    }

    void HandleNormalActions()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
            GameManager.Conn.Reducers.StartGameManual();

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Conn.Reducers.HandleActionChangeRequestMagician(
                new ActionRequestMagician(
                    State: MagicianState.Reload,
                    new AttackInformation(),
                    new ReloadInformation(),
                    new DustInformation(),
                    new CloakInformation(),
                    new HypnosisInformation()
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Conn.Reducers.HandleActionChangeRequestMagician(
                new ActionRequestMagician(
                    State: MagicianState.Cloak,
                    new AttackInformation(),
                    new ReloadInformation(),
                    new DustInformation(),
                    new CloakInformation(),
                    new HypnosisInformation()
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.Conn.Reducers.HandleActionChangeRequestMagician(
                new ActionRequestMagician(
                    State: MagicianState.Hypnosis,
                    new AttackInformation(),
                    new ReloadInformation(),
                    new DustInformation(),
                    new CloakInformation(),
                    new HypnosisInformation()
                )
            );
        }

        if (Input.GetMouseButtonDown(1)) {
            GameManager.Conn.Reducers.HandleStatelessActionRequestMagician(new StatelessActionRequestMagician(Action: MagicianStatelessAction.Tarot));
        }
    }

    public MovementRequest BuildMovementRequest()
    {
        MovementRequest CurrentRequest = new(
            MoveForward: false,
            MoveBackward: false,
            MoveLeft: false,
            MoveRight: false,
            Sprint: false,
            Jump: false,
            Crouch: false,
            Aim: new DbRotation2(0, 0)
        );

        if (Input.GetKey(KeyCode.W)) CurrentRequest.MoveForward = true;
        if (Input.GetKey(KeyCode.S)) CurrentRequest.MoveBackward = true;
        if (Input.GetKey(KeyCode.A)) CurrentRequest.MoveLeft = true;
        if (Input.GetKey(KeyCode.D)) CurrentRequest.MoveRight = true;

        if (Input.GetKey(KeyCode.LeftShift)) CurrentRequest.Sprint = true;
        if (Input.GetKey(KeyCode.LeftControl)) CurrentRequest.Crouch = true;

        if (PendingJump) { CurrentRequest.Jump = true; PendingJump = false; }

        CurrentRequest.Aim = new DbRotation2(Yaw, Pitch);

        return CurrentRequest;
    }

    public static bool IsPermissionOccupied(Magician Magician, string Key)
    {
        foreach (PermissionEntry Entry in Magician.Permissions)
        {
            if (Entry.Key == Key)
                return Entry.Subscribers.Count != 0;
        }

        throw new Exception($"Permission Entry With Key {Key} Not Found");
    }

    void LateUpdate()
    {
        float k = 1f - Mathf.Exp(-12f * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, TargetPosition, k);

        float TargetYaw = TargetRotation.Yaw;
        Quaternion TargetRot = Quaternion.Euler(0f, TargetYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, 1f - Mathf.Exp(-12f * Time.deltaTime));

        if (ThirdPersonCamPivot != null)
        {
            PitchCurrent = Mathf.SmoothDampAngle(PitchCurrent, TargetRotation.Pitch, ref PitchVel, PitchSmooth);
            ThirdPersonCamPivot.transform.localRotation = Quaternion.Euler(PitchCurrent, 0f, 0f);
        }

        if (Animator != null)
        {
            Animator.SetFloat("ForwardSpeed", TargetForwardSpeed, SpeedBlendTime, Time.deltaTime);
            Animator.SetFloat("HorizontalSpeed", TargetHorizontalSpeed, SpeedBlendTime, Time.deltaTime);
        }
    }

    public void HandleMagicianUpdate(EventContext context, Magician oldChar, Magician newChar)
    {
        if (Id != newChar.Id)
            return;

        Magician = newChar;
        TargetPosition = newChar.Position;
        TargetRotation = newChar.Rotation;

        bool Jump = oldChar.KinematicInformation.Jump is false && newChar.KinematicInformation.Jump is true;

        bool Attack = oldChar.State is not MagicianState.Attack && newChar.State is MagicianState.Attack;
        bool AttackDone = oldChar.State is MagicianState.Attack && newChar.State is not MagicianState.Attack;

        bool Reload = oldChar.State is not MagicianState.Reload && newChar.State is MagicianState.Reload;
        bool ReloadDone = oldChar.State is MagicianState.Reload && newChar.State is not MagicianState.Reload;

        bool Dust = oldChar.State is not MagicianState.Dust && newChar.State is MagicianState.Dust;
        bool DustDone = oldChar.State is MagicianState.Dust && newChar.State is not MagicianState.Dust;

        bool Cloak = oldChar.State is not MagicianState.Cloak && newChar.State is MagicianState.Cloak;
        bool CloakDone = oldChar.State is MagicianState.Cloak && newChar.State is not MagicianState.Cloak;

        bool Hypnosis = oldChar.State is not MagicianState.Hypnosis && newChar.State is MagicianState.Hypnosis;
        bool HypnosisDone = oldChar.State is MagicianState.Hypnosis && newChar.State is not MagicianState.Hypnosis;

        bool Stunned = oldChar.State is not MagicianState.Stunned && newChar.State is MagicianState.Stunned;
        bool StunnedDone = oldChar.State is MagicianState.Stunned && newChar.State is not MagicianState.Stunned;

        bool Grounded = newChar.KinematicInformation.Grounded;
        bool Crouching = newChar.KinematicInformation.Crouched;
        bool Falling = newChar.KinematicInformation.Falling;
        bool Landed = oldChar.KinematicInformation.Grounded is false && newChar.KinematicInformation.Grounded is true;

        if (Animator != null)
        {
            if (Jump) { 
                Animator.SetTrigger("Jump");
                WorldAudioManager.PlayJumpSound();
            }

            if (Landed) {
                WorldAudioManager.PlayLandSound();
            }

            if (Attack) {
                Animator.SetTrigger("Attack");
                MatchManager.Instance.AudioManager.PlayAttackSound(IsOwner);
            }

            if (AttackDone) Animator.SetTrigger("AttackDone");

            if (Reload) Animator.SetTrigger("Reload");
            if (ReloadDone) Animator.SetTrigger("ReloadDone");

            if (Dust) {
                Animator.SetTrigger("Dust");
                MatchManager.Instance.AudioManager.PlayDustCastSound(IsOwner);
            }

            if (DustDone) Animator.SetTrigger("DustDone");

            if (Cloak) {
                Animator.SetTrigger("Cloak");
                MatchManager.Instance.AudioManager.PlayCloakSound(IsOwner);
            }

            if (CloakDone) Animator.SetTrigger("CloakDone");

            if (Hypnosis) { 
                Animator.SetTrigger("Hypnosis");
                MatchManager.Instance.AudioManager.PlayHypnosisSound(IsOwner);
            }

            if (HypnosisDone) Animator.SetTrigger("HypnosisDone");

            if (Stunned) {
                Animator.SetTrigger("Stunned");
                MatchManager.Instance.AudioManager.PlayStunnedSound(IsOwner);
            }

            if (StunnedDone) Animator.SetTrigger("StunnedDone");

            Animator.SetBool("Crouching", Crouching);
            Animator.SetBool("Falling", Falling);
            Animator.SetBool("Grounded", Grounded);
        }

        DbVector3 AnimationVelocity = newChar.RequestedVelocity;
        Vector3 vWorld = new(AnimationVelocity.X, 0f, AnimationVelocity.Z);
        Quaternion YawOnly = Quaternion.Euler(0f, newChar.Rotation.Yaw, 0f);
        Vector3 vLocal = Quaternion.Inverse(YawOnly) * vWorld;

        TargetForwardSpeed = vLocal.z;
        TargetHorizontalSpeed = vLocal.x;
    }

    public void HandleHudUpdate(EventContext context, Magician oldMagician, Magician newMagician)
    {
        if (!IsOwner || Id != newMagician.Id)
            return;

        if (oldMagician.CombatInformation.Health != newMagician.CombatInformation.Health)
            MagicianHud.UpdateHealth((int)newMagician.CombatInformation.Health);

        if (oldMagician.Bullets.Count != newMagician.Bullets.Count)
            MagicianHud.UpdateAmmo(newMagician.Bullets.Count);

        HandleTarotHud(oldMagician, newMagician);
        HandleDustHud(oldMagician, newMagician);
        HandleCloakHud(oldMagician, newMagician);
        HandleHypnosisHud(oldMagician, newMagician);
    }

    void HandleTarotHud(Magician oldMagician, Magician newMagician)
    {
        if (oldMagician.StatelessTimers[0].State is StatelessTimerState.Useable && newMagician.StatelessTimers[0].State is StatelessTimerState.InCooldown) {
            MagicianHud.StartTarotCooldown();
            MatchManager.Instance.AudioManager.PlayTarotCastSound(IsOwner); 
        }

        if (newMagician.StatelessTimers[0].State is StatelessTimerState.InCooldown)
            MagicianHud.UpdateTarot((int)Math.Ceiling(newMagician.StatelessTimers[0].CooldownTime - newMagician.StatelessTimers[0].CurrentTime));

        if (oldMagician.StatelessTimers[0].State is StatelessTimerState.InCooldown && newMagician.StatelessTimers[0].State is StatelessTimerState.Useable)
            MagicianHud.EndTarotCooldown();
    }

    void HandleDustHud(Magician oldMagician, Magician newMagician)
    {
        if (oldMagician.Timers[2].State is TimerState.Usable && newMagician.Timers[2].State is not TimerState.Usable)
            MagicianHud.StartDustCooldown();

        if (newMagician.Timers[2].State is not TimerState.Usable)
            MagicianHud.UpdateDust((int)Math.Ceiling(newMagician.Timers[2].CooldownTime - newMagician.Timers[2].CurrentTime));

        if (oldMagician.Timers[2].State is not TimerState.Usable && newMagician.Timers[2].State is TimerState.Usable)
            MagicianHud.EndDustCooldown();
    }

    void HandleCloakHud(Magician oldMagician, Magician newMagician)
    {
        if (oldMagician.Timers[3].State is TimerState.Usable && newMagician.Timers[3].State is not TimerState.Usable)
            MagicianHud.StartCloakCooldown();

        if (newMagician.Timers[3].State is not TimerState.Usable)
            MagicianHud.UpdateCloak((int)Math.Ceiling(newMagician.Timers[3].CooldownTime - newMagician.Timers[3].CurrentTime));

        if (oldMagician.Timers[3].State is not TimerState.Usable && newMagician.Timers[3].State is TimerState.Usable)
            MagicianHud.EndCloakCooldown();
    }

    void HandleHypnosisHud(Magician oldMagician, Magician newMagician)
    {
        if (oldMagician.Timers[4].State is TimerState.Usable && newMagician.Timers[4].State is not TimerState.Usable)
            MagicianHud.StartHypnosisCooldown();

        if (newMagician.Timers[4].State is not TimerState.Usable)
            MagicianHud.UpdateHypnosis((int)Math.Ceiling(newMagician.Timers[4].CooldownTime - newMagician.Timers[4].CurrentTime));

        if (oldMagician.Timers[4].State is not TimerState.Usable && newMagician.Timers[4].State is TimerState.Usable)
            MagicianHud.EndHypnosisCooldown();
    }

    public void HandleMagicianEffectInsert(EventContext context, PlayerEffect insertedEffect)
    {
        if (IsOwner) {
            if (insertedEffect.TargetId == Id)
                MagicianHud.HandleEffectAsTarget(insertedEffect);

            else if (insertedEffect.SenderId == Id)
                MagicianHud.HandleEffectAsSender(insertedEffect);
        }

        if (insertedEffect.TargetId == Id) {
            if (insertedEffect.EffectType == EffectType.Dust)
                MagicianOutwardHud.SetOutwardDustCloudActive(true);

            else if (insertedEffect.EffectType == EffectType.Cloak)
                MagicianOutwardHud.SetInvisible(Local: IsOwner);
            
            else if (insertedEffect.EffectType == EffectType.Invincible)
                MagicianOutwardHud.SetInvincible();
        }
    }

    public void HandleMagicianEffectDelete(EventContext context, PlayerEffect deletedEffect)
    {
        if (IsOwner) {
            if (deletedEffect.TargetId == Id)
                MagicianHud.HandleEffectRemoveAsTarget(deletedEffect);
        }

        if (deletedEffect.TargetId == Id) {
            if (deletedEffect.EffectType == EffectType.Dust)
                MagicianOutwardHud.SetOutwardDustCloudActive(false);

            else if (deletedEffect.EffectType == EffectType.Cloak)
                MagicianOutwardHud.ResetInvisible();
            
            else if (deletedEffect.EffectType == EffectType.Invincible)
                MagicianOutwardHud.ResetInvincible();
        }
    }

    public void HandleMagicianEffectUpdate(EventContext context, PlayerEffect oldEffect, PlayerEffect newEffect)
    {
        if (!IsOwner)
            return;

        if (newEffect.EffectType == EffectType.Damage || newEffect.EffectType == EffectType.Stunned)
            return;

        ApplicationInformation oldInfo = oldEffect.EffectInfo.ApplicationInformation;
        ApplicationInformation newInfo = newEffect.EffectInfo.ApplicationInformation;

        if (newEffect.EffectType == EffectType.Dust && newInfo.EndTime - newInfo.CurrentTime < 1.0 && oldInfo.EndTime - oldInfo.CurrentTime > 1.0)
            MagicianHud.TryHudIconFlash(newEffect);

        else if (newInfo.EndTime - newInfo.CurrentTime < 2.0 && oldInfo.EndTime - oldInfo.CurrentTime > 2.0)
            MagicianHud.TryHudIconFlash(newEffect);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Magician"))
        {
            MagicianController Player = other.gameObject.GetComponent<MagicianController>();
            if (Player.Id != Id)
            {
                CollisionEntry Entry = new(EntryType: CollisionEntryType.Magician, Id: Player.Id);
                GameManager.Conn.Reducers.AddCollisionEntryMagician(Entry, Identity);
            }
        }
        else if (other.gameObject.CompareTag("Map"))
        {
            MapPiece Map = other.gameObject.GetComponent<MapPiece>();
            CollisionEntry Entry = new(EntryType: CollisionEntryType.Map, Id: Map.Id);
            GameManager.Conn.Reducers.AddCollisionEntryMagician(Entry, Identity);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Magician"))
        {
            MagicianController Player = other.gameObject.GetComponent<MagicianController>();
            if (Player.Id != Id)
            {
                CollisionEntry Entry = new(EntryType: CollisionEntryType.Magician, Id: Player.Id);
                GameManager.Conn.Reducers.RemoveCollisionEntryMagician(Entry, Identity);
            }
        }
        else if (other.gameObject.CompareTag("Map"))
        {
            MapPiece Map = other.gameObject.GetComponent<MapPiece>();
            CollisionEntry Entry = new(EntryType: CollisionEntryType.Map, Id: Map.Id);
            GameManager.Conn.Reducers.RemoveCollisionEntryMagician(Entry, Identity);
        }
    }

    public void LeaveToLobby()
    {
        if (IsOwner)
            MatchManager.Instance.CleanupMatchManager();
    }

    public void Delete()
    {
        WorldAudioManager?.StopKinematicStateSound();
        UnsubscribeDbHandlers();
        Destroy(gameObject);
    }
}