using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimationController : MonoBehaviour {

    public static BoardAnimationController instance = null;
    public Queue<AnimationProcedure> toProccessQueue;
    public int inProcessCount;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);


    }

    // Use this for initialization
    void Start () {
        toProccessQueue = new Queue<AnimationProcedure>();
        inProcessCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (inProcessCount > 0) return;

        if (toProccessQueue.Count > 0)
        {
            //Debug.Log("Move: Queue Count = " + animationQueue.Count + ", time before Process Animation = " + (Time.time - BoardManager.instance.prevTime));
            AnimationProcedure ap = toProccessQueue.Dequeue();
            inProcessCount++;
            ap.CallAction();
            //Debug.Log("Move: Queue Count = " + animationQueue.Count + ", time after Process Animation = " + (Time.time - BoardManager.instance.prevTime));
        }


    }

    public void AddAnimationProcedure(AnimationProcedure ap)
    {
        toProccessQueue.Enqueue(ap);
    }

    public void RemoveProcessedAnimation()
    {
        if (inProcessCount > 0) inProcessCount--;
    }

}
