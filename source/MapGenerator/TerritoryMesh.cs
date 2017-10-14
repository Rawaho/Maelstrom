using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MapGenerator.File;
using SaintCoinach.Graphics;
using SaintCoinach.Graphics.Lgb;
using SaintCoinach.IO;
using SaintCoinach.Xiv;
using Shared.SqPack;
using SharpNav;
using SharpNav.Geometry;
using SharpNav.IO.Binary;
using SharpNav.Pathfinding;

namespace MapGenerator
{
    public class TerritoryMesh
    {
        private const float TileSize = 160f;

        private readonly TerritoryType territoryType;
        private readonly PackCollection packCollection;

        private NavMeshGenerationSettings settings;
        private TiledNavMesh navigationMesh;
        private readonly List<Triangle3> triangles = new List<Triangle3>();

        public TerritoryMesh(TerritoryType territoryType)
        {
            this.territoryType = territoryType;
            packCollection     = territoryType.Sheet.Collection.PackCollection;
        }

        /// <summary>
        /// Build collision mesh for territory.
        /// </summary>
        public void BuildTerrainCollisionMesh(bool debugMesh)
        {
            StringBuilder sb = null;
            if (debugMesh)
                sb = new StringBuilder();

            string territoryPath          = territoryType.Bg;
            string territoryCollisionPath = $"bg/{territoryPath.Substring(0, territoryPath.IndexOf("/level/", StringComparison.Ordinal) + 1)}collision/";

            if (!packCollection.TryGetFile(territoryCollisionPath + "list.pcb", out SaintCoinach.IO.File listFile))
            {
                Console.WriteLine($"[{territoryType.Name}] Terrain PCB list file is missing!");
                return;
            }

            Console.WriteLine($"[{territoryType.Name}] Parsing terrain PCB files...");

            var parsedPcbListFile = new PcbListFile(listFile);
            foreach (PcbListFile.PcbEntry pcbEntry in parsedPcbListFile.PcbEntries)
            {
                string terrainPcbFilePath = territoryCollisionPath + $"tr{pcbEntry.Id:D4}.pcb";
                string terrainPcbFileName = System.IO.Path.GetFileName(terrainPcbFilePath);

                if (!packCollection.TryGetFile(terrainPcbFilePath, out SaintCoinach.IO.File pcbFile))
                {
                    Console.WriteLine($"[{territoryType.Name}] {terrainPcbFileName} is invalid!");
                    continue;
                }

                Console.WriteLine($"[{territoryType.Name}] Parsing {terrainPcbFileName}...");

                var parsedPcbFile = new PcbFile(pcbFile);
                parsedPcbFile.WriteMesh(triangles);

                if (debugMesh)
                    parsedPcbFile.WriteMesh(sb);
            }

            BuildObjectCollisionMesh(sb);

            if (sb != null)
                System.IO.File.WriteAllText($"{MapGenerator.DebugDirectory}\\{territoryType.Name}.obj", sb.ToString());
        }

        private void BuildObjectCollisionMesh(StringBuilder sb)
        {
            Console.WriteLine($"[{territoryType.Name}] Parsing object PCB files...");

            var territory = new Territory(territoryType);
            foreach (LgbFile lgbFile in territory.LgbFiles)
            {
                foreach (LgbGroup lgbGroup in lgbFile.Groups)
                {
                    foreach (ILgbEntry lgbEntry in lgbGroup.Entries)
                    {
                        if (lgbEntry?.Type != LgbEntryType.Model)
                            continue;

                        LgbModelEntry modelEntry = (LgbModelEntry)lgbEntry;

                        string objectPath          = modelEntry.Model.Model.File.Path;
                        string objectCollisionPath = $"{objectPath.Substring(0, objectPath.IndexOf("/bgparts/", StringComparison.Ordinal) + 1)}collision/";
                        string objectPcbFileName   = System.IO.Path.GetFileName(objectPath);
                        string objectPcbFilePath   = objectCollisionPath + System.IO.Path.ChangeExtension(objectPcbFileName, ".pcb");

                        // not all objects will have a collision mesh
                        if (!packCollection.TryGetFile(objectPcbFilePath, out SaintCoinach.IO.File pcbFile))
                            continue;

                        Console.WriteLine($"[{territoryType.Name}] Parsing {objectPcbFileName}...");

                        Matrix4x4 meshMatrix = Matrix4x4.CreateScale(modelEntry.Model.Scale.X, modelEntry.Model.Scale.Y, modelEntry.Model.Scale.Z)
                            * Matrix4x4.CreateRotationX(modelEntry.Model.Rotation.X)
                            * Matrix4x4.CreateRotationY(modelEntry.Model.Rotation.Y)
                            * Matrix4x4.CreateRotationZ(modelEntry.Model.Rotation.Z)
                            * Matrix4x4.CreateTranslation(modelEntry.Model.Translation.X, modelEntry.Model.Translation.Y, modelEntry.Model.Translation.Z);

                        var parsedPcbFile = new PcbFile(pcbFile);
                        parsedPcbFile.WriteMesh(triangles, meshMatrix);

                        if (sb != null)
                            parsedPcbFile.WriteMesh(sb, meshMatrix);
                    }
                }
            }
        }

        /// <summary>
        /// Build navigation mesh for territory based on collision mesh.
        /// </summary>
        public void BuildNavigationMesh(bool single)
        {
            if (triangles.Count == 0)
                return;

            Console.WriteLine($"[{territoryType.Name}] Generating Navigation Mesh...");

            RecastNavimesh navimeshSettings = GameTableManager.RecastNavimesh.SingleOrDefault(e => e.Name.ToString() == territoryType.Name)
                ?? GameTableManager.RecastNavimesh[0] /*default*/;

            settings = new NavMeshGenerationSettings
            {
                CellSize         = navimeshSettings.CellSize,
                CellHeight       = navimeshSettings.CellHeight,
                MaxClimb         = navimeshSettings.AgentMaxClimb,
                AgentHeight      = navimeshSettings.AgentHeight,
                AgentRadius      = navimeshSettings.AgentRadius,
                MinRegionSize    = (int)navimeshSettings.RegionMinSize,
                MergedRegionSize = (int)navimeshSettings.RegionMergedSize,
                MaxEdgeLength    = (int)navimeshSettings.MaxEdgeLength,
                MaxEdgeError     = navimeshSettings.MaxEdgeError,
                VertsPerPoly     = (int)navimeshSettings.VerticiesPerPolygon,
                SampleDistance   = (int)navimeshSettings.SampleDistance,
                MaxSampleError   = (int)navimeshSettings.MaxEdgeError,
                BuildBoundingVolumeTree = true
            };

            if (single)
                navigationMesh = NavMesh.Generate(triangles, settings);
            else
            {
                BBox3 boundingBox = triangles.GetBoundingBox();
                navigationMesh = new TiledNavMesh(boundingBox.Min, TileSize, TileSize, 1024, 256);

                for (int y = 0; y < Math.Ceiling(Math.Abs(boundingBox.Max.X - boundingBox.Min.X) / TileSize); ++y)
                    for (int x = 0; x < Math.Ceiling(Math.Abs(boundingBox.Max.Z - boundingBox.Min.Z) / TileSize); ++x)
                        BuildNavigationMeshTile(boundingBox, x, y);
            }

            Console.WriteLine($"[{territoryType.Name}] Finished generating Navigation Mesh");
            new NavMeshBinarySerializer().Serialize($"{MapGenerator.NaviMeshDirectory}\\{territoryType.Name}.snb", navigationMesh);
        }

        private void BuildNavigationMeshTile(BBox3 boundingBox, int x, int y)
        {
            // calculate bounding box for tile
            var tileBoundingBox = new BBox3
            {
                Min = new SharpNav.Geometry.Vector3
                {
                    X = boundingBox.Min.X + x * TileSize,
                    Y = boundingBox.Min.Y,
                    Z = boundingBox.Min.Z + y * TileSize
                },
                Max = new SharpNav.Geometry.Vector3
                {
                    X = boundingBox.Min.X + (x + 1) * TileSize,
                    Y = boundingBox.Max.Y,
                    Z = boundingBox.Min.Z + (y + 1) * TileSize
                }
            };

            // increase tile size to capture geometry on boundry
            int borderSize = settings.VoxelAgentRadius + 3;
            tileBoundingBox.Min[0] -= borderSize * settings.CellSize;
            tileBoundingBox.Min[2] -= borderSize * settings.CellSize;
            tileBoundingBox.Max[0] += borderSize * settings.CellSize;
            tileBoundingBox.Max[2] += borderSize * settings.CellSize;

            var heightfield = new Heightfield(tileBoundingBox, settings);
            heightfield.RasterizeTriangles(triangles);
            heightfield.FilterLedgeSpans(settings.VoxelAgentHeight, settings.VoxelMaxClimb);
            heightfield.FilterLowHangingWalkableObstacles(settings.VoxelMaxClimb);
            heightfield.FilterWalkableLowHeightSpans(settings.VoxelAgentHeight);

            var compactHeightfield = new CompactHeightfield(heightfield, settings);
            compactHeightfield.Erode(settings.VoxelAgentRadius);
            compactHeightfield.BuildDistanceField();
            compactHeightfield.BuildRegions(borderSize, settings.MinRegionSize, settings.MergedRegionSize);

            var polyMesh = new PolyMesh(compactHeightfield.BuildContourSet(settings), settings);
            if (polyMesh.VertCount == 0 || polyMesh.PolyCount == 0)
                return;

            var buildData = new NavMeshBuilder(polyMesh, new PolyMeshDetail(polyMesh, compactHeightfield, settings), new OffMeshConnection[0], settings);
            buildData.Header.X = x;
            buildData.Header.Y = y;
            navigationMesh.AddTile(buildData);

            Console.WriteLine($"[{territoryType.Name}] Generated Navigation Mesh tile {x},{y} - {buildData.NavVerts.Length}");
        }
    }
}
