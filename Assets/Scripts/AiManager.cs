using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class AiManager : NetworkBehaviour
{
    public static AiManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public List<Player> playerOutOfSafety;

    int tick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tick = Random.Range(0, 9999);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        tick++;
        if (!IsServer) return;
        if(tick % 50 == 0)
        {
            UpdatePlayersSafety();
        }
    }

    void UpdatePlayersSafety()
    {
        playerOutOfSafety.Clear();
        foreach(Player player in GameManager.Instance.players)
        {
            if(player.safetyLevel == 0 && !player.isDead.Value)
            {
                playerOutOfSafety.Add(player);
            }
        }
    }

}
