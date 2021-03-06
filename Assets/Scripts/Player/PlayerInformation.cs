﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInformation : NetworkBehaviour
{
    // TODO: Might want to change these to Enums?
    public static string ROLE_KILLER = "killer";
    public static string ROLE_VILLAGER = "villager";
    public static string STATUS_ALIVE = "alive";
    public static string STATUS_DEAD = "dead";
    public static string STATUS_GHOST = "ghost";

    [SyncVar] private string _playerRole = string.Empty; // killer or villager 
    [SyncVar] private string _status = "alive"; // alive, dead, or ghost


    // Future TODO: Inventory Tracking, health?, other information about the player.

    [Server]
    public void SetPlayerRole(string pPlayerRole)
    {
        _playerRole = pPlayerRole;
    }

    [Server]
    public void SetStatus(string pStatus)
    {
        _status = pStatus;
    }

    public string GetPlayerRole()
    {
        return _playerRole;
    }

    public string GetStatus()
    {
        return _status;
    }


}
