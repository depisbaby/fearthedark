using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCam : MonoBehaviour
{
    public static PlayerCam Instance;
    private void Awake()
    {
        Instance = this;
    }

    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    [SerializeField]MoveCamera moveCamera;
    [SerializeField] private LayerMask layerMask;

    public Transform rightHoldingPoint;
    public Transform leftHoldingPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (orientation == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y")  * sensY;

        //Recoil is added in Gun.cs!!!!!!!!!!!!!!!!!!

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (GameManager.Instance.localPlayer != null)
        {
            GameManager.Instance.localPlayer.lookingDirection.Value = transform.forward;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, layerMask))// pick up
            {
                if(hit.transform.parent != null)
                {
                    
                    I_Interactable interactable = hit.transform.parent.gameObject.GetComponent<I_Interactable>();
                    if (interactable != null) {
                        //Debug.Log("vittu");
                        interactable.Interact();
                        return;
                    }
                }

                if (GameManager.Instance.localPlayer.heldByLocal.Count >= 2) return;

                Item item = hit.transform.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    Debug.Log(item.itemName);
                    NetworkObject no = item.gameObject.GetComponent<NetworkObject>();
                    ItemManager.Instance.PlayerPicksUpItemServerRpc(NetworkManager.Singleton.LocalClientId, no.NetworkObjectId);
                }
            }
            else //drop
            {
                GameManager.Instance.localPlayer.LocalDropItem(null);
                //Debug.Log("Did not Hit");
            }


        }



    }


    
}
