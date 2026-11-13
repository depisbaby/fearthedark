
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using TMPro;
using System.Threading.Tasks;
using System;

public class Player : NetworkBehaviour
{
    [Header("Network Variables")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> isWalking = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isCrouching = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> lookingDirection = new NetworkVariable<Vector3>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Local Variables")]
    [SerializeField] Transform orientation;
    public Transform cameraPosition;
    public PlayerVisuals playerVisuals;

    public List<Item> heldByLocal = new List<Item>();

    [SerializeField] AudioSource footStepsAudioSource;
    public int safetyLevel;
    bool isInSafety;
    int tick;

    bool isBlind;

    [SerializeField] private GameObject deathCamHolder;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        tick = UnityEngine.Random.Range(0, 9999);
        PlayerMovementTutorial movement = gameObject.GetComponent<PlayerMovementTutorial>();
        if (!IsOwner)
        {
            movement.enabled = false;

        }
        else
        {

            OneBitFilter.Instance.SetFilterNormal(true);
            PlayerCam.Instance.orientation = orientation;
            MoveCamera.Instance.cameraPosition = cameraPosition;
            playerVisuals.ModelHideFromLocal();
            SessionManager.Instance.playerPrefabDone = true;
            GameManager.Instance.localPlayer = this;
            GameManager.Instance.AddPlayersAlreadyConnected();

        }

        GameManager.Instance.PlayerJoined(this);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead.Value) return;

        Footsteps();

        if (IsOwner)
        {
            if(safetyLevel <= 0)
            {
                OneBitFilter.Instance.SetFilterHunted(false);
            }
            else
            {
                OneBitFilter.Instance.SetFilterNormal(false);
            }
        }
    }

    private void FixedUpdate()
    {
        tick++;
        if(tick % 50 == 0)
        {
            if (!isInSafety)
            {
                if(safetyLevel > 0)
                {
                    safetyLevel--;
                }
            }
            else
            {
                isInSafety = false;
                safetyLevel = 10;
            }
           
        }
    }


    private void OnDestroy()
    {
        if (IsServer)
        {
            GameManager.Instance.PlayerDisconnected(this);
        }
    }

    public void DropEverything()
    {
        foreach(Item item in heldByLocal)
        {
            item.SetLocalHold(null);
        }

        heldByLocal.Clear();
    }

    public void LocalDropItem(Item _item)
    {
        if (heldByLocal.Count == 0) return;

        Item item;
        if (_item!= null)
        {
            heldByLocal.Remove(_item);
            _item.SetLocalHold(null);
        }
        else
        {
            item = heldByLocal[0];
            heldByLocal.Remove(item);
            item.SetLocalHold(null);
        }

        if(heldByLocal.Count == 1)
        {
            heldByLocal[0].SetLocalHold(PlayerCam.Instance.rightHoldingPoint);
        }
    }


    public void LocalPickUp(Item item)
    {
        heldByLocal.Insert(0,item);

        heldByLocal[0].SetLocalHold(PlayerCam.Instance.rightHoldingPoint);
        
        if(heldByLocal.Count == 2)
        {
            heldByLocal[1].SetLocalHold(PlayerCam.Instance.leftHoldingPoint);
        }
    }

    void Footsteps()
    {
        if((isWalking.Value ==1 || isWalking.Value == 2) && !isCrouching.Value)
        {
            footStepsAudioSource.volume = 1f;
        }
        else
        {
            footStepsAudioSource.volume = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(other.gameObject.layer == 9)
        {
            isInSafety = true;
        }
    }

    [ClientRpc] public void TeleportClientRpc(Vector3 destination)
    {
        if (IsOwner)
        {
            Teleport(destination);
        }
    }
    async void Teleport(Vector3 destination)
    {
        Debug.Log("Teleporting player to " + destination + "...");

        while (true)
        {
            transform.position = destination;
            await Task.Delay(100);
            if((destination-transform.position).magnitude < 1.0f){
                break;
            }
        }


    }

    public void SetBlind(bool blind)
    {
        isBlind = blind;
    }

    async void LocalKill()
    {
        if (isDead.Value) return;
        isDead.Value = true;
        SetBlind(true);
        GameObject go = Instantiate(deathCamHolder);
        go.transform.position = cameraPosition.transform.position;
        go.transform.rotation = PlayerCam.Instance.gameObject.transform.rotation;
        WorldCamera.Instance.BindTo(go);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        Rigidbody rb2 = GetComponent<Rigidbody>();
        rb.linearVelocity = rb2.linearVelocity;
        DropEverything();

        await Task.Delay(2000);

        HUD.Instance.SetBlind(true);

        if (GameManager.Instance.players.Count > 1) { 

            foreach(Player player in GameManager.Instance.players)
            {
                if(!player.isDead.Value && player != this)
                {
                    
                    WorldCamera.Instance.StartSpectating(player);
                    HUD.Instance.SetBlind(false);
                    break;
                }
            }
        }
        else
        {
            GameManager.Instance.ExitToMainMenu("");

        }

    }

    [ClientRpc]public void KillClientRPC()
    {
        if (IsOwner)
        {
            LocalKill();
        }
    }
}
