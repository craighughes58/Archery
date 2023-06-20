using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnerBehaviour : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.AddSpawner(GetComponent<SpawnerBehaviour>());
    }
    [Tooltip("The basic enemy that this prefab spawns")]
    [SerializeField] public GameObject Enemy;
    
    /// <summary>
    /// 
    /// </summary>
    public abstract void SpawnEnemy();

   
}
