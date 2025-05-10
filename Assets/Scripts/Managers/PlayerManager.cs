using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static PlayerManager instance;



    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [SerializeField] private List<Player> playerList = new List<Player>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {

        if (respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<StartPoint>().transform;
        }

        if (player == null)
            player = FindFirstObjectByType<Player>();

    }

    private void Update()
    {
        Debug.Log(respawnPoint);
    }

    public void RespawnPlayer()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
            return;

        StartCoroutine(RespawnCoroutine());

    }


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Respawn Coroutine");
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
        OnPlayerRespawn?.Invoke();
        
    }

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    public List<Player> GetPlayerList() => playerList;

}
