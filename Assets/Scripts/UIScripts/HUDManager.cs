using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("HUD Element Targeting")]
    [Tooltip("A reference to the slider that will fill / drain to display health info")]
    [SerializeField]
    private Slider healthBar = null;
    [Tooltip("A reference to the text box displaying current & max arrowCount")]
    [SerializeField]
    private Text arrowCountDisplay = null;
    [Tooltip("A reference to the text box displaying game timer info")]
    [SerializeField]
    private Text timerDisplay = null;
    [Tooltip("A reference to the charge display box")]
    [SerializeField]
    private Slider chargeBar = null;
    #endregion

    #region non-serialized private fields
    private PlayerController player = null; //reference to player for pulling data at need
    private GameController controller = null; //reference to GameController for pulling data at need
    private int currentHealthVal = 0; //UNUSED ATM; need to use it for healthBarChange smoothing
    private int maxHealth = 0; //used to bound healthbar; must change if maxHealth is expected to change
    private int currentAmmoVal = 0; //used to store current helath
    private int maxAmmo = 0; //stores max arrow quiver size
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        //wrangle a reference to the player for data pulling purposes
        player = GameObject.Find("Player").GetComponent<PlayerController>();


        //wrangle a reference to the GameController to subscribe to Timer data;
        controller = GameObject.Find("GameController").GetComponent<GameController>();

        //set health conditions in HUD
        PlayerController.updateHealth += healthBarChange;
        maxHealth = player.getMaxHealth();
        healthBar.maxValue = maxHealth;
        healthBarChange(player.getCurrHealth());

        //set ammo conditions in HUD
        PlayerController.updateAmmo += ammoCounterChange;
        maxAmmo = player.getMaxAmmo();
        currentAmmoVal = player.getCurrAmmo();
        ammoDisplayUpdate(currentAmmoVal, maxAmmo);

        //set charge counter conditions in HUD
        PlayerController.updateCharge += chargeDisplayUpdate;
        chargeDisplayUpdate(0);


        //set timer conditions in HUD; don't need to preset as time will get autocalled in the first frame
        GameController.updateTimer += timerDisplayChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //function to handle HUD changes to healthbar value
    void healthBarChange(int newHealth)
    {
        //next step will be using Mathf.Lerp to make the flow up / down "smooth" as opposed to instantaneus - BH
        if (newHealth >= maxHealth)
        {
            healthBar.value = maxHealth;
        }
        else if (newHealth < maxHealth && newHealth > 0)
        {
            healthBar.value = newHealth;
        }
        else
        {
            healthBar.value = 0;
        }
    }

    //function to handle HUD changes to ammo counter values
    void ammoCounterChange(int newAmmo)
    {
        if (newAmmo >= maxAmmo)
        {
            ammoDisplayUpdate(maxAmmo, maxAmmo);
        }
        else if (newAmmo < maxAmmo && newAmmo > 0)
        {
            ammoDisplayUpdate(newAmmo, maxAmmo);
        }
        else
        {
            ammoDisplayUpdate(0, maxAmmo);
        }
    }

    void ammoDisplayUpdate(int currAmmo, int maxAmmo)
    {
        arrowCountDisplay.text = currAmmo + " / " + maxAmmo;
    }

    //POSS CHANGE - This doesn't feel particularly efficient to me
    //might be more efficient to only call the delegate function in game controller after reaching threshhold values of change.
    //as opposed to every single frame regardless
    void timerDisplayChange(float currTime)
    {
        if (timerDisplay.text != currTime.ToString("f2"))
        {
            timerDisplay.text = currTime.ToString("f2");
        }
    }

    void chargeDisplayUpdate(float currCharge)
    {
        if (currCharge <= 0)
        {
            chargeBar.GetComponentInParent<CanvasGroup>().alpha = 0;
            chargeBar.value = 0;
        }
        else 
        {
            chargeBar.GetComponentInParent<CanvasGroup>().alpha = 1;
            chargeBar.value = currCharge;
        }
    }

}
