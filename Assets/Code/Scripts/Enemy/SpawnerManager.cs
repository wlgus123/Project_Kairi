using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public List<EnemySpawner> spawners = new List<EnemySpawner>();

    void Awake()
    {
        spawners.AddRange(GetComponentsInChildren<EnemySpawner>());
    }

    public void RespawnAll()
    {
        foreach (var spawner in spawners)
        {
            spawner.SendMessage("Spawn", SendMessageOptions.DontRequireReceiver);
        }
    }
}
