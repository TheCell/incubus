using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement_Manager : MonoBehaviour
{
    [SerializeField] private int achievementID = -1;

    private void OnTriggerEnter(Collider other)
    {
		if (!SteamManager.Initialized)
		{
			return;
		}

        if (other.name == "CharacterController")
        {
            switch (achievementID)
            {
                case 1:
                    Set_FIRST_MANIPULATION();
                    break;
                case 2:
                    Set_RESPAWN();
                    break;
                case 3:
                    Set_FIRST_PART();
                    break;
                case 4:
                    Set_SECOND_PART();
                    break;
                case 5:
                    Set_THIRD_PART();
                    break;
                case 6:
                    Set_LAST_PART();
                    break;
                case 7:
                    Set_TWELVE_MINUTES();
                    break;
                case 8:
                    Set_TEN_MINUTES();
                    break;
                case 9:
                    Set_EIGHT_MINUTES();
                    break;
                case 10:
                    Set_PLAYTHROUGH();
                    break;
                case 11:
                    Set_HEART();
                    break;
                default:
                    Debug.LogError($"Case {achievementID} not in list");
                    break;
            }
        }
    }

    private static void Set_FIRST_MANIPULATION()
    {
        Steamworks.SteamUserStats.SetAchievement("FIRST_MANIPULATION");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_RESPAWN()
    {
        Steamworks.SteamUserStats.SetAchievement("RESPAWN");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_FIRST_PART()
    {
        Steamworks.SteamUserStats.SetAchievement("FIRST_PART");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_SECOND_PART()
    {
        Steamworks.SteamUserStats.SetAchievement("SECOND_PART");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_THIRD_PART()
    {
        Steamworks.SteamUserStats.SetAchievement("THIRD_PART");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_LAST_PART()
    {
        Steamworks.SteamUserStats.SetAchievement("LAST_PART");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_TWELVE_MINUTES()
    {
        Steamworks.SteamUserStats.SetAchievement("TWELVE_MINUTES");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_TEN_MINUTES()
    {
        Steamworks.SteamUserStats.SetAchievement("TEN_MINUTES");
        Steamworks.SteamUserStats.StoreStats();
    }

    public static void Set_EIGHT_MINUTES()
    {
        Steamworks.SteamUserStats.SetAchievement("EIGHT_MINUTES");
        Steamworks.SteamUserStats.StoreStats();
    }

    private static void Set_PLAYTHROUGH()
    {
        Steamworks.SteamUserStats.SetAchievement("PLAYTHROUGH");
        Steamworks.SteamUserStats.StoreStats();
    }

    private static void Set_HEART()
    {
        Steamworks.SteamUserStats.SetAchievement("HEART");
        Steamworks.SteamUserStats.StoreStats();
    }
}
