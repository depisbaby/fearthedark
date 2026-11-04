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
    public List<AiAgent> monsterLibrary;

    Crawler crawler;

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

    public void SpawnMonsters(int count)
    {
        if(!IsServer) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = MapManager.Instance.rooms[1].monsterSpawn.transform.position;

            if(crawler == null)
            {
                Crawler _crawler = Instantiate(monsterLibrary[0].gameObject).GetComponent<Crawler>();
                _crawler.nav.Warp(spawnPos);
                crawler = _crawler;

                NetworkObject no = _crawler.gameObject.GetComponent<NetworkObject>();
                no.Spawn();
            }
            else
            {

            }
        }
    }

}
