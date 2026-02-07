using UnityEngine;
using System;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using System.Linq;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
#nullable enable
public class MatchManager : MonoBehaviour
{
    [SerializeField] Canvas RespawnCanvas;
    [SerializeField] TextMeshProUGUI RespawnTime;

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
    public DbConnection Conn;

    public Dictionary<Identity, MagicianController> Players = new();
    public MagicianController MagicianPrefab;
    public Dictionary<uint, MapPiece> MapPieces = new();
    public List<MapPiece> MapPrefabs;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Initalized && GameId is null && Input.GetKeyDown(KeyCode.P))
            Conn.Reducers.TryJoinGame();
        
        if (Initalized && GameId is not null && Input.GetKeyDown(KeyCode.P)) {
            Conn.Reducers.TryLeaveGame();
            CleanupMatchManager();
        }

        if (HasRespawnAt) {
            Timestamp NowTimestamp = (Timestamp)DateTimeOffset.UtcNow; 
            long RemainingMicroseconds = RespawnAtTimestamp.TimeDurationSince(NowTimestamp).Microseconds; 

            if (RemainingMicroseconds <= 0) { 
                RespawnTime.text = ""; return; 
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

    public void InitializeMatchManager()
    {
        Conn = GameManager.Conn;
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
            if (Character.GameId == GameId && Players.ContainsKey(Character.Identity) is false)
            {
                var prefab = Instantiate(MagicianPrefab);
                prefab.Initalize(Character);
                Players.Add(Character.Identity, prefab);
            }
        }

        foreach (Map MapPiece in Conn.Db.Map.Iter())
        {
            if (MapPieces.ContainsKey((uint)MapPiece.Id)) continue;

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

            if (MatchingPrefab == null) continue;

            MapPiece Prefab = Instantiate(MatchingPrefab);
            Prefab.Initialize(MapPiece);
            MapPieces.Add((uint)MapPiece.Id, Prefab);
        }

        if (GameId is not null) {
            Game? Game = Conn.Db.Game.Id.Find((uint)GameId);
            if (Game is not null && Game.InProgress == true) { Started = true; }

            if (Started) {

                List<ScoreboardPlayer> Scores = Game!.Scoreboard.Players.OrderByDescending(s => s.Score).ToList();
                for (int index = 0; index <= 2; index++) {
                    if (index < Scores.Count) {
                        string name = Scores[index].Name;
                        ulong score = Scores[index].Score;
                        ScoreboardSlots[index].text = $"{index + 1}. {name} - {score}";         
                    }
                }

                GameTimersTimer? GameTimer = Conn.Db.GameTimers.GameId.Find((uint)GameId);
                if (GameTimer is not null) {
                    if (GameTimer.GameId == GameId) {
                        if (GameTimer.ScheduledAt is ScheduleAt.Time(var Timestamp)) {
                            GameEndTimestamp = Timestamp;
                        }
                        GameTime.text = "15:00";
                    } 
                }
                
                GameInfoCanvas.gameObject.SetActive(true);
            }
        }
    }

    public void AddNewCharacter(EventContext context, Magician Character)
    {
        if (Character.Identity == GameManager.LocalIdentity)
        {
            GameId = Character.GameId;
            InitializeMatch();
        }

        if (GameId is not null && Character.GameId == GameId && Players.ContainsKey(Character.Identity) is false)
        {
            var prefab = Instantiate(MagicianPrefab);
            prefab.Initalize(Character);     
            Players.Add(Character.Identity, prefab);
        }
            
    }
    public void RemoveCharacter(EventContext context, Magician Character)
    {
        if (GameId is not null && Character.GameId == GameId)
        {
            Players.TryGetValue(Character.Identity, out var prefab);
            if (prefab != null)
            {
                prefab.Delete();
                Players.Remove(Character.Identity);
            }
        }
    }

    public void OnInsertRespawnTimer(EventContext context, RespawnTimersTimer insertedTimer)
    {
        if (insertedTimer.Identity != GameManager.LocalIdentity) return;

        RespawnCanvas.gameObject.SetActive(true);
        RespawnTime.text = "5";

        if (insertedTimer.ScheduledAt is ScheduleAt.Time(var RespawnTimestamp)) {
            RespawnAtTimestamp = RespawnTimestamp;
            HasRespawnAt = true;
        }      
    }

    public void OnDeleteRespawnTimer(EventContext context, RespawnTimersTimer deletedTimer)
    {
        if (deletedTimer.Identity != GameManager.LocalIdentity) return;
        RespawnCanvas.gameObject.SetActive(false);
        HasRespawnAt = false;
    }

    public void EndGame(EventContext context, Game EndedGame)
    {
        if (EndedGame.Id == GameId) {
            CleanupMatchManager();
        }
    }

    public void UpdateScoreboard(EventContext context, Game oldGame, Game newGame)
    {
        if (newGame.Id == GameId && (Started || oldGame.InProgress == false && newGame.InProgress == true)) {
            Started = true;
            List<ScoreboardPlayer> Scores = newGame.Scoreboard.Players.OrderByDescending(s => s.Score).ToList();
            for (int index = 0; index <= 2; index++) {
                if (index < Scores.Count) {
                    string name = Scores[index].Name;
                    ulong score = Scores[index].Score;
                    ScoreboardSlots[index].text = $"{index + 1}. {name} - {score}";         
                }
            }
        }

        GameInfoCanvas.gameObject.SetActive(true);
    }

    public void OnInsertGameTimer(EventContext context, GameTimersTimer insertedTimer)
    {
        if (insertedTimer.GameId == GameId) {
            Started = true;
            if (insertedTimer.ScheduledAt is ScheduleAt.Time(var Timestamp)) {
                GameEndTimestamp = Timestamp;
            }
            GameTime.text = "15:00";
        } 
    }

    public void CleanupMatchManager()
    {
        var PlayerIdentities = Players.Keys.ToList();
        for (int Index = 0; Index < PlayerIdentities.Count; Index++)
        {
            var Identity = PlayerIdentities[Index];
            if (Players.TryGetValue(Identity, out var Prefab) && Prefab != null)
            {
                Prefab.Delete();
            }
            Players.Remove(Identity);
        }

        
        var MapPieceIds = MapPieces.Keys.ToList();
        for (int Index = 0; Index < MapPieceIds.Count; Index++)
        {
            var MapPieceId = MapPieceIds[Index];
            if (MapPieces.TryGetValue(MapPieceId, out var Prefab) && Prefab != null) {
                Prefab.Delete();
            }
            MapPieces.Remove(MapPieceId);
        }

        GameId = null;
        Started = false;
        HasRespawnAt = false;
    
        GameInfoCanvas.gameObject.SetActive(false);
        RespawnCanvas.gameObject.SetActive(false);

        // Send Client To Lobby Screen Once Created
    }
}
