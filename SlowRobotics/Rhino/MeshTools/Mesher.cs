using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRobotics.Rhino.MeshTools
{
    public static class Mesher
    {
        public static Mesh buildMeshFromPolylineSections(List<Polyline> sections)
        {
            Polyline last = null;
            Mesh chunk = new Mesh();
            int c = 0;
            foreach (Polyline p in sections)
            {
                if (last != null)
                {
                    int numVerts = p.Count;
                    for (int i = 0; i < numVerts - 1; i++)
                    {
                        chunk.Vertices.Add(p[i]);
                        chunk.Vertices.Add(p[(i + 1)]);
                        chunk.Vertices.Add(last[(i + 1)]);
                        chunk.Vertices.Add(last[i]);
                        chunk.Faces.AddFace(c, c + 1, c + 2, c + 3);
                        c += 4;
                    }
                }

                last = p;
            }
            return chunk;
        }

        public static Mesh buildMesh(IEnumerable<MFace> faces)
        {
            int i = 0;
            Mesh chunk = new Mesh();
            foreach (MFace f in faces)
            {
                chunk.Vertices.Add(f.v1);
                chunk.Vertices.Add(f.v2);
                chunk.Vertices.Add(f.v3);
                chunk.VertexColors.Add(f.c);
                chunk.VertexColors.Add(f.c);
                chunk.VertexColors.Add(f.c);

                if (f.quad)
                {
                    chunk.Vertices.Add(f.v4);
                    chunk.VertexColors.Add(f.c);
                    chunk.Faces.AddFace(i, i + 1, i + 2, i + 3);
                    i += 4;
                }
                else {
                    chunk.Faces.AddFace(i, i + 1, i + 2);
                    i += 3;
                }
            }

            return chunk;
        }

    }
}
