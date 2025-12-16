using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks.Data;
using Steamworks;
public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyCode;
    [SerializeField] private Button _leaveButton;
    [SerializeField] private Transform _playerInfosTrs;
    [SerializeField] private GameObject _playerInfoPrefab;
    private Dictionary<ulong, PlayerInfoUI> _connectedPlayerDic;

    private void Awake()
    {
        _connectedPlayerDic = new Dictionary<ulong, PlayerInfoUI>();
    }

    #region Events
    private void OnEnable()
    {
        SteamNetworkManager.Instance.Events.OnLobbyEntered += Event_LobbyEntered;
        SteamNetworkManager.Instance.Events.OnLobbyLeaved += Event_LobbyLeaved;
        SteamNetworkManager.Instance.Events.OnMemberChanged += Event_MemberChanged;
    }
    private void OnDisable()
    {
        SteamNetworkManager.Instance.Events.OnLobbyEntered -= Event_LobbyEntered;
        SteamNetworkManager.Instance.Events.OnLobbyLeaved -= Event_LobbyLeaved;
        SteamNetworkManager.Instance.Events.OnMemberChanged -= Event_MemberChanged;
    }
    private void Event_LobbyEntered(Lobby? lobby)
    {
        RefreshLobby(true);
        SetLobby(lobby);
    }
    private void Event_LobbyLeaved()
    {
        RefreshLobby(false);
    }
    private void Event_MemberChanged(bool hasLeft, Friend friend)
    {
        if (hasLeft)
        {
            RemovePlayer(friend);
        }
        else
        {
            AddPlayer(friend);
        }
    }
    #endregion
    private void Start()
    {
        _leaveButton.onClick.AddListener(LeaveLobby);
    }
    private void LeaveLobby()
    {
        SteamNetworkManager.Instance.Events.CallLeaveLobby();
    }
    private void RefreshLobby(bool isActive)
    {
        _lobbyCode.gameObject.SetActive(isActive);
        _leaveButton.gameObject.SetActive(isActive);
        if (!isActive)
        {
            ClearPlayerInfos();
        }
    }
    private void SetLobby(Lobby? lobby)
    {
        _lobbyCode.text = lobby.Value.Id.ToString();
        var connectedMembers = SteamNetworkManager.Instance.GetConnectedMembers();
        foreach (var member in connectedMembers)
        {
            AddPlayer(member);
        }

    }
    private void AddPlayer(Friend friend)
    {
        if (!_connectedPlayerDic.ContainsKey(friend.Id))
        {
            PlayerInfoUI playerInfo = Instantiate(_playerInfoPrefab, _playerInfosTrs).GetComponent<PlayerInfoUI>();
            playerInfo.SetPlayerInfo(friend);
            _connectedPlayerDic[friend.Id] = playerInfo;
        }
    }
    private void RemovePlayer(Friend friend)
    {
        if (_connectedPlayerDic.ContainsKey(friend.Id))
        {
            PlayerInfoUI playerInfo = _connectedPlayerDic[friend.Id];
            _connectedPlayerDic.Remove(friend.Id);
            Destroy(playerInfo.gameObject);
        }
    }
    private void ClearPlayerInfos()
    {
        foreach (var player in _connectedPlayerDic)
        {
            Destroy(player.Value.gameObject);
        }
        _connectedPlayerDic.Clear();
    }

}
