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
        public static Mesh weldVertices(Mesh mesh)
        {
            return mesh;
        }

        public static Mesh pipeCurve(Curve curve, int numSections, float radius, int numSides)
        {
            //generate sections
            List<Polyline> sections = new List<Polyline>();
            foreach(double d in curve.DivideByCount(numSections, true))
            {
                Plane p;
                if (curve.PerpendicularFrameAt(d, out p))
                {
                    Circle circ = new Circle(p, radius);
                    Polyline pts = new Polyline();
                    for (int i = 0;i<numSides;i++)
                    {
                        pts.Add(circ.PointAt((2.0 * Math.PI * ((double)i / numSides))));
                    }
                    pts.Add(pts[0]); //close polyline
                    sections.Add(pts);
                }
            }

            return buildClosedMeshFromPolylineSections(sections, true , true);
        }

        public static Mesh buildClosedMeshFromPolylineSections(List<Polyline> sections, bool capStart, bool capEnd)
        {
            Mesh chunk = new Mesh();
            int numVertsPerPoly = 0;
            int vertexCounter = 0;
            foreach (Polyline p in sections)
            {
                if (p != null)
                {
                    if (numVertsPerPoly == 0)
                    {
                        numVertsPerPoly = (p.IsClosed) ? p.Count - 1 : p.Count; //set num verts
                    }

                    chunk.Vertices.AddVertices(p.Take(numVertsPerPoly)); //add verts

                    if (p == sections.First() && capStart)
                    {
                        foreach (MeshFace f in p.TriangulateClosedPolyline()) chunk.Faces.AddFace(f.Flip());
                    }
                    if (p == sections.Last() && capEnd)
                    {
                        MeshFace[] end = p.TriangulateClosedPolyline();
                        for (int i = 0; i < end.Length; i++)
                        {
                            end[i].A += vertexCounter;
                            end[i].B += vertexCounter;
                            end[i].C += vertexCounter;
                            end[i].D += vertexCounter;
                        }
                        chunk.Faces.AddFaces(end); //cap end
                    }

                    vertexCounter += numVertsPerPoly; //count verts
                }
            }

            for (int i = 0; i <= (numVertsPerPoly * (sections.Count - 1)) - 1; i++)
            {
                if (i % (numVertsPerPoly) == numVertsPerPoly - 1)
                {
                    chunk.Faces.AddFace(i, i - numVertsPerPoly + 1, i + 1, i + numVertsPerPoly);
                }
                else
                {
                    chunk.Faces.AddFace(i, i + 1, i + numVertsPerPoly + 1, i + numVertsPerPoly);
                }

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
