using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Shared;
using WorldServer.Game.Entity;

namespace WorldServer.Game.Map
{
    public class WorldGrid
    {
        public const int Size        = 128;
        public const int CellBreadth = Size / WorldCell.Size;

        private readonly Vector2G gridCoordinates;
        private readonly Dictionary<Vector2G, WorldCell> cells = new Dictionary<Vector2G, WorldCell>();

        public static Vector2G GetCoord(Vector3 position)
        {
            int gx = (int)Math.Floor(position.X / Size);
            int gy = (int)Math.Floor(position.Z / Size);
            Debug.Assert(gx.InRange(-BaseMap.GridBreadth, BaseMap.GridBreadth) && gy.InRange(-BaseMap.GridBreadth, BaseMap.GridBreadth));

            return new Vector2G(gx, gy);
        }

        public WorldGrid(Vector2G coordinates)
        {
            gridCoordinates = coordinates;
            for (int x = 0; x < CellBreadth; x++)
                for (int y = 0; y < CellBreadth; y++)
                    cells.Add(new Vector2G(x, y), new WorldCell());
        }

        public void AddActor(Actor actor)
        {
            if (!cells.TryGetValue(WorldCell.GetCoord(gridCoordinates, actor.Position.Offset), out WorldCell cell))
                return;

            cell.AddActor(actor);
        }

        public void RemoveActor(Actor actor)
        {
            if (!cells.TryGetValue(WorldCell.GetCoord(gridCoordinates, actor.Position.Offset), out WorldCell cell))
                return;

            cell.RemoveActor(actor);
        }

        public void RelocateActor(Actor actor, Vector3 newPosition)
        {
            Vector2G curCellCoordinates = WorldCell.GetCoord(gridCoordinates, actor.Position.Offset);
            Vector2G newCellCoordinates = WorldCell.GetCoord(gridCoordinates, newPosition);

            if (curCellCoordinates != newCellCoordinates)
            {
                if (!cells.TryGetValue(curCellCoordinates, out WorldCell curCell))
                    return;
                
                if (!cells.TryGetValue(newCellCoordinates, out WorldCell newCell))
                    return;

                curCell.RemoveActor(actor);
                newCell.AddActor(actor);
            }
        }

        public void Serach(SearchCheck check, List<Vector2G> tranversedCells, List<Actor> intersectedActors)
        {
            foreach (Vector2G cellCoord in tranversedCells)
                if (cells.TryGetValue(cellCoord, out WorldCell cell))
                    cell.Search(check, intersectedActors);
        }

        public void Update(double lastTick)
        {
            foreach (KeyValuePair<Vector2G, WorldCell> pair in cells)
                pair.Value.Update(lastTick);
        }
    }
}
