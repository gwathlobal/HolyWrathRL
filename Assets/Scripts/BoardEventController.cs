using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * BoardEventController handles animation events. The logic is as follows - each event has some code, including coroutines.
 * When the event is called, it increases the "inProcessCount" variable. When the event is finished, it should MANUALLY call the "RemoveFinishedEvent" method inside the 
 * "action" closure (thus decreasing the "inProcessCount" variable). 
 * The coroutines that need to block input should MANUALLY increase "coroutinesInProcess" variable when entering themselves, and MANUALLY decrease the 
 * "coroutinesInProcess" when exiting themselves.
 * The event is considered finished, if both conditions are true:  
 * - inProcessCount is zero
 * - coroutinesInProcess is zero
 * Once an event is finshed, BoardEventController takes the next event until the next event.
 * While they are events in queue, or any event or coroutine is being processed the input is blocked and BoardManager does not take the next mob to process.
 */

public class BoardEventController : MonoBehaviour {

    public delegate void action();

    public class Event
    {
        private action action;
        private GameObject mo;

        public Event(GameObject _mo, action _action)
        {
            action = _action;
            mo = _mo;
        }

        public void CallAction()
        {
            //Debug.Log("Call Action: About to call action on animation");
            if (mo.activeSelf)
                action();
            else
                BoardEventController.instance.RemoveFinishedEvent();
        }
    }

    public static BoardEventController instance = null;
    public Queue<Event> toProccessQueue;
    public int inProcessCount;

    public int coroutinesInProcess;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        toProccessQueue = new Queue<Event>();
        inProcessCount = 0;
        coroutinesInProcess = 0;
    }

    void Update()
    {
        if (inProcessCount > 0 || coroutinesInProcess > 0) return;

        if (toProccessQueue.Count > 0)
        {
            //Debug.Log("Move: Queue Count = " + animationQueue.Count + ", time before Process Animation = " + (Time.time - BoardManager.instance.prevTime));
            Event ev = toProccessQueue.Dequeue();
            inProcessCount++;
            ev.CallAction();
            //Debug.Log("Move: Queue Count = " + animationQueue.Count + ", time after Process Animation = " + (Time.time - BoardManager.instance.prevTime));
        }


    }

    public void AddEvent(Event ev)
    {
        toProccessQueue.Enqueue(ev);
    }

    public void RemoveFinishedEvent()
    {
        if (inProcessCount > 0) inProcessCount--;
    }

}
