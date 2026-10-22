using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

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
        
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DevModePrep();
    }

    async void DevModePrep()
    {
        
        if (GameManager.Instance.buildAutoHost)
        {
            if (!Application.isEditor)
            {
                SessionManager.Instance.AttemptHosting();

                while(RelayScript.Instance.currentJoinCode == "")
                {
                    await Task.Yield();
                }
                Debug.Log("GGGGGG");
                joiningCodeTmp.gameObject.SetActive(true);
                joiningCodeTmp.text = RelayScript.Instance.currentJoinCode;
                

                while (localPlayer == null)
                {
                    await Task.Yield();
                }
                await Task.Delay(1000);
                localPlayer.transform.position = new Vector3(0, 1, -2);

            }
        }
        if (GameManager.Instance.editorAutoHost)
        {
            if (Application.isEditor)
            {
                SessionManager.Instance.AttemptHosting();

                while (RelayScript.Instance.currentJoinCode == "")
                {
                    await Task.Yield();
                }
                Debug.Log("GGGGGG");
                joiningCodeTmp.gameObject.SetActive(true);
                joiningCodeTmp.text = RelayScript.Instance.currentJoinCode;


                while (localPlayer == null)
                {
                    await Task.Yield();
                }
                await Task.Delay(1000);
                localPlayer.transform.position = new Vector3(0, 1, -2);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        
    }

    public void PlayerJoined(Player player)
    {
        if (IsServer)
        {
            players.Add(player);
        }
    }

    public void PlayerDisconnected(Player player) {
        if (IsServer)
        {
            players.Remove(player);
        }
    }
}
