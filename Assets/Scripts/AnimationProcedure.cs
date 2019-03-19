using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void action();

public class AnimationProcedure {

    private action action;

    public AnimationProcedure(action _action)
    {
        action = _action;
    }

    public void CallAction()
    {
        //Debug.Log("Call Action: About to call action on animation");
        action();
    }
}
