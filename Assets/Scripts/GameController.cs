using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //You can access the game controller by typing GameController.Instance.VARIABLE YOU WANT 
    public static GameController Instance { get; private set;}//you can get it from other scripts but not set it 

    #region private variables
    //
    private int Score = 0;
    //
    private List<SpawnerBehaviour> Spawners = new List<SpawnerBehaviour>();

    #endregion

    #region Serialized Fields
    [Tooltip("How much time a player has in a round")]
    [SerializeField] private float timer;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TickDownTimer());
    }


    #region Scoring Information
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount)
    {
        Score += amount;
    }

    #endregion

    #region Timer 

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator TickDownTimer()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //Activate end game procedures
        EndGame();
        yield return null;
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndGame()
    {
        print("Game Over");
    }
    #endregion

    #region SpawningEnemies
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    public void AddSpawner(SpawnerBehaviour sb)
    {
        Spawners.Add(sb);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="scene"></param>
    public void AddSpawner(SpawnerBehaviour sb, int scene)
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void TriggerBasicSpawners()
    {
        foreach(SpawnerBehaviour x in Spawners)
        {
            x.SpawnEnemy();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    public void TriggerSpawnerScene(int scene)
    {

    }
    #endregion
}
