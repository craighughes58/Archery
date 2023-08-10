using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //You can access the game controller by typing GameController.Instance.VARIABLE YOU WANT 
    public static GameController Instance { get; private set;}//you can get it from other scripts but not set it 

    #region Delegate events
    //subscribable delegate event to notify relevant parties that timer is changing; currently used by UI -BMH
    public delegate void TimerUpdate(float newTime);
    public static event TimerUpdate updateTimer;
    #endregion

    #region private variables
    //How many points the player has acrewed
    private int _Score = 0;
    //The list of all spawners in the map
    private List<SpawnerBehaviour> _Spawners = new List<SpawnerBehaviour>();
    //
    private Vector3 _LastGroundedPos;

    #endregion

    #region Serialized Fields
    [Tooltip("How much time a player has in a round")]
    [SerializeField] private float timer;

    [Tooltip("The menu that appears when the player dies")]
    [SerializeField] private Canvas LossCanvas;

    [Tooltip("The music that plays in the background")]
    [SerializeField] private AudioSource ThemeMusic;
    #endregion

    /// <summary>
    /// This makes the method a singleton
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
        //LossCanvas.enabled = false;
        //LossCanvas.enabled = false;
        StartCoroutine(TickDownTimer());
    }

    #region Scoring Information
    /// <summary>
    /// this method updates the player's score
    /// </summary>
    /// <param name="amount">How many points the player gained</param>
    public void AddScore(int amount)
    {
        _Score += amount;
    }

    #endregion

    #region Timer 

    /// <summary>
    /// This coroutine is a timer that ticks down and then updates a member of the HUD that displays how much time is left
    /// </summary>
    /// <returns></returns>
    public IEnumerator TickDownTimer()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            updateTimer(timer); //calls event for use by other scripts
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
    /// This script adds newly made spawners to a list of all spawners in the level
    /// </summary>
    /// <param name="sb">The spawner behaviour being passed in</param>
    public void AddSpawner(SpawnerBehaviour sb)
    {
        _Spawners.Add(sb);
    }
    /// <summary>
    /// This script adds newly made spawners to a list of all spawners in the level and tracks what scene it's a part of
    /// </summary>
    /// <param name="sb">The spawner behaviour being passed in</param>
    /// <param name="scene">This determines if it's part of a scene where certain enemies need to be made</param>
    public void AddSpawner(SpawnerBehaviour sb, int scene)
    {
        
    }

    /// <summary>
    /// This method triggers all spawners in the level to spawn enemies
    /// </summary>
    public void TriggerBasicSpawners()
    {
        foreach(SpawnerBehaviour x in _Spawners)
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

    /// <summary>
    /// This method will be called whenever the player is grounded
    /// it will set their last grounded position to where they are at in the map
    /// </summary>
    /// <param name="LastPos"></param>
    public void SetLastPlayerPosition(Vector3 LastPos)
    {
        _LastGroundedPos = LastPos;
    }

    /// <summary>
    /// if the trigger attached to the game controller is tripped
    /// if it's a random object it's destroyed
    /// if it's a player return them to their last grounded spot in the map
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //if the object falling is the player return it to it's last grounded area
        if (other.gameObject.tag.Equals("Player"))
        {
            
            other.transform.position = _LastGroundedPos;
            print(other.transform.position);
        }
        //otherwise destroy it to clean up
        else
        {
            Destroy(other.gameObject);
        }
    }
    #endregion

    #region Loss Menu
    /// <summary>
    /// When the player reaches the loss condition this method shows the screen
    /// </summary>
    public void ShowLossScreen()
    {
        //reactivate mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //pause music
        ThemeMusic.Pause();
        //show canvas
        LossCanvas.enabled = true;
    }
    #endregion
}

