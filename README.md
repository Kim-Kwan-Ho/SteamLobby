# SteamLobby

## Project Info
Unity + Facepunch.Steamworks base Steam Lobby System

## SteamMatchmaking Callback Reference

### Currently Implemented Callbacks

| Callback Event | Host | Client | Description |
|----------------|:----:|:------:|-------------|
| `OnLobbyCreated` | ✅ | ❌ | Called when lobby creation completes (Host only) |
| `OnLobbyEntered` | ✅ | ✅ | Called when entering a lobby (All members) |
| `OnLobbyMemberJoined` | ✅ | ✅ | Called when a new member joins (Existing members) |
| `OnLobbyMemberLeave` | ✅ | ✅ | Called when a member leaves (Remaining members) |
| `OnLobbyMemberDisconnected` | ✅ | ✅ | Called when a member disconnects (Remaining members) |

### Available SteamMatchmaking Callbacks (Not Yet Implemented)

| Callback Event | Host | Client | Description |
|----------------|:----:|:------:|-------------|
| `OnLobbyDataChanged` | ✅ | ✅ | Called when lobby metadata has changed |
| `OnLobbyGameCreated` | ✅ | ✅ | Called when a game server has been associated with the lobby |
| `OnLobbyMemberKicked` | ✅ | ✅ | Called when a lobby member was kicked |
| `OnLobbyMemberBanned` | ✅ | ✅ | Called when a lobby member was banned |
| `OnLobbyMemberDataChanged` | ✅ | ✅ | Called when member data has changed |
| `OnChatMessage` | ✅ | ✅ | Called when a chat message is received from a lobby member |
| `OnLobbyInvite` | ✅ | ✅ | Called when invited to a lobby |



### Require Asset
https://github.com/Unity-Technologies/multiplayer-community-contributions.git?path=/Transports/com.community.netcode.transport.facepunch