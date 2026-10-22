using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Door : NetworkBehaviour, I_Interactable
{
    [SerializeField] private Collider frameCollider;
    [SerializeField] private Collider boardCollider;
    [SerializeField] private Transform closedRotation;
    [SerializeField] private Transform openRotation1;
    [SerializeField] private Transform openRotation2;
    [SerializeField] private bool locked;
    [SerializeField] private GameObject keyObject;
    bool IsOpen;
    Quaternion targetRotation;


    private void Awake()
    {
        targetRotation = closedRotation.rotation;
        IsOpen = false;
        Physics.IgnoreCollision(frameCollider, boardCollider, true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        boardCollider.transform.rotation = Quaternion.Slerp(boardCollider.transform.rotation, targetRotation, Time.deltaTime * 3f);
    }


    [ServerRpc(RequireOwnership = false)]
    void ToggleDoorServerRpc(Vector3 openerPosition)
    {
        // Debug.Log("heippa");
        if (!IsServer) return;
        if (locked)
        {
            return;
        }

        if (IsOpen)
        {
            IsOpen = false;
            targetRotation = closedRotation.rotation;
        }
        else
        {
            float distance1 = ((openRotation1.transform.position + openRotation1.transform.forward) - openerPosition).magnitude;
            float distance2 = ((openRotation2.transform.position + openRotation2.transform.forward) - openerPosition).magnitude;
            if (distance1 < distance2) {
                IsOpen = true;
                targetRotation = openRotation2.transform.rotation;
            }
            else
            {
                IsOpen = true;
                targetRotation = openRotation1.transform.rotation;
            }
        }
    }

    public void Interact()
    {
        ToggleDoorServerRpc(GameManager.Instance.localPlayer.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if(locked == false) return;

        if(other.transform.gameObject.tag == "Key")
        {
            locked = false;
            float distance1 = ((openRotation1.transform.position + openRotation1.transform.forward) - other.transform.position).magnitude;
            float distance2 = ((openRotation2.transform.position + openRotation2.transform.forward) - other.transform.position).magnitude;
            if (distance1 < distance2)
            {
                SetUnlockedClientRpc(true);
            }
            else
            {
                SetUnlockedClientRpc(false);
            }
            ItemManager.Instance.DespawnItem(other.transform.parent.gameObject.GetComponent<Item>());
            
        }
    }

    [ClientRpc]void SetUnlockedClientRpc(bool side)
    {
        keyObject.SetActive(true);
        if (side) {
            keyObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
