using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Threading.Tasks;

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
    public NavMeshAgent nav;
    public AiState state;
    protected int tick;

    public Animator anim;

    protected int idleRemaining;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tick = Random.Range(0, 9999);
        Debug.Log("Position: " + nav.transform.position);
        Debug.Log("Agent enabled: " + nav.enabled);
        Debug.Log("On NavMesh: " + nav.isOnNavMesh);
        Debug.Log("Agent active: " + nav.isActiveAndEnabled);
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

            anim.SetBool("moving", moving.Value);
        }

    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        tick++;
        if (tick % 25 == 0) {

            CheckPath();
            AiBehaviour();
        }

    }


    protected void CheckPath()
    {
        if (nav.hasPath == false || nav.pathPending) return;

        if(nav.remainingDistance < 1.0f || nav.pathStatus == NavMeshPathStatus.PathPartial || nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            
            nav.ResetPath();
            state = AiState.None;
            Debug.Log("Path cleared");
        }
        
    }

    protected void SetDestination(Vector3 position)
    {
        state = AiState.Moving;
        nav.SetDestination(position);
    }
    protected virtual void AiBehaviour()
    {
        
        //Child implementation...
    }

    protected virtual void ChildStart()
    {

        //Child implementation...
    }

    protected void Roam()
    {
        Vector3 playerPos = GameManager.Instance.players[Random.Range(0, GameManager.Instance.players.Count - 1)].transform.position;
        Room room = MapManager.Instance.GetRandomNearbyRoom(MapManager.Instance.NearestRoomFromPosition(playerPos), 1);
        //Room room = MapManager.Instance.NearestRoomFromPosition(playerPos);
        if (room == null) return;
        if (room.monsterSpawn != null)
        {
            SetDestination(room.monsterSpawn.transform.position);
        }
        
    }

    protected async void Idle(int time)
    {
        Debug.Log("Idling");
        state = AiState.Idle;
        await Task.Delay(time);
        if (state != AiState.Idle) return;
        state = AiState.None;
        Debug.Log("Done idling");
    }

    protected void Interrupt()
    {
        nav.ResetPath();
        state = AiState.None;
    }


}
