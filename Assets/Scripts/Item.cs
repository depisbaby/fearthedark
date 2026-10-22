using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour
{
    public string itemName;
    [HideInInspector]public bool isPickedUp;
    Transform followedTransform;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ChildOnNetworkSpawn();
    }

    public void OnDestroy()
    {
        if (GameManager.Instance.localPlayer.heldByLocal.Contains(this))
        {
            GameManager.Instance.localPlayer.LocalDropItem(this);
        }
        ChildOnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        ChildUpdate();

        if (!IsOwner) return;

        if (followedTransform != null)
        {
            if ((followedTransform.position - transform.position).magnitude > 2.0f)
            {
                GameManager.Instance.localPlayer.LocalDropItem(this);
            }

            Vector3 forceVector = (followedTransform.position - transform.position) * Time.deltaTime * 10000;
            
            rb.AddForce (forceVector);

            Quaternion delta = followedTransform.rotation * Quaternion.Inverse(rb.rotation);

            delta.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180f)
                angle -= 360f;

            Vector3 torque = axis * angle * 20 * Time.deltaTime;

            rb.AddTorque(torque);
        }
    }

    private void FixedUpdate()
    {
        ChildFixedUpdate();
    }

    public void SetLocalHold(Transform hold)
    {
        if(hold == null)
        {
            if (!IsOwner) return;
            followedTransform = null;
            rb.useGravity = true;
            rb.linearDamping = 0.0f;
            rb.angularDamping = 0.05f;
            ItemManager.Instance.PlayerDroppedItemServerRpc(NetworkObjectId);
        }
        else{
            if (!IsOwner) return;
            followedTransform = hold;
            rb.useGravity = false;
            rb.linearDamping = 10.0f;
            rb.angularDamping = 30.0f;
        }
    }
    protected virtual void ChildOnNetworkSpawn()
    {
        //child override implementation...
    }

    protected virtual void ChildOnDestroy()
    {
        //child override implementation...
    }

    protected virtual void ChildFixedUpdate()
    {
        //child override implementation...
    }
    protected virtual void ChildUpdate()
    {
        //child override implementation...
    }

    public virtual void Operate()
    {
        //child override implementation...
    }
}
