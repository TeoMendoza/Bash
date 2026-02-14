using UnityEngine;
using System;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using System.Linq;
using TMPro;

#nullable enable
public class MatchManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject InGameUI;
    [SerializeField] GameObject InLobbyUI;
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] CursorLocker CursorLocker;

    [SerializeField] Canvas RespawnCanvas;
    [SerializeField] TextMeshProUGUI RespawnTime;

    [SerializeField] Canvas InGameMenuCanvas;
    bool InGameMenuActive = false;

    [SerializeField] Canvas GameInfoCanvas;
    [SerializeField] TextMeshProUGUI GameTime;
    [SerializeField] List<TextMeshProUGUI> ScoreboardSlots;

    [Header("Match State")]
    public Timestamp RespawnAtTimestamp;
    bool HasRespawnAt;

    public Timestamp GameEndTimestamp;
    public bool Started = false;

    public static MatchManager Instance { get; private set; }
    public bool Initalized = false;
    public uint? GameId = null;

    public Player Player;
    public DbConnection Conn;

    [Header("Prefabs")]
    public MagicianController MagicianPrefab;
    public List<MapPiece> MapPrefabs;

    public Dictionary<ulong, MagicianController> MagicianPrefabs = new();
    MagicianController? LocalMagician = null;

    public Dictionary<uint, MapPiece> MapPieces = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        HandleMenuToggle();
        UpdateRespawnTimerUi();
        UpdateGameTimerUi();
    }

    public void InitializeMatchManager(Player LocalPlayer)
    {
        if (Initalized)
            return;

        Conn = GameManager.Conn;
        Player = LocalPlayer;
        PlayerName.text = LocalPlayer.Name;

        RegisterDbHandlers();

        Initalized = true;
    }

    public void JoinGame()
    {
        if (Initalized && GameId is null)
            Conn.Reducers.TryJoinGame();        
    }

    public void LeaveGame()
    {
        if (Initalized && GameId is not null)
        {
            Conn.Reducers.TryLeaveGame();
            CleanupMatchManager();
        }
    }

    void RegisterDbHandlers()
    {
        Conn.Db.Magician.OnInsert += AddNewCharacter;
        Conn.Db.Magician.OnDelete += RemoveCharacter;

        Conn.Db.Game.OnUpdate += UpdateScoreboard;
        Conn.Db.Game.OnDelete += EndGame;

        Conn.Db.RespawnTimers.OnInsert += OnInsertRespawnTimer;
        Conn.Db.RespawnTimers.OnDelete += OnDeleteRespawnTimer;

        Conn.Db.GameTimers.OnInsert += OnInsertGameTimer;
    }

    void UnregisterDbHandlers()
    {
        Conn.Db.Magician.OnInsert -= AddNewCharacter;
        Conn.Db.Magician.OnDelete -= RemoveCharacter;

        Conn.Db.Game.OnUpdate -= UpdateScoreboard;
        Conn.Db.Game.OnDelete -= EndGame;

        Conn.Db.RespawnTimers.OnInsert -= OnInsertRespawnTimer;
        Conn.Db.RespawnTimers.OnDelete -= OnDeleteRespawnTimer;

        Conn.Db.GameTimers.OnInsert -= OnInsertGameTimer;
    }

    void HandleMenuToggle()
    {
        if (!Initalized || GameId is null || !Input.GetKeyDown(KeyCode.Escape))
            return;

        SetMenuActive(!InGameMenuActive);
    }

    void SetMenuActive(bool Active)
    {
        InGameMenuActive = Active;
        CursorLocker.SetUiOpen(Active);
        InGameMenuCanvas.gameObject.SetActive(Active);

        if (LocalMagician != null)
        {
            if (Active) LocalMagician.DisableInput();
            else LocalMagician.EnableInput();
        }
    }

    void UpdateRespawnTimerUi()
    {
        if (!HasRespawnAt)
            return;

        Timestamp NowTimestamp = (Timestamp)DateTimeOffset.UtcNow;
        long RemainingMicroseconds = RespawnAtTimestamp.TimeDurationSince(NowTimestamp).Microseconds;

        if (RemainingMicroseconds <= 0)
        {
            RespawnTime.text = "";
            return;
        }

        double RemainingSeconds = RemainingMicroseconds / 1_000_000.0;
        int RemainingSecondsCeil = (int)Math.Ceiling(RemainingSeconds);
        RespawnTime.text = RemainingSecondsCeil.ToString();
    }

    void UpdateGameTimerUi()
    {
        if (!Started)
            return;

        Timestamp NowTimestamp = (Timestamp)DateTimeOffset.UtcNow;
        long RemainingMicroseconds = GameEndTimestamp.TimeDurationSince(NowTimestamp).Microseconds;

        if (RemainingMicroseconds <= 0)
        {
            GameTime.text = "";
            return;
        }

        double RemainingSeconds = RemainingMicroseconds / 1_000_000.0;
        int RemainingSecondsCeil = (int)Math.Ceiling(RemainingSeconds);

        int RemainingMinutes = RemainingSecondsCeil / 60;
        int RemainingSecondsRemainder = RemainingSecondsCeil % 60;
        string SecondsBuffer = RemainingSecondsRemainder < 10 ? "0" : "";

        GameTime.text = $"{RemainingMinutes}:{SecondsBuffer}{RemainingSecondsRemainder}";
    }

    public void InitializeMatch()
    {
        SpawnExistingMagicians();
        SpawnExistingMapPieces();
        TryInitializeStartedState();
        SetInGameUiActive();
    }

    void SpawnExistingMagicians()
    {
        foreach (Magician Character in Conn.Db.Magician.Iter())
        {
            if (Character.GameId != GameId)
                continue;

            TrySpawnMagician(Character);
        }
    }

    void TrySpawnMagician(Magician Character)
    {
        ulong MagicianId = Character.Id;
        if (MagicianPrefabs.ContainsKey(MagicianId))
            return;

        var Prefab = Instantiate(MagicianPrefab);
        Prefab.Initalize(Character);
        MagicianPrefabs.Add(MagicianId, Prefab);

        if (Character.Identity == GameManager.LocalIdentity)
        {
            LocalMagician = Prefab;
            Prefab.EnableInput();
        }
    }

    void SpawnExistingMapPieces()
    {
        foreach (Map MapPiece in Conn.Db.Map.Iter())
        {
            uint MapPieceId = (uint)MapPiece.Id;
            if (MapPieces.ContainsKey(MapPieceId))
                continue;

            if (!TryFindMapPrefab(MapPiece.Name, out MapPiece MatchingPrefab))
                continue;

            MapPiece Prefab = Instantiate(MatchingPrefab);
            Prefab.Initialize(MapPiece);
            MapPieces.Add(MapPieceId, Prefab);
        }
    }

    bool TryFindMapPrefab(string PieceName, out MapPiece MatchingPrefab)
    {
        for (int Index = 0; Index < MapPrefabs.Count; Index++)
        {
            MapPiece Candidate = MapPrefabs[Index];
            if (Candidate != null && Candidate.PieceName == PieceName)
            {
                MatchingPrefab = Candidate;
                return true;
            }
        }

        MatchingPrefab = default!;
        return false;
    }

    void TryInitializeStartedState()
    {
        if (GameId is null)
            return;

        Game? Game = Conn.Db.Game.Id.Find((uint)GameId);
        if (Game is null)
            return;

        if (Game.InProgress == true)
            Started = true;

        if (!Started)
            return;

        UpdateScoreboardUi(Game.Scoreboard.Players);

        GameTimersTimer? GameTimer = Conn.Db.GameTimers.GameId.Find((uint)GameId);
        if (GameTimer is not null && GameTimer.GameId == GameId)
        {
            if (GameTimer.ScheduledAt is ScheduleAt.Time(var Timestamp))
                GameEndTimestamp = Timestamp;

            GameTime.text = "15:00";
        }

        GameInfoCanvas.gameObject.SetActive(true);
    }

    void SetInGameUiActive()
    {
        InGameUI.SetActive(true);
        InLobbyUI.SetActive(false);
        CursorLocker.SetUiOpen(false);
    }

    public void AddNewCharacter(EventContext context, Magician Character)
    {
        if (Character.Identity == GameManager.LocalIdentity && GameId == null)
        {
            GameId = Character.GameId;
            InitializeMatch();
            return;
        }

        if (GameId is null || Character.GameId != GameId)
            return;

        TrySpawnMagician(Character);
    }

    public void RemoveCharacter(EventContext context, Magician Character)
    {
        ulong MagicianId = Character.Id;

        if (MagicianPrefabs.TryGetValue(MagicianId, out var Prefab) && Prefab != null)
        {
            Prefab.Delete();
            MagicianPrefabs.Remove(MagicianId);
        }

        if (Character.Identity == GameManager.LocalIdentity && LocalMagician != null && LocalMagician.Id == MagicianId)
            LocalMagician = null;
    }

    public void OnInsertRespawnTimer(EventContext context, RespawnTimersTimer insertedTimer)
    {
        if (insertedTimer.Identity != GameManager.LocalIdentity)
            return;

        RespawnCanvas.gameObject.SetActive(true);
        RespawnTime.text = "5";

        if (insertedTimer.ScheduledAt is ScheduleAt.Time(var RespawnTimestamp))
        {
            RespawnAtTimestamp = RespawnTimestamp;
            HasRespawnAt = true;
        }
    }

    public void OnDeleteRespawnTimer(EventContext context, RespawnTimersTimer deletedTimer)
    {
        if (deletedTimer.Identity != GameManager.LocalIdentity)
            return;

        RespawnCanvas.gameObject.SetActive(false);
        HasRespawnAt = false;
    }

    public void OnInsertGameTimer(EventContext context, GameTimersTimer insertedTimer)
    {
        if (insertedTimer.GameId != GameId)
            return;

        Started = true;

        if (insertedTimer.ScheduledAt is ScheduleAt.Time(var Timestamp))
            GameEndTimestamp = Timestamp;

        GameTime.text = "15:00";
    }

    public void UpdateScoreboard(EventContext context, Game oldGame, Game newGame)
    {
        if (newGame.Id != GameId)
            return;

        bool JustStarted = oldGame.InProgress == false && newGame.InProgress == true;
        if (!(Started || JustStarted))
            return;

        Started = true;
        UpdateScoreboardUi(newGame.Scoreboard.Players);
        GameInfoCanvas.gameObject.SetActive(true);
    }

    void UpdateScoreboardUi(IReadOnlyList<ScoreboardPlayer> Players)
    {
        List<ScoreboardPlayer> Scores = Players.OrderByDescending(s => s.Score).ToList();

        for (int Index = 0; Index <= 2; Index++)
        {
            if (Index < Scores.Count)
            {
                string Name = Scores[Index].Name;
                ulong Score = Scores[Index].Score;
                ScoreboardSlots[Index].text = $"{Index + 1}. {Name} - {Score}";
            }
            else
            {
                ScoreboardSlots[Index].text = "";
            }
        }
    }

    public void EndGame(EventContext context, Game EndedGame)
    {
        if (EndedGame.Id == GameId)
            CleanupMatchManager();
    }

    public void CleanupMatchManager()
    {
        DeleteAllMagicians();
        DeleteAllMapPieces();

        ResetMatchState();
        SetLobbyUiActive();

        // Keep In Case Later On Bugs Happen Because Of Table Subscriptions - But These Make Rejoining Match Not Work Currently
        //UnregisterDbHandlers();
        //Initalized = false;
    }

    void DeleteAllMagicians()
    {
        var MagicianIds = MagicianPrefabs.Keys.ToList();
        for (int Index = 0; Index < MagicianIds.Count; Index++)
        {
            ulong MagicianId = MagicianIds[Index];
            if (MagicianPrefabs.TryGetValue(MagicianId, out var Prefab) && Prefab != null)
                Prefab.Delete();

            MagicianPrefabs.Remove(MagicianId);
        }
    }

    void DeleteAllMapPieces()
    {
        var MapPieceIds = MapPieces.Keys.ToList();
        for (int Index = 0; Index < MapPieceIds.Count; Index++)
        {
            uint MapPieceId = MapPieceIds[Index];
            if (MapPieces.TryGetValue(MapPieceId, out var Prefab) && Prefab != null)
                Prefab.Delete();

            MapPieces.Remove(MapPieceId);
        }
    }

    void ResetMatchState()
    {
        GameId = null;
        Started = false;
        HasRespawnAt = false;
        LocalMagician = null;

        GameInfoCanvas.gameObject.SetActive(false);
        RespawnCanvas.gameObject.SetActive(false);
        InGameMenuCanvas.gameObject.SetActive(false);

        GameTime.text = "";
        RespawnTime.text = "";

        SetMenuActive(false);
    }

    void SetLobbyUiActive()
    {
        InGameUI.SetActive(false);
        InLobbyUI.SetActive(true);
        CursorLocker.SetUiOpen(true);
    }
}
