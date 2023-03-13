using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Text;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    
    private void Awake()
    {
        instance= this;
    }

    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject menuButtons;

    public GameObject createRoomScreen;
    public TMP_InputField createRoomInputField;
    public GameObject createRoomButton;

    public TMP_InputField maxPlayersCounter;
    public TMP_Text maxPlayersWariningText;
    private string deafultPlayers = "4";

    public GameObject roomScreen;
    public TMP_Text roomNameText, playerNameLabel;
    private List<TMP_Text> allPlayersNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public RoomButton roomButton;
    private List<RoomButton> roomButtonList = new List<RoomButton>();

    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    private bool hasSetNickname;

    public string gameToPlay;
    public GameObject startGameButton;

    public GameObject roomTest;

    public GameObject bodyParts;

    void Start()
    {

        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to network...";

        PhotonNetwork.ConnectUsingSettings();

#if UNITY_EDITOR
        roomTest.SetActive(true);
#endif
    }

    void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joinning Lobby...";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if(!hasSetNickname)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);

            if (PlayerPrefs.HasKey("PlayerNickname"))
            {
                nameInput.text = PlayerPrefs.GetString("PlayerNickname");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
        maxPlayersCounter.text = deafultPlayers;
    }

    public void CreateRoom()
    {
        string createRoomInput = createRoomInputField.text;
        if (!string.IsNullOrEmpty(createRoomInput))
        {
            RoomOptions options= new RoomOptions();

            byte maxPlayersOut;
            byte.TryParse(maxPlayersCounter.text, out maxPlayersOut);

            if (maxPlayersOut < 20 && maxPlayersOut > 1)
            {
                options.MaxPlayers = maxPlayersOut;

                PhotonNetwork.CreateRoom(createRoomInput, options);

                CloseMenus();
                loadingText.text = "Creating Room...";
                loadingScreen.SetActive(true);

                createRoomInputField.text = "";
            }

            else
            {
                StartCoroutine(FlickerWarning(maxPlayersWariningText, maxPlayersWariningText.alpha, maxPlayersWariningText.color, 2));
            }

        }
    }

    private IEnumerator FlickerWarning(TMP_Text warningText,float baseAlpha, Color baseColor, int fastWarningSpeed, int reapet = 1)
    {
        float t = 0;
        float dec = (1 - baseAlpha) / 100;
        for (int i = 0; i < reapet; i++)
        {
            while (warningText.alpha < 1)
            {
                warningText.alpha += dec;
                warningText.color = Color.Lerp(baseColor, Color.red, t += dec);
                yield return new WaitForSecondsRealtime(t / (1000 * fastWarningSpeed));
            }

            while (warningText.alpha > baseAlpha)
            {
                warningText.alpha -= dec;
                t -= dec;
                warningText.color = Color.Lerp(Color.red, baseColor, 1 - t);
                yield return new WaitForSecondsRealtime(t / (1000 * fastWarningSpeed));
            }
        }
        

    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }

    private void ListAllPlayers()
    {
        foreach (TMP_Text playerName in allPlayersNames)
        {
            Destroy(playerName.gameObject);
        }
        allPlayersNames.Clear();

        // Photon Network Player class
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayersNames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayersNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create room: \n" + message;
        CloseMenus();
        errorScreen.SetActive(true);
        
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    public void CloseRoomBrowser() {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in roomButtonList)
        {
            Destroy(rb.gameObject);
        }
        roomButtonList.Clear();

        roomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newRoomButton = Instantiate(roomButton, roomButton.transform.parent);
                newRoomButton.SetButtonDetails(roomList[i]);
                newRoomButton.gameObject.SetActive(true);

                roomButtonList.Add(newRoomButton);
            }
        }
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);

        CloseMenus();
        loadingText.text = "Joining " + roomInfo.Name;
        loadingScreen.SetActive(true);
    }

    public void SetNickname()
    {
        string pickedName = nameInput.text;

        if (!string.IsNullOrEmpty(pickedName)){
            PhotonNetwork.NickName = pickedName;

            PlayerPrefs.SetString("PlayerNickname", pickedName);

            hasSetNickname= true;

            CloseMenus();
            menuButtons.SetActive(true);
            bodyParts.SetActive(true);

        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(gameToPlay);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void QuickJoin()
    {
        RoomOptions options= new RoomOptions();
        options.MaxPlayers = 8;

        PhotonNetwork.CreateRoom("Test");
        CloseMenus();
        loadingText.text = "Creating test room...";
        loadingScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
