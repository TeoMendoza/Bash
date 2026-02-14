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
    [SerializeField] GameObject InGameUI;
    [SerializeField] GameObject InLobbyUI;
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] CursorLocker CursorLocker;
    [SerializeField] Canvas RespawnCanvas;
    [SerializeField] TextMeshProUGUI RespawnTime;

    [SerializeField] Canvas InGameMenuCanvas;
    bool InGameMenuActive = false;

    public Timestamp RespawnAtTimestamp;
    private bool HasRespawnAt;

    [SerializeField] Canvas GameInfoCanvas;
    [SerializeField] TextMeshProUGUI GameTime;
    [SerializeField] List<TextMeshProUGUI> ScoreboardSlots;
    public Timestamp GameEndTimestamp;
    public bool Started = false;

    public static MatchManager Instance { get; private set; }
    public bool Initalized = false;
    public uint? GameId = null;
    public Player Player;
    public DbConnection Conn;

    public Dictionary<ulong, MagicianController> MagiciansById = new();
    public MagicianController MagicianPrefab;
    private MagicianController? LocalMagician = null;

    public Dictionary<uint, MapPiece> MapPieces = new();
    public List<MapPiece> MapPrefabs;

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
        if (Initalized && GameId is not null && Input.GetKeyDown(KeyCode.Escape) && InGameMenuActive == false)
        {
            CursorLocker.SetUiOpen(true);
            InGameMenuCanvas.gameObject.SetActive(true);
            InGameMenuActive = true;

            if (LocalMagician != null)
                LocalMagician.DisableInput();
        }
        else if (Initalized && GameId is not null && Input.GetKeyDown(KeyCode.Escape) && InGameMenuActive == true)
        {
            CursorLocker.SetUiOpen(false);
            InGameMenuCanvas.gameObject.SetActive(false);
            InGameMenuActive = false;

            if (LocalMagician != null)
                LocalMagician.EnableInput();
        }

        if (HasRespawnAt)
        {
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

        if (Started)
        {
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
    }

    public void LeaveGame()
    {
        if (Initalized && GameId is not null)
        {
            Conn.Reducers.TryLeaveGame();
            CleanupMatchManager();
        }
    }

    public void JoinGame()
    {
        if (Initalized && GameId is null)
            Conn.Reducers.TryJoinGame();
    }

    public void InitializeMatchManager(Player LocalPlayer)
    {
        if (Initalized)
            return;

        Conn = GameManager.Conn;
        Player = LocalPlayer;
        PlayerName.text = LocalPlayer.Name;

        Conn.Db.Magician.OnInsert += AddNewCharacter;
        Conn.Db.Magician.OnDelete += RemoveCharacter;

        Conn.Db.Game.OnUpdate += UpdateScoreboard;
        Conn.Db.RespawnTimers.OnInsert += OnInsertRespawnTimer;
        Conn.Db.RespawnTimers.OnDelete += OnDeleteRespawnTimer;

        Conn.Db.GameTimers.OnInsert += OnInsertGameTimer;
        Conn.Db.Game.OnDelete += EndGame;

        Initalized = true;
    }

    public void InitializeMatch()
    {
        foreach (Magician Character in Conn.Db.Magician.Iter())
        {
            if (Character.GameId != GameId)
                continue;

            ulong MagicianId = Character.Id;
            if (MagiciansById.ContainsKey(MagicianId))
                continue;

            var Prefab = Instantiate(MagicianPrefab);
            Prefab.Initalize(Character);
            MagiciansById.Add(MagicianId, Prefab);

            if (Character.Identity == GameManager.LocalIdentity)
                LocalMagician = Prefab;
        }

        foreach (Map MapPiece in Conn.Db.Map.Iter())
        {
            if (MapPieces.ContainsKey((uint)MapPiece.Id))
                continue;

            MapPiece MatchingPrefab = default!;

            for (int PrefabIndex = 0; PrefabIndex < MapPrefabs.Count; PrefabIndex++)
            {
                MapPiece CandidatePrefab = MapPrefabs[PrefabIndex];
                if (CandidatePrefab != null && CandidatePrefab.PieceName == MapPiece.Name)
                {
                    MatchingPrefab = CandidatePrefab;
                    break;
                }
            }

            if (MatchingPrefab == null)
                continue;

            MapPiece Prefab = Instantiate(MatchingPrefab);
            Prefab.Initialize(MapPiece);
            MapPieces.Add((uint)MapPiece.Id, Prefab);
        }

        if (GameId is not null)
        {
            Game? Game = Conn.Db.Game.Id.Find((uint)GameId);
            if (Game is not null && Game.InProgress == true)
                Started = true;

            if (Started)
            {
                List<ScoreboardPlayer> Scores = Game!.Scoreboard.Players.OrderByDescending(s => s.Score).ToList();
                for (int Index = 0; Index <= 2; Index++)
                {
                    if (Index < Scores.Count)
                    {
                        string Name = Scores[Index].Name;
                        ulong Score = Scores[Index].Score;
                        ScoreboardSlots[Index].text = $"{Index + 1}. {Name} - {Score}";
                    }
                }

                GameTimersTimer? GameTimer = Conn.Db.GameTimers.GameId.Find((uint)GameId);
                if (GameTimer is not null)
                {
                    if (GameTimer.GameId == GameId)
                    {
                        if (GameTimer.ScheduledAt is ScheduleAt.Time(var Timestamp))
                            GameEndTimestamp = Timestamp;

                        GameTime.text = "15:00";
                    }
                }

                GameInfoCanvas.gameObject.SetActive(true);
            }
        }

        InGameUI.SetActive(true);
        InLobbyUI.SetActive(false);
        CursorLocker.SetUiOpen(false);
    }

    public void AddNewCharacter(EventContext context, Magician Character)
    {
        if (Character.Identity == GameManager.LocalIdentity)
        {
            GameId = Character.GameId;
            InitializeMatch();
            return;
        }

        if (GameId is null || Character.GameId != GameId)
            return;

        ulong MagicianId = Character.Id;
        if (MagiciansById.ContainsKey(MagicianId))
            return;

        var NewPrefab = Instantiate(MagicianPrefab);
        NewPrefab.Initalize(Character);
        MagiciansById.Add(MagicianId, NewPrefab);
    }

    public void RemoveCharacter(EventContext context, Magician Character)
    {
        ulong MagicianId = Character.Id;

        if (MagiciansById.TryGetValue(MagicianId, out var Prefab) && Prefab != null)
        {
            Prefab.Delete();
            MagiciansById.Remove(MagicianId);
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

    public void EndGame(EventContext context, Game EndedGame)
    {
        if (EndedGame.Id == GameId)
            CleanupMatchManager();
    }

    public void UpdateScoreboard(EventContext context, Game oldGame, Game newGame)
    {
        if (newGame.Id == GameId && (Started || oldGame.InProgress == false && newGame.InProgress == true))
        {
            Started = true;

            List<ScoreboardPlayer> Scores = newGame.Scoreboard.Players.OrderByDescending(s => s.Score).ToList();
            for (int Index = 0; Index <= 2; Index++)
            {
                if (Index < Scores.Count)
                {
                    string Name = Scores[Index].Name;
                    ulong Score = Scores[Index].Score;
                    ScoreboardSlots[Index].text = $"{Index + 1}. {Name} - {Score}";
                }
            }

            GameInfoCanvas.gameObject.SetActive(true);
        }
    }

    public void OnInsertGameTimer(EventContext context, GameTimersTimer insertedTimer)
    {
        if (insertedTimer.GameId == GameId)
        {
            Started = true;

            if (insertedTimer.ScheduledAt is ScheduleAt.Time(var Timestamp))
                GameEndTimestamp = Timestamp;

            GameTime.text = "15:00";
        }
    }

    public void CleanupMatchManager()
    {
        Conn.Db.Magician.OnInsert -= AddNewCharacter;
        Conn.Db.Magician.OnDelete -= RemoveCharacter;

        Conn.Db.Game.OnUpdate -= UpdateScoreboard;
        Conn.Db.RespawnTimers.OnInsert -= OnInsertRespawnTimer;
        Conn.Db.RespawnTimers.OnDelete -= OnDeleteRespawnTimer;

        Conn.Db.GameTimers.OnInsert -= OnInsertGameTimer;
        Conn.Db.Game.OnDelete -= EndGame;

        var MagicianIds = MagiciansById.Keys.ToList();
        for (int Index = 0; Index < MagicianIds.Count; Index++)
        {
            ulong MagicianId = MagicianIds[Index];
            if (MagiciansById.TryGetValue(MagicianId, out var Prefab) && Prefab != null)
                Prefab.Delete();

            MagiciansById.Remove(MagicianId);
        }

        var MapPieceIds = MapPieces.Keys.ToList();
        for (int Index = 0; Index < MapPieceIds.Count; Index++)
        {
            var MapPieceId = MapPieceIds[Index];
            if (MapPieces.TryGetValue(MapPieceId, out var Prefab) && Prefab != null)
                Prefab.Delete();

            MapPieces.Remove(MapPieceId);
        }

        GameId = null;
        Started = false;
        HasRespawnAt = false;
        LocalMagician = null;

        GameInfoCanvas.gameObject.SetActive(false);
        RespawnCanvas.gameObject.SetActive(false);
        InGameMenuCanvas.gameObject.SetActive(false);

        InGameUI.SetActive(false);
        InLobbyUI.SetActive(true);
        CursorLocker.SetUiOpen(true);

        Initalized = false;
    }
}
