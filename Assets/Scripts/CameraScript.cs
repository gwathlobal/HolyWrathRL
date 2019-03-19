using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {

    public GameObject player;
    float speed = 10;

    // Use this for initialization
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float camWidth = GlobalConsts.SpriteSize * GlobalConsts.MapViewW;
        float camHeight = GlobalConsts.SpriteSize * GlobalConsts.MapViewH;

        GetComponent<Camera>().rect = new Rect(
            ((screenWidth / 2) - (camWidth / 2)) / screenWidth,
            1 - camHeight / screenHeight,
            camWidth / screenWidth,
            camHeight / screenHeight);
        GetComponent<Camera>().orthographicSize = camWidth / (((camWidth / camHeight) * 2) * GlobalConsts.SpriteSize);
    }

    void Update()
    {
        Level level = BoardManager.instance.level;
        float x = transform.position.x;
        float y = transform.position.y;
        Vector3 screenPoint = GetComponent<Camera>().ViewportToScreenPoint(new Vector3(0, 0, 0));
        float camWidth = GlobalConsts.SpriteSize * GlobalConsts.MapViewW;
        float camHeight = GlobalConsts.SpriteSize * GlobalConsts.MapViewH;

        switch (UIManager.instance.screenStatus)
        {
            case MainScreenStatus.statusLookAt:
                if (Input.mousePosition.x > screenPoint.x + camWidth - GlobalConsts.SpriteSize * 3 &&
                    Input.mousePosition.x < screenPoint.x + camWidth)
                {
                    x += speed * Time.deltaTime; 
                }
                if (Input.mousePosition.x < screenPoint.x + GlobalConsts.SpriteSize * 3 &&
                    Input.mousePosition.x > screenPoint.x)
                {
                    x -= speed * Time.deltaTime; 
                }
                if (Input.mousePosition.y > screenPoint.y + camHeight - GlobalConsts.SpriteSize * 3 &&
                    Input.mousePosition.y < screenPoint.y + camHeight)
                {
                    y += speed * Time.deltaTime; 
                }
                if (Input.mousePosition.y < screenPoint.y + GlobalConsts.SpriteSize * 3 &&
                    Input.mousePosition.y > screenPoint.y)
                {
                    y -= speed * Time.deltaTime; 
                }
                break;
            default:
                x = player.transform.position.x + 0.5f;
                y = player.transform.position.y - 0.5f;
                break;
        }

        if (x - GlobalConsts.MapViewW / 2f < 0) x = (GlobalConsts.MapViewW / 2f);
        if (y - GlobalConsts.MapViewH / 2f + 1 < 0) y = (GlobalConsts.MapViewH / 2f) - 1;
        if (x + GlobalConsts.MapViewW / 2f >= level.maxX) x = level.maxX - (GlobalConsts.MapViewW / 2f);
        if (y + GlobalConsts.MapViewH / 2f + 1 >= level.maxY) y = level.maxY - (GlobalConsts.MapViewH / 2f) - 1;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
