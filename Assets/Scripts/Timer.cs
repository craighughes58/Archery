/******************************************************************************
 * Author: MaKayla Elder
 * 
 * Date: 6.14.23
 * 
 * Description:
 *  A singular timer object to be used for repeat operations.
 * 
 * Use notes:
 * 
 * Use bHasTimerStarted to determine if a timer is running. After starting a timer, use bHasTimerCompleted to check if finished.
 * bool started changes to false when timer is completed, so this may be more useful to check if looping timers
 * Use StopTimer() or ResetTimer() to stop Timer, ResetTimer() has the added benefit of setting the time to 0;
 *Be sure to use ResetTimer() when a particular operation no longer needs it
 *This will ensure a clean timer for the next use.
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float timer;
    private float StoppedTime;
    private bool stopped;
    private bool completed;
    private bool started;
    private Coroutine TimerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        stopped = true;
        started = false;
        TimerCoroutine = StartCoroutine(TickDownTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //where the magic happens
    public IEnumerator TickDownTimer()
    {
        while (timer > 0)
        {
            if (stopped)
            {
                yield break;
            }
            timer -= Time.deltaTime;
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (!stopped)
        {
            completed = true;
        }
        started = false;
        yield return null;
    }

    public void StopTimer() 
    { 
        StopCoroutine(TimerCoroutine);
        stopped = true;
        started = false;
        completed = false;
        StoppedTime += timer;
    }
    public void ResetTimer()
    {
        StopCoroutine(TimerCoroutine);
        stopped = true;
        started = false;
        completed = false;
        timer = 0;
    }
    public void ResumeTimer()
    {
        started = true;
        completed = false;
        stopped = false;
        //timer = the remaining time stored in StoppedTime when StopTimer() was used
        timer = StoppedTime;
        StartCoroutine(TickDownTimer());

    }

    public void NewTimer(float Duration)
    {
        started = true;
        completed = false;
        stopped = false;
        timer = Duration;
        StartCoroutine(TickDownTimer());
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
