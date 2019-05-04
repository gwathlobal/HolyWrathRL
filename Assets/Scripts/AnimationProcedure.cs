using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void action();

public class AnimationProcedure {

    private action action;
    private GameObject mo;

    public AnimationProcedure(GameObject _mo, action _action)
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
            BoardAnimationController.instance.RemoveProcessedAnimation();
    }
}
