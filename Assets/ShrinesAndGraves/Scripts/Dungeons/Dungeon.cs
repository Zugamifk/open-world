using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    public class Dungeon : Region
    {
        public int depth;
        public int maxRooms;
        public Rect roomLimits;
        public int maxHallLength;
        public Vector2i hallHeightRange;
        public int maxExits;

        Queue<Room.Exit> freeExits;

        public override void Fill(Grid g)
        {
            var entrancePos = Random.Range(rect.xMin, rect.xMax);
            var entranceTile = g.surface[entrancePos];

            var entranceWidth = Random.Range(hallHeightRange.x, hallHeightRange.y);
            var entranceHall = new Recti(entrancePos-entranceWidth/2, entranceTile.gridPosition.y-depth+1, entranceWidth, depth);

            g.SetTileData(entranceHall, environment.tileTypes[1]);

            var r = GetRoom();
            var entranceRoomexit = r.AddRandomExit(Room.Side.TOP, entranceWidth);
            var rx = entranceHall.x - entranceRoomexit.interval.x;
            var ry = entranceHall.y - r.size.y;
            g.SetTileData(new Recti(rx, ry, r.size.x, r.size.y), environment.tileTypes[1]);
        }

        public Room GetRoom()
        {
            var room = new Room();;
            room.size = new Vector2i(
                    Random.Range(roomLimits.xMin, roomLimits.xMax),
                    Random.Range(roomLimits.yMin, roomLimits.yMax)
                );
            
            return room;
        }
    }
}