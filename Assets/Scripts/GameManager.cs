using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Player localPlayer;


    public List<Player> players;

    public TMPro.TMP_Text joiningCodeTmp;
    [Header("Dev settings")]
    public bool buildAutoHost;
    public bool editorAutoHost;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.IgnoreLayerCollision(0, 7);
        Physics.IgnoreLayerCollision(0, 9);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public async void StartGame() {

        MainMenu.Instance.Close();

        

        if (IsServer)
        {
            if(MapManager.Instance != null)
            {

                MapManager.Instance.GenerateMap();

                while (!MapManager.Instance.isMapGenerated)
                {
                    await Task.Yield();
                }

                await Task.Delay(100);

                AiManager.Instance.SpawnMonsters(1);

                foreach (Player player in players)
                {
                    player.TeleportClientRpc(MapManager.Instance.startingRoom.transform.position + new Vector3(UnityEngine.Random.Range(-1.0f,1.0f), 2, UnityEngine.Random.Range(-1.0f, 1.0f)));
                }
            }

        }

        await Task.Delay(2000);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        HUD.Instance.SetBlind(false);
    }

    public void ExitToMainMenu(string menuMessage)
    {
        if (IsHost)
        {
            HostKickAllClientRpc();
        }



        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

    }

    [ClientRpc]void HostKickAllClientRpc()
    {
        if (IsHost) return;
        ExitToMainMenu("Disconnected: Host left the game.");
    }

    public void PlayerJoined(Player player)
    {
        players.Add(player);

        if (IsServer)
        {
            if(MapManager.Instance.startingRoom != null)
            {
                player.TeleportClientRpc(MapManager.Instance.startingRoom.transform.position + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 2, UnityEngine.Random.Range(-1.0f, 1.0f)));
            }
        }

        
    }

    public void PlayerDisconnected(Player player) {
        
        players.Remove(player);
    }

    public void AddPlayersAlreadyConnected()
    {
        Player[] _players = FindObjectsByType<Player>(
            FindObjectsSortMode.None
            );

        foreach (Player item in _players)
        {
            if(item != localPlayer)
            {
                players.Add(item);
            }
        }
    }

}
