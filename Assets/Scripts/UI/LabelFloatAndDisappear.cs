using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelFloatAndDisappear : MonoBehaviour {

    public float moveTime = 0.02f;
    private float inverseMoveTime;
    public Vector3 end;

    // Use this for initialization
    void Start () {
        inverseMoveTime = 1f / moveTime;
    }
	
	// Update is called once per frame
	void Update () {

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        if (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPostion;
        }
        else Destroy(gameObject);
    }
}
