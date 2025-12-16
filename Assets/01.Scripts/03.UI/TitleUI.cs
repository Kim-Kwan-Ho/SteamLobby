using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks.Data;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button _createLobbyButton;

    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private TMP_InputField _lobbyCodeInput;

    #region Events
    private void OnEnable()
    {
        SteamNetworkManager.Instance.Events.OnLobbyEntered += Event_LobbyEntered;
        SteamNetworkManager.Instance.Events.OnLobbyLeaved += Event_LobbyLeaved;
    }
    private void OnDisable()
    {
        SteamNetworkManager.Instance.Events.OnLobbyEntered -= Event_LobbyEntered;
        SteamNetworkManager.Instance.Events.OnLobbyLeaved -= Event_LobbyLeaved;
    }
    private void Event_LobbyEntered(Lobby? lobby)
    {
        RefreshTitle(false);
    }
    private void Event_LobbyLeaved()
    {
        RefreshTitle(true);
    }
    #endregion
    
    private void Start()
    {
        _createLobbyButton.onClick.AddListener(CreateLobby);
        _joinLobbyButton.onClick.AddListener(JoinLobby);
    }
    private void CreateLobby()
    {
        SteamNetworkManager.Instance.Events.CallCreateLobby();
    }
    private void JoinLobby()
    {
        SteamNetworkManager.Instance.Events.CallJoinLobby(_lobbyCodeInput.text);
    }


    private void RefreshTitle(bool isActive)
    {
        _createLobbyButton.gameObject.SetActive(isActive);
        _joinLobbyButton.gameObject.SetActive(isActive);
        _lobbyCodeInput.gameObject.SetActive(isActive);
        
    }
}
