
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using TMPro;
using System.Threading.Tasks;

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
    [SerializeField] Transform cameraPosition;
    [SerializeField] PlayerVisuals playerVisuals;

    public List<Item> heldByLocal = new List<Item>();

    [SerializeField] AudioSource footStepsAudioSource;
    [SerializeField] Material screenSpaceMaterial;
    public int safetyLevel;
    bool isInSafety;
    int tick;
    float ssWhite;
    float ssEdge;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        tick = Random.Range(0, 9999);
        PlayerMovementTutorial movement = gameObject.GetComponent<PlayerMovementTutorial>();
        if (!IsOwner)
        {
            movement.enabled = false;

        }
        else
        {

            screenSpaceMaterial.SetFloat("_White", 5.0f);
            screenSpaceMaterial.SetFloat("_Edge1", 5.0f);
            PlayerCam.Instance.orientation = orientation;
            MoveCamera.Instance.cameraPosition = cameraPosition;
            playerVisuals.ModelHideFromLocal();
            SessionManager.Instance.playerPrefabDone = true;
            GameManager.Instance.localPlayer = this;

        }

        if (IsServer)
        {
            GameManager.Instance.PlayerJoined(this);
        }


    }

    // Update is called once per frame
    void Update()
    {
        Footsteps();

        if (IsOwner)
        {
            if(safetyLevel <= 0)
            {
                ssWhite = Mathf.Lerp(ssWhite, 0.0f, Time.deltaTime * 3);
                ssEdge = Mathf.Lerp(ssEdge, 0.5f, Time.deltaTime * 3);
                screenSpaceMaterial.SetFloat("_White", ssWhite);
                screenSpaceMaterial.SetFloat("_Edge1", ssEdge);
            }
            else
            {
                ssWhite = Mathf.Lerp(ssWhite, 5.0f, Time.deltaTime * 1);
                ssEdge = Mathf.Lerp(ssEdge, 5.0f, Time.deltaTime * 1);
                screenSpaceMaterial.SetFloat("_White", ssWhite);
                screenSpaceMaterial.SetFloat("_Edge1", ssEdge);
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
}
