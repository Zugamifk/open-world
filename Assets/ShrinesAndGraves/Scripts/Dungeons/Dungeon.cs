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
        public int buffer = 10;

        public HazardData spikesData;

        public List<Room> rooms = new List<Room>();
        

        Queue<Room.Exit> freeExits;

        MixedSquareTiling backgroundTiling;

        public override void Fill(Grid g)
        {
            var entrancePos = Random.Range(rect.xMin, rect.xMax);
            var entranceTile = g.surface[entrancePos];

            var entranceWidth = Random.Range(hallHeightRange.x, hallHeightRange.y);
            var entranceHall = new Room(new Recti(entrancePos - entranceWidth / 2, entranceTile.gridPosition.y - depth + 1, entranceWidth, depth));

            var r = GetRoom();
            var entranceRoomexit = r.AddRandomExit(Room.Side.TOP, entranceWidth);
            r.position = new Vector2i(entranceHall.position.x - entranceRoomexit.interval.x, entranceHall.position.y - r.size.y);

            Stack<Room> roomStack = new Stack<Room>();

            roomStack.Push(r);

            SetRoom(g, entranceHall);
            SetRoom(g, r);

            for (int i = 0; i < maxRooms; i++)
            {

                if (roomStack.Count == 0) break;

                r = roomStack.Pop();
                
                var side = Random.value;
                if (side < .25f && r.GetExit(Room.Side.BOTTOM)==null)
                {
                    r = AddExtensionRoom(g, r, Room.Side.BOTTOM);
                    roomStack.Push(r);
                }
                else
                {
                    var er = r.GetExit(Room.Side.RIGHT) != null;
                    var el = r.GetExit(Room.Side.LEFT) != null;
                    if(er && el) {
                        if(r.GetExit(Room.Side.BOTTOM)==null) {
                            r = AddExtensionRoom(g, r, Room.Side.BOTTOM);
                            roomStack.Push(r);
                        }
                    } else if (el)
                    {
                        r = AddExtensionRoom(g, r, Room.Side.RIGHT);
                        roomStack.Push(r);
                    }
                    else if (er)
                    {
                        r = AddExtensionRoom(g, r, Room.Side.LEFT);
                        roomStack.Push(r);
                    }
                    else
                    {
                        if (Random.value >= 0.5f)
                        {
                            r = AddExtensionRoom(g, r, Room.Side.RIGHT);
                            roomStack.Push(r);
                        }
                        else
                        {
                            r = AddExtensionRoom(g, r, Room.Side.LEFT);
                            roomStack.Push(r);
                        }
                    }
                }
            }

            foreach (var room in rooms)
            {
                CleanRoomEntities(room, g);
            }
        }

        Room AddExtensionRoom(Grid g, Room r, Room.Side s)
        {
            var entranceWidth = Random.Range(hallHeightRange.x, hallHeightRange.y);
            var entranceRoomexit = r.AddRandomExit(s, entranceWidth);
            var entranceLength = Random.Range(3, 10);
            Recti entranceRect = default(Recti);
            switch (s)
            {
                case Room.Side.LEFT:
                    entranceRect = new Recti(r.position.x - entranceLength + 1, r.position.y + entranceRoomexit.interval.y, entranceLength, entranceWidth);
                break;
                case Room.Side.RIGHT:
                    entranceRect = new Recti(r.position.x + r.size.x, r.position.y + entranceRoomexit.interval.y, entranceLength, entranceWidth);                    
                break;
                case Room.Side.TOP:
                    entranceRect = new Recti(r.position.x + entranceRoomexit.interval.x, r.position.y + r.size.y, entranceWidth, entranceLength);
                break;
                case Room.Side.BOTTOM:
                    entranceRect = new Recti(r.position.x + entranceRoomexit.interval.x, r.position.y - entranceLength + 1, entranceWidth, entranceLength);                    
                break;
                default:
                    break;
            }
            var entranceHall = new Room(entranceRect);
            r = GetRoom();
            entranceRoomexit = r.AddRandomExit(Room.GetOppositeSide(s), entranceWidth);
            switch (s)
            {
                case Room.Side.LEFT:
                    r.position = new Vector2i(entranceHall.position.x - r.size.x, entranceHall.position.y - entranceRoomexit.interval.x);
                    break;
                case Room.Side.RIGHT:
                    r.position = new Vector2i(entranceHall.position.x + entranceHall.size.x, entranceHall.position.y - entranceRoomexit.interval.x);
                    break;
                case Room.Side.TOP:
                    r.position = new Vector2i(entranceHall.position.x - entranceRoomexit.interval.x, entranceHall.position.y + entranceHall.size.y);
                    break;
                case Room.Side.BOTTOM:
                    r.position = new Vector2i(entranceHall.position.x - entranceRoomexit.interval.x, entranceHall.position.y - r.size.y);
                    break;
                default:
                    break;
            }

            SetRoom(g, entranceHall);
            SetRoom(g, r);

            for (int i = 0; i < r.size.x; i++)
            {
                var x = r.position.x + i;
                var y = r.position.y;
                var ground = g.GetTile(x, y - 1);
                if (ground.collides)
                {
                    var spike = new Hazard(spikesData, new Vector2f16(x, y));
                    g.AddEntity(spike);
                    r.entities.Add(spike);
                }
            }

            int P = Random.Range(1, 5);
            for (int p = 0; p < P; p++)
            {
                int L = Random.Range(3, 12);
                int y = Random.Range(4, r.size.y - 3);
                int x = Random.Range(0, r.size.x);
                for (int l = 0; l < L; l++)
                {
                    var t = g.GetTile(r.position.x+ x + l, r.position.y+y);
                    if (!t.collides)
                    {
                        t.SetData(environment.GetTileData("platform"));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return r;
        }

        public Room GetRoom()
        {
            var room = new Room();
            room.size = new Vector2i(
                    Random.Range(roomLimits.xMin, roomLimits.xMax),
                    Random.Range(roomLimits.yMin, roomLimits.yMax)
                );
            
            return room;
        }

        public void SetRoom(Grid g, Room r)
        {
            
            g.SetTileData(r.rect, environment.GetTileData("empty"));

            rooms.Add(r);

            var areaRect = new Recti(r.position - buffer, r.size + buffer * 2);
            RegionUtility.FillGroundTiles(g, areaRect, environment.GetTileData("ground"));
            RegionUtility.UpdateDepths(g, areaRect, r.rect);
        }

        void CleanRoomEntities(Room r, Grid g)
        {
            var tr = new List<Entity>();
            foreach (var e in r.entities)
            {
                var x = e.position.x;
                var y = e.position.y;
                var t = g.GetTile(x, y - 1);
                if (t == null || !t.collides)
                {
                    g.Removeentity(e);
                    tr.Add(e);
                }
            }
            foreach (var e in tr)
            {
                r.entities.Remove(e);
            }
        }
    }
}