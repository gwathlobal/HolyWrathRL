using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool los_func(int x, int y, int prev_x, int prev_y);

public static class LOS_FOV
{
    static public bool DrawLine(int x0, int y0, int x1, int y1, los_func func)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int prev_x = x0, prev_y = y0;
        int err = (dx > dy ? dx : -dy) / 2, e2;
        bool result;
        for (; ; )
        {
            result = func(x0, y0, prev_x, prev_y);
            if (!result) return false;
            if (x0 == x1 && y0 == y1) break;
            prev_x = x0; prev_y = y0;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
        return true;
    }

    static public void DrawFOV(int cx, int cy, int r, los_func func)
    {
        DrawCircleFOV(cx, cy, r, func);
    }

    static private void DrawSquareFOV(int cx, int cy, int r, los_func func)
    {
        List<Vector2Int> cells = new List<Vector2Int>(); 
        for (int x = cx - r ; x <= cx + r ; x++ )
        {
            cells.Add(new Vector2Int(x, cy - r));
            cells.Add(new Vector2Int(x, cy + r));
        }
        for (int y = cy - r; y <= cy + r; y++)
        {
            cells.Add(new Vector2Int(cx - r, y));
            cells.Add(new Vector2Int(cx + r, y));
        }
        foreach(Vector2Int cell in cells)
        {
            DrawLine(cx, cy, cell.x, cell.y, func);
        }
    }

    static private void DrawCircleFOV(int cx, int cy, int r, los_func func)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        int tx, ty;
        for (int i = 0; i <= 360; i++)
        {
            tx = cx + (int)Math.Round(r * Math.Cos(i * (Math.PI / 180)));
            ty = cy - (int)Math.Round(r * Math.Sin(i * (Math.PI / 180)));

            if (!cells.Exists((v) => (v.x == tx)&&(v.y == ty)))
            {
                cells.Add(new Vector2Int(tx, ty));
            }
        }
        foreach (Vector2Int cell in cells)
        {
            DrawLine(cx, cy, cell.x, cell.y, func);
        }

    }
}