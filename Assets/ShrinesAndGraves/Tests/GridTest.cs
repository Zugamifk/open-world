using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class GridTest : MonoBehaviour
    {

        Grid grid;
        int W, H;
        Vector2 offset;

        void Start()
        {
            W = (int)(Camera.main.orthographicSize * Camera.main.aspect) * 2 + 1;
            H = (int)Camera.main.orthographicSize * 2 + 1;
            grid = new Grid(W,H);
            offset = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    grid.SetTile(x, y, new Tile());
                }
            }
            //grid[(Vector3i)offset].collides = true;
        }

        void Update()
        {
            Vector2 os = (Vector2)Camera.main.transform.position - offset;
            var mp = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    Color c = grid.GetTile(x,y).collides ? Color.white : Color.green;
                    var rect = new Rect(x + os.x, y + os.y, 1, 1);
                    if(rect.Contains(mp)) c = Color.red;
                    Debugx.DrawRect(rect, c);
                }
            }
            foreach (var r in Math2D.SuperCover(Vector2.zero, mp-os))
            {
                var rect = new Rect(r+os, Vector2.one);
                Debugx.DrawRect(rect, Color.yellow);
            }
            RaycastData data;
            if (grid.Raycast(Vector2.zero, mp - os, out data))
            {
                Debug.DrawLine(os, data.point + os, Color.magenta);
                Debug.Log(data.point);
            }
            else
            {
                Debug.DrawLine(os, mp, Color.blue);
            }
        }
    }
}