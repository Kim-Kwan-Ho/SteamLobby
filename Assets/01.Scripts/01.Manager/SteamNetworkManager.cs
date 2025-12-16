using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using UnityEngine;


[RequireComponent(typeof(SteamNetworkEvents))]
public class SteamNetworkManager : MonoBehaviour
{
    private static SteamNetworkManager _instance;
    public static SteamNetworkManager Instance { get { return _instance; } }

    [SerializeField] private SteamNetworkEvents _events;
    public SteamNetworkEvents Events { get { return _events; } }

    private Lobby? _currentLobby;
    private ulong _ownerId;
    private Dictionary<ulong, Friend> _connectedMemberDic;

    [Header("Lobby Settings")]
    [SerializeField] private bool _isPublic;
    [SerializeField] private int _maxLobbyMember;

    [Header("Options")]
    private bool _isCreatingLobby;
    private bool _isJoiningLobby;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            if (_events == null)
            {
                _events = GetComponent<SteamNetworkEvents>();
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _isCreatingLobby = false;
        _isJoiningLobby = false;
        _connectedMemberDic = new Dictionary<ulong, Friend>();
        SubScribeSteamNetworkEvents();
        SubscribeMatchMakingEvents();
    }

    private void OnDestroy()
    {
        UnSubScribeSteamNetworkEvents();
        UnSubscribeMatchMakingEvents();
    }

    #region SteamNetwork Events
    private void SubScribeSteamNetworkEvents()
    {
        _events.OnCreateLobby += Event_CreateLobby;
        _events.OnJoinLobby += Event_JoinLobby;
        _events.OnLeaveLobby += Event_LeaveLobby;
    }
    private void UnSubScribeSteamNetworkEvents()
    {
        _events.OnCreateLobby -= Event_CreateLobby;
        _events.OnJoinLobby -= Event_JoinLobby;
        _events.OnLeaveLobby -= Event_LeaveLobby;
    }

    private void Event_CreateLobby()
    {
        if (_isCreatingLobby)
            return;
        StartCoroutine(CoCreateLobby());
    }
    private IEnumerator CoCreateLobby()
    {
        _isCreatingLobby = true;
        var lobbyTask = SteamMatchmaking.CreateLobbyAsync(_maxLobbyMember);
        while (!lobbyTask.IsCompleted)
        {
            yield return null;
        }

        if (lobbyTask.Exception != null)
        {
            Debug.LogError($"Fail To Create Lobby: {lobbyTask.Exception.Message}");
        }
        else if (!lobbyTask.Result.HasValue)
        {
            Debug.LogError($"None Valuable Lobby");
        }
        _isCreatingLobby = false;
    }
    private void Event_JoinLobby(string lobbyCode)
    {
        if (_isJoiningLobby)
            return;
        StartCoroutine(CoJoinLobby(lobbyCode));
    }

    private IEnumerator CoJoinLobby(string lobbyCode)
    {
        _isJoiningLobby = true;
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.LogError("Null LobbyCode Input");
            _isJoiningLobby = false;
            _events.CallLobbyJoinFailed();
            yield break;
        }
        if (!ulong.TryParse(lobbyCode, out ulong id))
        {
            Debug.LogError("Unsupported Type LobbyCode Input");
            _isJoiningLobby = false;
            _events.CallLobbyJoinFailed();
            yield break;
        }
        Lobby lobby = new Lobby(id);
        var joinTask = lobby.Join();
        while (!joinTask.IsCompleted)
        {
            yield return null;
        }
        if (joinTask.Exception != null)
        {
            Debug.LogError($"Fail To Enter Lobby {joinTask.Exception.Message}");
            _events.CallLobbyJoinFailed();
        }
        _isJoiningLobby = false;
    }
    private void Event_LeaveLobby()
    {
        LeaveLobby();
    }

    #endregion
    #region MatchMaking Events

    private void SubscribeMatchMakingEvents()
    {
        SteamMatchmaking.OnLobbyCreated += Event_LobbyCreated;
        SteamMatchmaking.OnLobbyEntered += Event_LobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += Event_LobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += Event_LobbyMemberLeaved;
        SteamMatchmaking.OnLobbyMemberDisconnected += Event_LobbyMemberDisconnected;
    }
    private void UnSubscribeMatchMakingEvents()
    {
        SteamMatchmaking.OnLobbyCreated -= Event_LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= Event_LobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= Event_LobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= Event_LobbyMemberLeaved;
        SteamMatchmaking.OnLobbyMemberDisconnected -= Event_LobbyMemberDisconnected;
    }
    private void Event_LobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError("Fail To Create Lobby");
            _events.CallLobbyCreateFailed();
            return;
        }

        if (_isPublic)
        {
            lobby.SetPublic();
        }
        else
        {
            lobby.SetPrivate();
        }
        lobby.SetJoinable(true);
        lobby.SetGameServer(lobby.Owner.Id);
        _currentLobby = lobby;
    }
    private void Event_LobbyEntered(Lobby lobby)
    {
        if (IsValidLobby(lobby))
        {
            SetConnectedMemberDic(lobby);
            _currentLobby = lobby;
            _ownerId = lobby.Owner.Id;
            _events.CallLobbyEntered(lobby);
        }
        else
        {
            Debug.Log("Non Valid Lobby");
            _events.CallLobbyJoinFailed();
        }
    }
    private void Event_LobbyMemberJoined(Lobby lobby, Friend friend)
    {
        _connectedMemberDic[friend.Id] = friend;
        _events.CallMemberChanged(false, friend);
    }

    private void Event_LobbyMemberLeaved(Lobby lobby, Friend friend)
    {
        if (_ownerId == friend.Id)
        {
            LeaveLobby();
            _events.CallHostLeft();
        }
        else
        {
            _connectedMemberDic.Remove(friend.Id);
            _events.CallMemberChanged(true, friend);
        }
    }
    private void Event_LobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        if (_ownerId == friend.Id)
        {
            LeaveLobby();
            _events.CallHostLeft();
        }
        else
        {
            _connectedMemberDic.Remove(friend.Id);
            _events.CallMemberChanged(true, friend);
        }
    }
    #endregion


    private void LeaveLobby()
    {
        _connectedMemberDic.Clear();
        _currentLobby?.Leave();
        _currentLobby = null;
        _ownerId = 0;
        _events.CallLobbyLeaved();
    }
    private bool IsValidLobby(Lobby lobby)
    {
        if (lobby.Id == 0)
        {
            return false;
        }

        if (lobby.Owner.Id == 0)
        {
            return false;
        }

        if (lobby.MaxMembers == 0)
        {
            return false;
        }

        if (lobby.MemberCount >= lobby.MaxMembers)
        {
            return false;
        }

        return true;
    }
    private void SetConnectedMemberDic(Lobby lobby)
    {
        _connectedMemberDic.Clear();
        foreach (var member in lobby.Members)
        {
            _connectedMemberDic[member.Id] = member;
        }
    }
    public Friend[] GetConnectedMembers()
    {
        return _connectedMemberDic.Values.ToArray();
    }
    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }
}
