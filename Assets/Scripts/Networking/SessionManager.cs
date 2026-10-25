using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.Utilities;
using Unity.Collections;
using AtomicConsole;
using Unity.Netcode.Transports.UTP;

public class SessionManager : MonoBehaviour
{

    #region Singleton
        public static SessionManager Instance { get; private set; }
        
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
            
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
    #endregion

    //Local
    public string localUsername;
    public bool offlineMode;
    public bool playerPrefabDone;
    public string joiningCode;

    public bool busy;

    public UnityTransport soloTransport;

    private void Start()
    {
        Physics.IgnoreLayerCollision(0, 7);
        Physics.IgnoreLayerCollision(0, 9);
    }
    [AtomicCommand(name:"join", description:"")]
    public async void AttemptJoining(string joinCode)
    {
        if (busy) return;
        busy = true;

        if(joinCode == "")
        {
            MainMenu.Instance.DisplayConnectionError("Please insert either a joining code or an IP address.");
            return;
        }
        
        while(RelayScript.Instance == null){
            await Task.Yield();
        }

        if(!await RelayScript.Instance.JoinRelay(joinCode))
        {
            MainMenu.Instance.DisplayConnectionError("Couldn't join to the server! (65)");
            busy = false;
            Debug.Log("Couldn't join to the server!");
            return;
        }

        while (NetworkManager.Singleton.IsListening == false)
        {
            await Task.Yield();
        }

        GameManager.Instance.StartGame();

        busy = false;
    }

    [AtomicCommand(name: "host", description: "")]
    public async void AttemptHosting()
    {
        if (busy) return;
        busy = true;

        while (RelayScript.Instance == null){
            await Task.Yield();
        }

        if (!await RelayScript.Instance.CreateRelay())
        {
            MainMenu.Instance.DisplayConnectionError("Couldn't start host! (91)");
            busy = false;
            Debug.Log("Couldn't start host!");
            //Console.Instance.ShowMessageInConsole(this.gameObject, "Failed to create a relay the session!");
            return;
        }

        while (NetworkManager.Singleton.IsListening == false)
        {
            await Task.Yield();
        }

        GameManager.Instance.StartGame();

    }

    public async void Solo()
    {
        if (busy) return;
        busy = true;

        NetworkManager.Singleton.NetworkConfig.NetworkTransport = soloTransport;

        if (!NetworkManager.Singleton.StartHost())
        {
            MainMenu.Instance.DisplayConnectionError("Error (116)");
            Debug.Log("Couldn't start host!");
            
        }

        GameManager.Instance.StartGame();
        busy = false;
    }

    public void ExitGame()
    {
        NetworkManager.Singleton.Shutdown();

        RelayScript.Instance.RelaySignOut();


    }


}
