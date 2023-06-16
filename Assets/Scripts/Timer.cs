/******************************************************************************
 * Author: MaKayla Elder
 * 
 * Date: 6.14.23
 * 
 * Description:
 *  An singular timer to be used for repeat operations.
 * 
 * 
 * 
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float timer;
    private bool stopped;
    private bool completed;
    private bool started;
    Timer()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        stopped = true;
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TickDownTimer()
    {
        if(stopped)
        {
            yield break;
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
       
       completed = true;
        started = false;
        yield return null;
    }

    public void StopTimer() 
    { 
        stopped = true;
        started = false;
    }   

    public void NewTimer(float Duration)
    {
        started = true;
        completed = false;
        stopped = false;
        timer = Duration;
        StartCoroutine(TickDownTimer());
    }

    public bool bHasTimerEnded()
    {
        return timer == 0;
    }
    public bool bHasTimerStarted()
    {
        return started;
    }
    public bool bHasTimerCompleted()
    {
        return completed;
    }

    public float GetTimeLeft() 
    { 
        return timer; 
    }
}
