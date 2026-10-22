using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class AiAgent : NetworkBehaviour
{
    public NetworkVariable<bool> moving = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public enum AiState
    {
        None=0,
        Idle=1,
        Moving=2
    }
    protected NavMeshAgent nav;
    public AiState state;
    protected int tick;

    public Animator anim;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tick = Random.Range(0, 9999);
        
        ChildStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (state == AiState.Moving) {
                moving.Value = true;
            }
            else
            {
                moving.Value = false;
            }
        }

        anim.SetBool("moving", moving.Value);
    }

    private void FixedUpdate()
    {
        tick++;
        if (tick % 25 == 0) {
            CheckPath();
            AiBehaviour();
        }

    }


    protected void CheckPath()
    {
        //Debug.Log("2");
        if(nav.remainingDistance < 1.0f || nav.pathStatus == NavMeshPathStatus.PathPartial || nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            nav.ResetPath();
            state = AiState.None;
        }
        
    }

    protected void SetDestination(Vector3 position)
    {
        nav.SetDestination(position);
        state = AiState.Moving;
    }
    protected virtual void AiBehaviour()
    {
        
        //Child implementation...
    }

    protected virtual void ChildStart()
    {

        //Child implementation...
    }

}
