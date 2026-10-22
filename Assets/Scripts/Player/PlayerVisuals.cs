using Unity.VisualScripting;
using UnityEngine;


public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerModelBase;
    [SerializeField] private SkinnedMeshRenderer playerModelMesh;
    [SerializeField] private Animator anim;
    [SerializeField] private Material shadowOnlyMaterial;
    [SerializeField] private 
    ushort tick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.speed = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        Animations();
    }

    public void ModelHideFromLocal()
    {
        SkinnedMeshRenderer renderer = playerModelMesh;

        Material[] materials = new Material[renderer.sharedMaterials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = shadowOnlyMaterial;
        }

        renderer.sharedMaterials = materials;
    }

    void Animations()
    {

        if(player.isWalking.Value == 1)
        {
            anim.SetBool("running", true);
            anim.SetBool("backwards", false);
        }
        else if (player.isWalking.Value == 2)
        {
            anim.SetBool("running", true);
            anim.SetBool("backwards", true);
        }
        else{
            anim.SetBool("running", false);
            anim.SetBool("backwards", false);
        }

        Vector3 lookingDir = new Vector3(player.lookingDirection.Value.x, 0, player.lookingDirection.Value.z);
        playerModelBase.transform.LookAt(playerModelBase.transform.position + lookingDir);
        anim.SetBool("dead", player.isDead.Value);
        anim.SetBool("crouching", player.isCrouching.Value);

    }
}
