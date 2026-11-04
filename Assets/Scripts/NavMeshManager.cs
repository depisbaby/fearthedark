using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Instance;

    private void Awake()
    {
        Instance = this;
        navMesh = GetComponent<NavMeshSurface>();
    }

    public NavMeshSurface navMesh;


    public void Bake()
    {
        navMesh.BuildNavMesh();
    }
}
