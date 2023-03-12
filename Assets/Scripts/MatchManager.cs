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
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat
    }

    public List<PlayerInfo> allplayers = new List<PlayerInfo>();
    private int index;

    private List<LeaderboardPlayer> lboardPlayers = new List<LeaderboardPlayer>();
    void Start()
    {
        if (!PhotonNetwork.IsConnected) SceneManager.LoadScene(0);
        else NewPlayerSend(PhotonNetwork.NickName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIController.instance.leaderboard.activeInHierarchy)
            {
                UIController.instance.leaderboard.SetActive(false);
            }
            else
            {
                ShowLeaderboard();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("Received Event" + theEvent);

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
        object[] package = new object[allplayers.Count];
        for (int i = 0; i < allplayers.Count; i++)
        {
            object[] piece = new object[4] { allplayers[i].name, allplayers[i].actor, allplayers[i].kills, allplayers[i].deaths };
            package[i] = piece;
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
        for (int i = 0; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo((string)piece[0], (int)piece[1], (int)piece[2], (int)piece[3]);
            allplayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
                index = i;

        }
    }
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