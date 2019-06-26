using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MovingObject : MonoBehaviour
{

    public delegate void DebuffMiddleFunc();
    public delegate void TeleportMiddleFunc();
    public delegate void MeleeMiddleFunc();
    public delegate void MoveMiddleFunc();
    public delegate void ProjectileMiddleFunc();
    public delegate void BreathEachTileFunc(int x, int y);
    public delegate void BreathEachMobFunc(Mob mob);
    public delegate void MindBurnMiddleFunc();

    public float moveTime;
    public float inverseMoveTime;
    public Rigidbody2D rb2D;
    public int coroutinesRunning;
    //public StateEnum state;
    //protected bool finishedMove = true;

    // Use this for initialization
    void Awake()
    {
        inverseMoveTime = 1f / moveTime;
        rb2D = GetComponent<Rigidbody2D>();
        coroutinesRunning = 0;
    }

    public void Move(Vector2 end, bool visible, MoveMiddleFunc middleFunc)
    {
        if (visible)
        {
            StartCoroutine(CoroutineMovement(end, middleFunc));
        }
        else
        {
            rb2D.MovePosition(end);
            if (middleFunc != null) middleFunc();
        }
    }

    protected IEnumerator CoroutineMovement(Vector3 end, MoveMiddleFunc middleFunc)
    {
        yield return StartCoroutine(CoroutineMoveObject(end));
        yield return StartCoroutine(CoroutineMoveMiddleFunc(middleFunc));
    }

    protected IEnumerator CoroutineMoveObject(Vector3 end)
    {
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition(newPostion);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }
        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
    }

    protected IEnumerator CoroutineMoveMiddleFunc(MoveMiddleFunc middleFunc)
    {
        //Debug.Log("Move Middle Func started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        if (middleFunc != null) middleFunc();
        yield return null;

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //Debug.Log("Move Middle Func ended");
    }

    public void MeleeAttack(Vector2 start, Vector2 end, bool visible, MeleeMiddleFunc middleFunc)
    {
        if (visible)
        {
            StartCoroutine(CoroutineMeleeAttack(start, end, middleFunc));
        }
        else
        {
            middleFunc();
        }
    }

    protected IEnumerator CoroutineMeleeAttack(Vector2 start, Vector2 end, MeleeMiddleFunc middleFunc)
    {
        yield return StartCoroutine(CoroutineMeleeMoveIn(end));
        yield return StartCoroutine(CoroutineMeleeMiddleFunc(middleFunc));
        yield return StartCoroutine(CoroutineMeleeMoveBack(start));
    }

    public void MoveAndAttack(Vector2 movPos, Vector2 attPos, bool visible, MoveMiddleFunc moveFunc, MeleeMiddleFunc meleeFunc)
    {
        if (visible)
        {
            StartCoroutine(CoroutineMoveAndAttackFull(movPos, attPos, moveFunc, meleeFunc));
        }
        else
        {
            StartCoroutine(CoroutineMoveAndAttackShort(movPos, moveFunc, meleeFunc));
        }
    }

    protected IEnumerator CoroutineMoveAndAttackFull(Vector2 movPos, Vector2 attPos, MoveMiddleFunc moveFunc, MeleeMiddleFunc meleeFunc)
    {
        yield return StartCoroutine(CoroutineMoveObject(movPos));
        yield return StartCoroutine(CoroutineMoveMiddleFunc(moveFunc));
        yield return StartCoroutine(CoroutineMeleeMoveIn(attPos));
        yield return StartCoroutine(CoroutineMeleeMiddleFunc(meleeFunc));
        yield return StartCoroutine(CoroutineMeleeMoveBack(movPos));
    }

    protected IEnumerator CoroutineMoveAndAttackShort(Vector2 movPos, MoveMiddleFunc moveFunc, MeleeMiddleFunc meleeFunc)
    {
        rb2D.MovePosition(movPos);
        yield return StartCoroutine(CoroutineMoveMiddleFunc(moveFunc));
        yield return StartCoroutine(CoroutineMeleeMiddleFunc(meleeFunc));
    }

    public void MoveProjectile(int tx, int ty, string str, MeleeMiddleFunc middleFunc)
    {
        Vector2 start = transform.position;
        Vector2 end = new Vector2(tx, ty);
        int xDir = (int)(end.x - start.x);
        int yDir = (int)(end.y - start.y);

        if ((BoardManager.instance.level.visible[(int)end.x, (int)end.y] || BoardManager.instance.level.visible[(int)start.x, (int)start.y]) &&
            !(xDir == 0 && yDir == 0))
        {
            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
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
            StartCoroutine(SmoothDisappear());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ExplosionCone(int sx, int sy, List<Vector2Int> dstLine, List<Mob> affectedMobs, BreathEachMobFunc EachMobFunc, BreathEachTileFunc EachTileFunc)
    {
        List<Vector2Int> explosions = new List<Vector2Int>();
        bool visible = false;
        Level level = BoardManager.instance.level;
        foreach (Vector2Int dst in dstLine)
        {
            LOS_FOV.DrawLine(sx, sy, dst.x, dst.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (blocks) return false;

                    if (!(sx == x && sy == y) && level.visible[x, y])
                        visible = true;

                    if (!(sx == x && sy == y))
                    {
                        explosions.Add(new Vector2Int(x, y));
                    }
                    return true;
                });
        }
        if (visible)
        {
            StartCoroutine(SmoothExplosionCone(sx, sy, explosions, affectedMobs, EachMobFunc, EachTileFunc));
        }
        else
        {
            foreach (Vector2Int pos in explosions)
            {
                EachTileFunc(pos.x, pos.y);
            }

            foreach (Mob mob in affectedMobs)
            {
                EachMobFunc(mob);
            }
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
            StartCoroutine(SmoothExplosion3x3(sx, sy));
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
            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                StartCoroutine(SmoothExplosion5x5(sx, sy));
                BoardEventController.instance.RemoveFinishedEvent();
            }));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Teleport(bool visibleStart, bool visibleEnd, TeleportMiddleFunc middleFunc)
    {
        if (visibleStart || visibleEnd)
        {
            StartCoroutine(CoroutineTeleport(middleFunc));
        }
        else
        {
            middleFunc();
        }
    }

    private IEnumerator CoroutineTeleport(TeleportMiddleFunc middleFunc)
    {
        yield return StartCoroutine(CoroutineFadeTo(0f, 0.15f));
        yield return StartCoroutine(CoroutineTeleportMiddleFunc(middleFunc));
        yield return StartCoroutine(CoroutineFadeTo(1f, 0.15f));
    }

    public void BuffDebuff(Vector2Int startPos, Vector2Int endPos, GameObject debuffStartPrefab, GameObject debufEndPrefab, DebuffMiddleFunc middleFunc)
    {
        if (BoardManager.instance.level.visible[startPos.x, startPos.y] && debuffStartPrefab != null)  
        {
            GameObject debuffStart = GameObject.Instantiate(debuffStartPrefab,
                    new Vector3(startPos.x + 0.5f, startPos.y - 0.5f, this.gameObject.transform.position.z),
                    Quaternion.identity);

            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                debuffStart.GetComponent<MovingObject>().StartCoroutine(debuffStart.GetComponent<MovingObject>().CoroutineFadeTo(0f, 0.15f));
                debuffStart.GetComponent<MovingObject>().StartCoroutine(debuffStart.GetComponent<MovingObject>().CoroutineScaleTo(3f, 0.15f));
                BoardEventController.instance.RemoveFinishedEvent();
            }));

            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                Destroy(debuffStart.gameObject);
                BoardEventController.instance.RemoveFinishedEvent();
            }));
        }

        BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
        {
            if (middleFunc != null) middleFunc();
            BoardEventController.instance.RemoveFinishedEvent();
        }));

        if (BoardManager.instance.level.visible[endPos.x, endPos.y] && debufEndPrefab != null)
        {
            GameObject debuffEnd = GameObject.Instantiate(debufEndPrefab,
                    new Vector3(endPos.x + 0.5f, endPos.y - 0.5f, this.gameObject.transform.position.z),
                    Quaternion.identity);
            debuffEnd.transform.localScale = new Vector3(3, 3, 1);
            debuffEnd.GetComponent<SpriteRenderer>().color = new Color(debufEndPrefab.GetComponent<SpriteRenderer>().color.r,
                debufEndPrefab.GetComponent<SpriteRenderer>().color.g,
                debufEndPrefab.GetComponent<SpriteRenderer>().color.b,
                0);

            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                debuffEnd.GetComponent<MovingObject>().StartCoroutine(debuffEnd.GetComponent<MovingObject>().CoroutineFadeTo(1f, 0.15f));
                debuffEnd.GetComponent<MovingObject>().StartCoroutine(debuffEnd.GetComponent<MovingObject>().CoroutineScaleTo(1f, 0.15f));
                BoardEventController.instance.RemoveFinishedEvent();
            }));

            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                Destroy(debuffEnd.gameObject);
                BoardEventController.instance.RemoveFinishedEvent();
            }));
        }
    }

    public void MindBurn(Mob target, MindBurnMiddleFunc middleFunc)
    {
        if (BoardManager.instance.level.visible[target.x, target.y]) 
        {
            GameObject mindBurn = GameObject.Instantiate(target.go, new Vector3(target.x, target.y, 0), Quaternion.identity);
            mindBurn.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);

            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                mindBurn.GetComponent<MovingObject>().StartCoroutine(mindBurn.GetComponent<MovingObject>().CoroutineMindBurn(middleFunc));
                BoardEventController.instance.RemoveFinishedEvent();
            }));
            
            BoardEventController.instance.AddEvent(new BoardEventController.Event(this.gameObject, () =>
            {
                Destroy(mindBurn.gameObject);
                BoardEventController.instance.RemoveFinishedEvent();
            }));
        }
        else
        {
            middleFunc();
        }
    }

    public IEnumerator CoroutineMindBurn(MindBurnMiddleFunc middleFunc)
    {
        yield return StartCoroutine(CoroutineFadeTo(0.6f, 0.15f));
        yield return StartCoroutine(CoroutineMindBurnMiddleFunc(middleFunc));
        yield return StartCoroutine(CoroutineFadeTo(0f, 0.15f));
    }

    protected IEnumerator CoroutineMeleeMoveIn(Vector3 end)
    {
        //Debug.Log("Melee Move In, state = " + state.ToString());
        //if (state != StateEnum.meleeMoveIn) yield return null;

        //Debug.Log("Melee Move In started");

        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        Vector3 halfpoint = (transform.position + end) / 2;

        float sqrRemainingDistance = (transform.position - halfpoint).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, halfpoint, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition(newPostion);

            sqrRemainingDistance = (transform.position - halfpoint).sqrMagnitude;

            yield return null;
        }
        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //Debug.Log("Melee Move In ended");
    }

    protected IEnumerator CoroutineMeleeMiddleFunc(MeleeMiddleFunc middleFunc)
    {
        //Debug.Log("Melee Middle Func started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        middleFunc();
        yield return null;

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //state = StateEnum.meleeMoveOut;
        //Debug.Log("Melee Middle Func ended");
    }

    protected IEnumerator CoroutineMeleeMoveBack(Vector3 start)
    {
        //Debug.Log("Melee Move Back started");

        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;
        float sqrRemainingDistance = (transform.position - start).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, start, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition(newPostion);

            sqrRemainingDistance = (transform.position - start).sqrMagnitude;

            yield return null;
        }
        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //Debug.Log("Melee Move Back ended");
    }

    protected IEnumerator SmoothMovementProjectile(Vector3 end, string str, MeleeMiddleFunc middleFunc)
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
        BoardEventController.instance.RemoveFinishedEvent();

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
    }

    protected IEnumerator SmoothExplosionCone(int sx, int sy, List<Vector2Int> explosionPos, List<Mob> affectedMobs, BreathEachMobFunc EachMobFunc, BreathEachTileFunc EachTileFunc)
    {
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;
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

                    EachTileFunc(pos.x, pos.y);
                };
            }

            foreach (Mob mob in affectedMobs)
            {
                if (Level.GetSimpleDistance(sx, sy, mob.x, mob.y) == row)
                {
                    EachMobFunc(mob);
                }
            }

            row++;
            yield return new WaitForSeconds(waitTime);
        }

        for (int i = explosions.Count - 1; i >= 0; i--)
        {
            Destroy(explosions[i]);
        }

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
    }

    protected IEnumerator SmoothExplosion3x3(int sx, int sy)
    {
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

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

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
    }

    protected IEnumerator SmoothExplosion5x5(int sx, int sy)
    {
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;
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

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
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

        BoardEventController.instance.RemoveFinishedEvent();
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

        BoardEventController.instance.RemoveFinishedEvent();
    }

    IEnumerator CoroutineFadeTo(float aValue, float aTime)
    {
        //Debug.Log("Fade To " + aValue + " started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        float alpha = color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(alpha, aValue, t));
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }
        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //Debug.Log("Fade To " + aValue + " ended");
    }

    IEnumerator CoroutineScaleTo(float scaleValue, float aTime)
    {
        //Debug.Log("Fade To " + aValue + " started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;
        Vector3 scale = gameObject.transform.localScale;
        float scaleX = scale.x;
        float scaleY = scale.y;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Vector3 newScale = new Vector3(Mathf.Lerp(scaleX, scaleValue, t), Mathf.Lerp(scaleY, scaleValue, t), scale.z);
            gameObject.transform.localScale = newScale;
            yield return null;
        }
        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //Debug.Log("Fade To " + aValue + " ended");
    }

    protected IEnumerator CoroutineTeleportMiddleFunc(TeleportMiddleFunc middleFunc)
    {
        //Debug.Log("Melee Middle Func, state = " + state.ToString());
        //if (state != StateEnum.meleeMiddleFunc) yield return null;
        //Debug.Log("Teleport Middle Func started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        middleFunc();
        yield return null;

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //state = StateEnum.meleeMoveOut;
        //Debug.Log("Teleport Middle Func ended");
    }

    protected IEnumerator CoroutineMindBurnMiddleFunc(MindBurnMiddleFunc middleFunc)
    {
        //Debug.Log("Melee Middle Func, state = " + state.ToString());
        //if (state != StateEnum.meleeMiddleFunc) yield return null;
        //Debug.Log("Teleport Middle Func started");
        BoardEventController.instance.coroutinesInProcess++;
        coroutinesRunning++;

        middleFunc();
        yield return null;

        coroutinesRunning--;
        BoardEventController.instance.coroutinesInProcess--;
        //state = StateEnum.meleeMoveOut;
        //Debug.Log("Teleport Middle Func ended");
    }
}