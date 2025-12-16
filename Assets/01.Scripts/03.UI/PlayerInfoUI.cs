using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;

    public void SetPlayerInfo(Friend friend)
    {
        _nameText.text = friend.Name;
    }
}
