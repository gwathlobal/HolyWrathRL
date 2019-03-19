using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void SmoothMeleeMiddleFunc();


public class MovingObject : MonoBehaviour {

    public float moveTime;
    public float inverseMoveTime;
    public Rigidbody2D rb2D;
    //protected bool finishedMove = true;

    // Use this for initialization
    void Awake() {
        inverseMoveTime = 1f / moveTime;
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void Move(int tx, int ty)
    {

        Vector2 start = transform.position;
        Vector2 end = new Vector2(tx, ty);
        int xDir = (int)(end.x - start.x);
        int yDir = (int)(end.y - start.y);

        //Debug.Log("Move: About to dispatch animation");

        if ((BoardManager.instance.level.visible[(int)end.x, (int)end.y] || BoardManager.instance.level.visible[(int)start.x, (int)start.y]) &&
            !(xDir == 0 && yDir == 0))
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(() => {
                //Debug.Log("Call Action - Move: About to start Smooth Movement");
                StartCoroutine(SmoothMovement(end));
            }));
        }
        else
        {
            rb2D.MovePosition(end);
        }

        //Debug.Log("Move: animation dispatched");
    }

    public void MeleeAttack(int xDir, int yDir, string str, SmoothMeleeMiddleFunc middleFunc)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        if ((BoardManager.instance.level.visible[(int)end.x, (int)end.y] || BoardManager.instance.level.visible[(int)start.x, (int)start.y]) &&
            !(xDir == 0 && yDir == 0))
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(() => {
                //Debug.Log("Call Action - Move: About to start Smooth Movement");
                StartCoroutine(SmoothMelee(end, str, middleFunc));
            }));
        }
        else
        {
            middleFunc();
        }

        //Debug.Log("Move: animation dispatched");
    }

    public void MoveProjectile(int tx, int ty, string str, SmoothMeleeMiddleFunc middleFunc)
    {
        Vector2 start = transform.position;
        Vector2 end = new Vector2(tx, ty);
        int xDir = (int)(end.x - start.x);
        int yDir = (int)(end.y - start.y);

        if ((BoardManager.instance.level.visible[(int)end.x, (int)end.y] || BoardManager.instance.level.visible[(int)start.x, (int)start.y]) &&
            !(xDir == 0 && yDir == 0))
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(() => {
                StartCoroutine(SmoothMovementProjectile(end, str, middleFunc));
            }));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Debug.Log("Smooth Movement");
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        BoardAnimationController.instance.RemoveProcessedAnimation();
        //BoardAnimationController.instance.ProcessNext();
        //finishedMove = true;
    }


    protected IEnumerator SmoothMelee(Vector3 end, string str, SmoothMeleeMiddleFunc middleFunc)
    {
        Vector3 start = transform.position;
        Vector3 halfpoint = (start + end) / 2;

        float sqrRemainingDistance = (transform.position - halfpoint).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, halfpoint, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - halfpoint).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        UIManager.instance.CreateFloatingText(str, end);

        middleFunc();

        sqrRemainingDistance = (transform.position - start).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, start, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - start).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        BoardAnimationController.instance.RemoveProcessedAnimation();
    }

    protected IEnumerator SmoothMovementProjectile(Vector3 end, string str, SmoothMeleeMiddleFunc middleFunc)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        UIManager.instance.CreateFloatingText(str, end);

        middleFunc();

        Destroy(this.gameObject);
        BoardAnimationController.instance.RemoveProcessedAnimation();
        
    }

}
