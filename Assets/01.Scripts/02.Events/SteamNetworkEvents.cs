using System;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamNetworkEvents : MonoBehaviour
{
    public Action OnCreateLobby;
    public void CallCreateLobby()
    {
        OnCreateLobby?.Invoke();
    }

    public Action OnLobbyCreateFailed;
    public void CallLobbyCreateFailed()
    {
        OnLobbyCreateFailed?.Invoke();
    }

    public Action<string> OnJoinLobby;
    public void CallJoinLobby(string lobbyCode)
    {
        OnJoinLobby?.Invoke(lobbyCode);
    }

    public Action OnLobbyJoinFailed;
    public void CallLobbyJoinFailed()
    {
        OnLobbyJoinFailed?.Invoke();
    }

    public Action OnLeaveLobby;
    public void CallLeaveLobby()
    {
        OnLeaveLobby?.Invoke();
    }
    public Action OnLobbyLeaved;
    public void CallLobbyLeaved()
    {
        OnLobbyLeaved?.Invoke();
    }

    public Action<Lobby?> OnLobbyEntered;
    public void CallLobbyEntered(Lobby? lobby)
    {
        OnLobbyEntered?.Invoke(lobby);
    }

    public Action<bool, Friend> OnMemberChanged;
    public void CallMemberChanged(bool hasLeft, Friend friend)
    {
        OnMemberChanged?.Invoke(hasLeft, friend);
    }

    public Action OnHostLeft;
    public void CallHostLeft()
    {
        OnHostLeft?.Invoke();
    }
}
