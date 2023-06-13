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
    public IEnumerator TickDownTimer()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        //Activate end game procedures
        EndGame();
        yield return null;
    }

    private void EndGame()
    {

    }
    #endregion

    #region SpawningEnemies

    #endregion
}
