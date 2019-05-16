using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void SmoothMeleeMiddleFunc();


public class MovingObject : MonoBehaviour
{

    public float moveTime;
    public float inverseMoveTime;
    public Rigidbody2D rb2D;
    //protected bool finishedMove = true;

    // Use this for initialization
    void Awake()
    {
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
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
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
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                //Debug.Log("Call Action - Move: About to start Melee Attack");
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
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                if (this.gameObject.activeSelf)
                    StartCoroutine(SmoothMovementProjectile(end, str, middleFunc));
            }));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void DisappearAfterAWhile(int tx, int ty)
    {
        if (BoardManager.instance.level.visible[tx, ty])
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothDisappear());
            }));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ExplosionCone(int sx, int sy, List<Vector2Int> dstLine, List<Vector3Int> mobStr)
    {
        List<Vector2Int> explosions = new List<Vector2Int>();
        Level level = BoardManager.instance.level;
        foreach (Vector2Int dst in dstLine)
        {
            LOS_FOV.DrawLine(sx, sy, dst.x, dst.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (blocks) return false;

                    if (!(sx == x && sy == y) && level.visible[x, y])
                    {
                        explosions.Add(new Vector2Int(x, y));
                    }
                    return true;
                });
        }
        if (explosions.Count > 0)
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothExplosionCone(sx, sy, explosions, mobStr));
            }));
        }
    }

    public void Explosion3x3(int sx, int sy)
    {
        bool result = false;
        Level level = BoardManager.instance.level;
        level.CheckSurroundings(sx, sy, true,
            (int x, int y) =>
            {
                if (level.visible[x, y])
                    result = true;
            });
        if (result)
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothExplosion3x3(sx, sy));
            }));
        }
    }

    public void Explosion5x5(int sx, int sy)
    {
        bool result = false;
        Level level = BoardManager.instance.level;

        LOS_FOV.DrawFOV(sx, sy, 2,
            (int dx, int dy, int pdx, int pdy) =>
            {
                if (level.visible[dx, dy])
                {
                    result = true;
                }

                if (TerrainTypes.terrainTypes[level.terrain[dx, dy]].blocksMovement) return false;

                return true;
            });

        if (result)
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothExplosion5x5(sx, sy));
            }));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void TeleportDisappear(int tx, int ty)
    {
        if (BoardManager.instance.level.visible[tx, ty])
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothFadeTo(0f, 0.15f));
            }));
        }
        else
        {
            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
        }
    }

    public void TeleportReappear(int tx, int ty)
    {
        if (BoardManager.instance.level.visible[tx, ty])
        {
            BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(this.gameObject, () =>
            {
                StartCoroutine(SmoothFadeTo(1f, 0.15f));
            }));
        }
        else
        {
            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
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

    protected IEnumerator SmoothDisappear()
    {
        float curTime = 0;

        while (curTime < moveTime)
        {
            curTime += Time.deltaTime;

            yield return null;
        }

        Destroy(this.gameObject);
        BoardAnimationController.instance.RemoveProcessedAnimation();

    }

    protected IEnumerator SmoothExplosionCone(int sx, int sy, List<Vector2Int> explosionPos, List<Vector3Int> mobStr)
    {
        List<GameObject> explosions = new List<GameObject>();

        int row = 0;
        float waitTime = 0.03f;

        while (row <= 6)
        {
            foreach (Vector2Int pos in explosionPos)
            {
                if (Level.GetSimpleDistance(sx, sy, pos.x, pos.y) == row)
                {
                    GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    if (Random.Range(0, 5) == 0)
                        explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 168, 0, 255);
                    else
                        explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                    explosions.Add(explosion);
                };
            }

            foreach (Vector3Int pos in mobStr)
            {
                if (Level.GetSimpleDistance(sx, sy, pos.x, pos.y) == row)
                {
                    UIManager.instance.CreateFloatingText(pos.z + " <i>DMG</i>", new Vector3(pos.x, pos.y, 0));
                };
            }

            row++;
            yield return new WaitForSeconds(waitTime);
        }

        for (int i = explosions.Count - 1; i >= 0; i--)
        {
            Destroy(explosions[i]);
        }

        BoardAnimationController.instance.RemoveProcessedAnimation();

    }

    protected IEnumerator SmoothExplosion3x3(int sx, int sy)
    {
        List<GameObject> explosions = new List<GameObject>();
        float waitTime = 0.1f;
        Level level = BoardManager.instance.level;

        if (level.visible[sx, sy])
        {
            GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(sx, sy, 0), Quaternion.identity);
            if (Random.Range(0, 5) == 0)
                explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 168, 0, 255);
            else
                explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
            explosions.Add(explosion);
        }
        yield return new WaitForSeconds(waitTime);

        level.CheckSurroundings(sx, sy, false,
            (int x, int y) =>
            {
                if (level.visible[x, y])
                {
                    GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    if (Random.Range(0, 5) == 0)
                        explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 168, 0, 255);
                    else
                        explosion.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                    explosions.Add(explosion);
                } 
            });
        yield return new WaitForSeconds(waitTime);

        for (int i = explosions.Count - 1; i >= 0; i--)
        {
            Destroy(explosions[i]);
        }
        BoardAnimationController.instance.RemoveProcessedAnimation();
    }

    protected IEnumerator SmoothExplosion5x5(int sx, int sy)
    {
        List<GameObject> explosions = new List<GameObject>();
        float waitTime = 0.1f;
        Level level = BoardManager.instance.level;
        Color32 color1 = new Color32(255, 168, 0, 255);
        Color32 color2 = new Color32(255, 0, 0, 255);

        List<Vector2Int> ring1 = new List<Vector2Int>();
        List<Vector2Int> ring2 = new List<Vector2Int>();

        LOS_FOV.DrawFOV(sx, sy, 2,
            (int dx, int dy, int pdx, int pdy) =>
            {
                if (Level.GetDistance(sx, sy, dx, dy) > 0 && Level.GetDistance(sx, sy, dx, dy) <= 2 && !ring1.Contains(new Vector2Int(dx, dy)))
                    ring1.Add(new Vector2Int(dx, dy));

                if (Level.GetDistance(sx, sy, dx, dy) > 2 && Level.GetDistance(sx, sy, dx, dy) <= 3 && !ring2.Contains(new Vector2Int(dx, dy)))
                    ring2.Add(new Vector2Int(dx, dy));

                if (TerrainTypes.terrainTypes[level.terrain[dx, dy]].blocksMovement) return false;

                return true;
            });

        if (level.visible[sx, sy])
        {
            GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(sx, sy, 0), Quaternion.identity);
            if (Random.Range(0, 5) == 0)
                explosion.GetComponent<SpriteRenderer>().color = color1;
            else
                explosion.GetComponent<SpriteRenderer>().color = color2;
            explosions.Add(explosion);
        }
        yield return new WaitForSeconds(waitTime);

        foreach (Vector2Int loc in ring1)
        {
            if (level.visible[loc.x, loc.y])
            {
                GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(loc.x, loc.y, 0), Quaternion.identity);
                if (Random.Range(0, 5) == 0)
                    explosion.GetComponent<SpriteRenderer>().color = color1;
                else
                    explosion.GetComponent<SpriteRenderer>().color = color2;
                explosions.Add(explosion);
            }
        }
        yield return new WaitForSeconds(waitTime);

        foreach (Vector2Int loc in ring2)
        {
            if (level.visible[loc.x, loc.y])
            {
                GameObject explosion = GameObject.Instantiate(UIManager.instance.explosionPrefab, new Vector3(loc.x, loc.y, 0), Quaternion.identity);
                if (Random.Range(0, 5) == 0)
                    explosion.GetComponent<SpriteRenderer>().color = color1;
                else
                    explosion.GetComponent<SpriteRenderer>().color = color2;
                explosions.Add(explosion);
            }
        }
        yield return new WaitForSeconds(waitTime);

        for (int i = explosions.Count - 1; i >= 0; i--)
        {
            Destroy(explosions[i]);
        }
        Destroy(this.gameObject);
        BoardAnimationController.instance.RemoveProcessedAnimation();
    }

    protected IEnumerator SmoothTeleportDisappear()
    {
        float curTime = 0;
        float waitTime = 0.1f;
        float alphaStep = 255 / waitTime;
        Color32 color = gameObject.GetComponent<SpriteRenderer>().color;

        while (gameObject.GetComponent<SpriteRenderer>().color.a > 0)
        {
            curTime += Time.deltaTime;

            gameObject.GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, (byte)(255 - Time.deltaTime * alphaStep));

            yield return null;
        }

        BoardAnimationController.instance.RemoveProcessedAnimation();
    }

    protected IEnumerator SmoothTeleportReappear()
    {
        float curTime = 0;
        float waitTime = 0.1f;
        float alphaStep = 255 / waitTime;
        Color32 color = gameObject.GetComponent<SpriteRenderer>().color;

        while (gameObject.GetComponent<SpriteRenderer>().color.a < 1)
        {
            curTime += Time.deltaTime;

            gameObject.GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, (byte)(0 + Time.deltaTime * alphaStep));

            yield return null;
        }

        BoardAnimationController.instance.RemoveProcessedAnimation();
    }

    IEnumerator SmoothFadeTo(float aValue, float aTime)
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        float alpha = color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(alpha, aValue, t));
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        BoardAnimationController.instance.RemoveProcessedAnimation();
    }
}