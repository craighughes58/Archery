using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawnerBehaviour : SpawnerBehaviour
{

    #region serialized variables
    [Tooltip("The sphere where enemies can be spawned")]
    [SerializeField] private float spawnRadius;

    [Tooltip("How many enemies can be spawned at once")]
    [SerializeField] private int enemyMax;

    [Tooltip("How much time elapses before checking if the spawner can respawn enemies")]
    [SerializeField] private float timer;

    [Tooltip("If the spawner can respawn enemies")]
    [SerializeField] private bool canRespawn;
    #endregion


    #region private variables

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
        if(canRespawn)
        {
            StartCoroutine(CheckRespawn());
        }
    }

    #region Spawning
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckRespawn()
    {
        Collider[] objectsInRadius;
        bool hasEnemy = false;
        yield return new WaitForSeconds(timer);
        //collect all objects in range
        objectsInRadius = Physics.OverlapSphere(transform.position, spawnRadius);
        //check if there are enemies
        foreach(Collider inRange in objectsInRadius)
        {
            if(inRange.tag.Equals("Enemy"))
            {
                hasEnemy = true;
                break;
            }
        }
        if(hasEnemy)
        {
            //if the list has enemies then restart
            StartCoroutine(CheckRespawn());
        }
        else
        {
            //if the list has no enemies then respawn
            SpawnEnemy();
        }

    }
    
    /// <summary>
    /// 
    /// </summary>
    public override void SpawnEnemy()
    {
        Vector2 randomPosition;
        for(int i = 0; i < enemyMax; i++)
        {
            randomPosition = Random.insideUnitCircle * spawnRadius;
            Instantiate(Enemy, new Vector3(transform.position.x + randomPosition.x, transform.position.y ,transform.position.z + randomPosition.y), Quaternion.identity);
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    // Gizmos.DrawSphere(transform.position, spawnRadius);

}
