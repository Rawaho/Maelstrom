using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using SharpNav.Geometry;

using Vector3 = System.Numerics.Vector3;
using SharpVector3 = SharpNav.Geometry.Vector3;

namespace MapGenerator.File
{
    public class PcbFile
    {
        public class Node
        {
            public struct Header
            {
                public const byte HeaderSize = 40;

                public uint Type { get; }
                public int Size { get; } // only set if node isn't a leaf

                public Vector3 Min { get; }
                public Vector3 Max { get; }

                public ushort VertexPCount { get; }
                public ushort IndexCount { get; }
                public uint VertexCount { get; }

                public Header(BinaryReader reader)
                {
                    Type         = reader.ReadUInt32();
                    Size         = reader.ReadInt32();
                    Min          = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Max          = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    VertexPCount = reader.ReadUInt16();
                    IndexCount   = reader.ReadUInt16();
                    VertexCount  = reader.ReadUInt32();
                }
            }

            public Header NodeHeader { get; }

            public List<Vector3> Verticies { get; } = new List<Vector3>();
            public List<Vector3> PackedVerticies { get; } = new List<Vector3>();
            public List<(byte X, byte Y, byte Z)> Indices { get; } = new List<(byte X, byte Y, byte Z)>();

            public Node(BinaryReader reader)
            {
                NodeHeader = new Header(reader);
            }

            private void ReadData(BinaryReader reader)
            {
                for (int i = 0; i < NodeHeader.VertexCount; i++)
                    Verticies.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));

                var origin = new Vector3(NodeHeader.Max.X - NodeHeader.Min.X, NodeHeader.Max.Y - NodeHeader.Min.Y, NodeHeader.Max.Z - NodeHeader.Min.Z);
                for (int i = 0; i < NodeHeader.VertexPCount; i++)
                {
                    PackedVerticies.Add(new Vector3(
                        ((float)reader.ReadUInt16() / 0xFFFF) * origin.X + NodeHeader.Min.X,
                        ((float)reader.ReadUInt16() / 0xFFFF) * origin.Y + NodeHeader.Min.Y,
                        ((float)reader.ReadUInt16() / 0xFFFF) * origin.Z + NodeHeader.Min.Z));
                }

                for (int i = 0; i < NodeHeader.IndexCount; i++)
                {
                    Indices.Add((reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));

                    // unknown data, research this
                    reader.BaseStream.Position += 9L;
                }
            }

            /// <summary>
            /// Read a single PCB node branch recursively returning the leaves.
            /// </summary>
            public static void ReadBranch(BinaryReader reader, List<Node> leaves)
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var node = new Node(reader);

                    int nodeSize;
                    int skipSize = 0;
                    if (node.NodeHeader.Type == 0x30)
                        nodeSize = node.NodeHeader.Size - Header.HeaderSize;
                    else
                    {
                        nodeSize = (int)(node.NodeHeader.VertexCount * 12 + node.NodeHeader.VertexPCount * 6 + node.NodeHeader.IndexCount * 12);

                        // nodes are aligned to 16 bytes
                        int remainder = nodeSize % 16;
                        if (remainder > 0)
                            skipSize += 16 - remainder;

                        // nodes are padded with 8 bytes
                        skipSize += sizeof(ulong);
                    }

                    using (MemoryStream nodeStream = new MemoryStream(reader.ReadBytes(nodeSize)))
                    {
                        using (BinaryReader nodeReader = new BinaryReader(nodeStream))
                        {
                            if (node.NodeHeader.Type == 0x30)
                            {
                                nodeStream.Position += 8L;
                                ReadBranch(nodeReader, leaves);
                            }
                            else
                            {
                                node.ReadData(nodeReader);
                                reader.BaseStream.Position += skipSize;

                                leaves.Add(node);
                            }
                        }
                    }
                }
            }
        }

        private readonly List<Node> leaves = new List<Node>();

        public PcbFile(SaintCoinach.IO.File pcbFile)
        {
            using (MemoryStream stream = new MemoryStream(pcbFile.GetData()))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // skip header, we don't use anything from it
                    stream.Position += 24L;

                    while (stream.Position < stream.Length)
                        Node.ReadBranch(reader, leaves);
                }
            }
        }

        /// <summary>
        /// Write mesh information in Wavefront obj format.
        /// </summary>
        public void WriteMesh(StringBuilder sb)
        {
            foreach (Node node in leaves)
            {
                node.PackedVerticies.Reverse();
                foreach (Vector3 vertex in node.PackedVerticies)
                    sb.AppendLine($"v {vertex.X} {vertex.Y} {vertex.Z}");

                node.Verticies.Reverse();
                foreach (Vector3 vertex in node.Verticies)
                    sb.AppendLine($"v {vertex.X} {vertex.Y} {vertex.Z}");

                foreach ((byte X, byte Y, byte Z) index in node.Indices)
                    if (index.X != 0 || index.Y != 0 || index.Z != 0)
                        sb.AppendLine($"f -{index.X + 1} -{index.Y + 1} -{index.Z + 1}");
            }
        }

        /// <summary>
        /// Write mesh information in Wavefront obj format.
        /// </summary>
        public void WriteMesh(StringBuilder sb, Matrix4x4 meshMatrix)
        {
            Matrix4x4 TransformVertex(Vector3 vertex)
            {
                return Matrix4x4.CreateTranslation(vertex.X, vertex.Y, vertex.Z) * meshMatrix;
            }

            foreach (Node node in leaves)
            {
                node.PackedVerticies.Reverse();
                foreach (Vector3 vertex in node.PackedVerticies)
                {
                    Matrix4x4 vertexMatrix = TransformVertex(vertex);
                    sb.AppendLine($"v {vertexMatrix.Translation.X} {vertexMatrix.Translation.Y} {vertexMatrix.Translation.Z}");
                }

                node.Verticies.Reverse();
                foreach (Vector3 vertex in node.Verticies)
                {
                    Matrix4x4 vertexMatrix = TransformVertex(vertex);
                    sb.AppendLine($"v {vertexMatrix.Translation.X} {vertexMatrix.Translation.Y} {vertexMatrix.Translation.Z}");
                }

                foreach ((byte X, byte Y, byte Z) index in node.Indices)
                    if (index.X != 0 || index.Y != 0 || index.Z != 0)
                        sb.AppendLine($"f -{index.X + 1} -{index.Y + 1} -{index.Z + 1}");
            }
        }

        /// <summary>
        /// Write mesh information in SharpNav format.
        /// </summary>
        /// <param name="triangles"></param>
        public void WriteMesh(List<Triangle3> triangles)
        {
            foreach (Node node in leaves)
            {
                int[] indices = node.Indices
                    .Select(i => new int[] { i.X, i.Y, i.Z })
                    .SelectMany(d => d).ToArray();

                IEnumerable<SharpVector3> vertices       = node.Verticies.Select(v => new SharpVector3(v.X, v.Y, v.Z));
                IEnumerable<SharpVector3> verticesPacked = node.PackedVerticies.Select(v => new SharpVector3(v.X, v.Y, v.Z));
                triangles.AddRange(TriangleEnumerable.FromIndexedVector3(
                    vertices.Concat(verticesPacked).ToArray(), indices, 0, 1, 0, node.Indices.Count));
            }
        }

        /// <summary>
        /// Write mesh information in SharpNav format.
        /// </summary>
        public void WriteMesh(List<Triangle3> triangles, Matrix4x4 meshMatrix)
        {
            foreach (Node node in leaves)
            {
                int[] indices = node.Indices
                    .Select(i => new int[] { i.X, i.Y, i.Z })
                    .SelectMany(d => d).ToArray();

                SharpVector3 CalculateVertex(Vector3 vertex)
                {
                    Matrix4x4 vertexMatrix = Matrix4x4.CreateTranslation(vertex.X, vertex.Y, vertex.Z) * meshMatrix;
                    return new SharpVector3(vertexMatrix.Translation.X, vertexMatrix.Translation.Y, vertexMatrix.Translation.Z);
                }

                IEnumerable<SharpVector3> vertices       = node.Verticies.Select(CalculateVertex);
                IEnumerable<SharpVector3> verticesPacked = node.PackedVerticies.Select(CalculateVertex);
                triangles.AddRange(TriangleEnumerable.FromIndexedVector3(
                    vertices.Concat(verticesPacked).ToArray(), indices, 0, 1, 0, node.Indices.Count));
            }
        }
    }
}
