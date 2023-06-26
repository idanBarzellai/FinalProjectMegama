using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
        PassDataScriptableObject passData = Resources.Load<PassDataScriptableObject>("passDataScriptable");
        if (passData != null) matchLength = passData.length;
        
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat,
        NextMatch,
        MatchTimerSync,
        WaitTimerSync,
        ChoosePlayers
    }

    
    public List<PlayerInfo> allplayers = new List<PlayerInfo>();
    private int index;

    private List<LeaderboardPlayer> lboardPlayers = new List<LeaderboardPlayer>();

    public enum GameState : byte
    {
        Waiting,
        Playing,
        Ending,
        Testing
        
    }

    public int killsToWIn = int.MaxValue;
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;

    public bool perpetual;
    public float matchLength = 60;
    private float choosingTime = 10;
    private float currentMatchTime;
    private float sendTimer;


    void Start()
    {
        if (!PhotonNetwork.IsConnected) SceneManager.LoadScene(0);
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
            state = GameState.Waiting;
            StartBGMusic();

            SetupTimer();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            UIController.instance.timerText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && state != GameState.Ending)
        {
             ShowLeaderboard();
        }
        else if(Input.GetKeyUp(KeyCode.Tab) && state != GameState.Ending)
        {
            UIController.instance.leaderboard.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (state == GameState.Waiting)
            {
                currentMatchTime -= Time.deltaTime;
                if(currentMatchTime - matchLength <= 0)
                {
                    ChoosePlayersSend();
                    state = GameState.Playing;
                }

                UpdateTimerDisplay(true);
            }

            if (state == GameState.Playing)
            {
                currentMatchTime -= Time.deltaTime;
                if (currentMatchTime <= 0f)
                {
                    currentMatchTime = 0f;
                    state = GameState.Ending;
                    ListPlayersSend();
                    StateCheck();
                }

                UpdateTimerDisplay(false);

            }
            sendTimer -= Time.deltaTime;
            if (sendTimer <= 0)
            {
                sendTimer += 1f;

                MatchTimerSyncSend();
            }

        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;


            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayersReceive(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStatReceive(data);
                    break;
                case EventCodes.NextMatch:
                    NextMatchReceive();
                    break;
                case EventCodes.MatchTimerSync:
                    MatchTimerSyncReceive(data);
                    break;

                case EventCodes.ChoosePlayers:
                    ChoosePlayersReceive();
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

    }
    public void NewPlayerSend(string username)
    {
        object[] package = new object[4] { username, PhotonNetwork.LocalPlayer.ActorNumber, 0, 0 };

        PhotonNetwork.RaiseEvent((byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);

        allplayers.Add(player);

        ListPlayersSend();
    }
    public void ListPlayersSend()
    {
        object[] package = new object[allplayers.Count + 1];

        package[0] = state;
        for (int i = 0; i < allplayers.Count; i++)
        {
            object[] piece = new object[4] { allplayers[i].name, allplayers[i].actor, allplayers[i].kills, allplayers[i].deaths };
            package[i + 1] = piece;
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.ListPlayers,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    public void ListPlayersReceive(object[] dataReceived)
    {
        allplayers.Clear();

        state = (GameState)dataReceived[0];

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo((string)piece[0], (int)piece[1], (int)piece[2], (int)piece[3]);
            allplayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
                index = i - 1;

        }

        StateCheck();
    }
    // Stat to update 0 - death , 1 - kill
    public void UpdateStatSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateStat,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    public void UpdateStatReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for (int i = 0; i < allplayers.Count; i++)
        {
            if(allplayers[i].actor == actor)
            {
                switch (statType)
                {
                    case 0: // kills
                        allplayers[i].kills += amount; 
                        break;
                    case 1: // deaths
                        allplayers[i].deaths+= amount;
                        break;
                }

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                if (UIController.instance.leaderboard.activeInHierarchy)
                {
                    ShowLeaderboard();
                }
                break;
            }

            
        }

        ScoreCheck();

    }

    public void UpdateStatsDisplay()
    {
        if (allplayers.Count > index)
        {

        UIController.instance.killsText.text = "Kills : " + allplayers[index].kills;
        UIController.instance.deathsText.text = "Deaths : " + allplayers[index].deaths;
        }
        else
        {
            UIController.instance.killsText.text = "Kills : 0";
            UIController.instance.deathsText.text = "Deaths : 0";
        }

    }

    void ShowLeaderboard()
    {
        UIController.instance.leaderboard.SetActive(true);


        foreach (LeaderboardPlayer lp in lboardPlayers)
        {
            Destroy(lp.gameObject);
        }

        lboardPlayers.Clear();

        UIController.instance.leaderboardPlayerDisplay.gameObject.SetActive(false) ;

        foreach (PlayerInfo player in SortPlayers(allplayers))
        {
            LeaderboardPlayer newPlayerDisplay = Instantiate(UIController.instance.leaderboardPlayerDisplay, UIController.instance.leaderboardPlayerDisplay.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths);

            newPlayerDisplay.gameObject.SetActive(true);

            lboardPlayers.Add(newPlayerDisplay);
        }

    }

    private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {

        players.Sort(new GFG());

        return players;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)

    {

        int index = allplayers.FindIndex(x => x.name == otherPlayer.NickName);

        Debug.Log(index);

        if (index != -1)

            allplayers.RemoveAt(index);



        ListPlayersSend();

    }

    void ScoreCheck()
    {
        bool winnerFound = false;

        foreach (PlayerInfo player in allplayers)
        {
            if(player.kills >= killsToWIn && killsToWIn > 0)
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound)
        {
            if(PhotonNetwork.IsMasterClient && state != GameState.Ending)
            {
                state = GameState.Ending;
                ListPlayersSend();
            }
        }
    }

    void StateCheck()
    {
        if(state == GameState.Ending)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        state = GameState.Ending;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.endScreen.SetActive(true);
        ShowLeaderboard();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndCo());
    }

    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);


        if (!perpetual)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (!Launcher.instance.changeMapBetweenRounds)
                {
                    NextMatchSend();
                }
                else
                {
                    int newLevel = Random.Range(0, Launcher.instance.allMaps.Length);

                    if(Launcher.instance.allMaps[newLevel] == SceneManager.GetActiveScene().name)
                    {
                        NextMatchSend();
                    }
                    else
                    {
                        PhotonNetwork.LoadLevel(Launcher.instance.allMaps[newLevel]);
                    }
                }
            }
        }
    }

    public void NextMatchSend()
    {

        PhotonNetwork.RaiseEvent((byte)EventCodes.NextMatch,
           null,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    public void NextMatchReceive()
    {
        state = GameState.Waiting;

        UIController.instance.deathScreen.SetActive(false);
        UIController.instance.endScreen.SetActive(false);
        UIController.instance.leaderboard.SetActive(false);
        UIController.instance.playerChoosingScreen.SetActive(true);
        UIController.instance.TurnOnOffAllBuyButtons(PowerupsManager.PowerUpsPowers.Null, false);


        foreach (PlayerInfo player in allplayers)
        {
            player.kills = 0;
            player.deaths = 0;
        }

        UpdateStatsDisplay();

        SetupTimer();
    }
    public void ChoosePlayersSend()
    {
        PhotonNetwork.RaiseEvent((byte)EventCodes.ChoosePlayers,
          null,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    public void ChoosePlayersReceive()
    {
        if (UIController.instance.playerChoosingScreen.activeInHierarchy)
        {
            PlayerSpawner.instance.SpawnPlayer();
            UIController.instance.playerChoosingScreen.SetActive(false);
        }
    }

    public void MatchTimerSyncSend()
    {
        object[] package = new object[] { (int)currentMatchTime , state};
        PhotonNetwork.RaiseEvent((byte)EventCodes.MatchTimerSync,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    public void MatchTimerSyncReceive(object[] dataReceived)
    {

        currentMatchTime = (int)dataReceived[0];
        state = (GameState)dataReceived[1];

        UpdateTimerDisplay(state == GameState.Waiting);

        UIController.instance.timerText.gameObject.SetActive(true);
    }


    public void SetupTimer()
    {
        currentMatchTime = matchLength + choosingTime;
        UpdateTimerDisplay(true);

    }


    public void UpdateTimerDisplay(bool isChoosingPlayerScreen)
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(isChoosingPlayerScreen ? currentMatchTime - matchLength : currentMatchTime);

        UIController.instance.timerText.text = timeToDisplay.Minutes.ToString("00") + ":" + timeToDisplay.Seconds.ToString("00");

        
    }

    private void StartBGMusic()
    {
        SoundManager.instacne.Play("BG");
    }

    public static GameState GetState() {if(instance == null) return GameState.Testing; return instance.state;}
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;

    public PlayerInfo(string _name , int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
}
class GFG : IComparer<PlayerInfo>
{
    public int Compare(PlayerInfo x, PlayerInfo y)
    {
        if (x.kills == 0 || y.kills == 0)
        {
            return 0;
        }

        // CompareTo() method

        return (y.kills).CompareTo((x.kills));

    }
}