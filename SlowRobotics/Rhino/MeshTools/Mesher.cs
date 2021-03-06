﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRobotics.Rhino.MeshTools
{
    /// <summary>
    /// Utilities for fast mesh creation
    /// </summary>
    public static class Mesher
    {

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh weldVertices(Mesh mesh)
        {
            return mesh;
        }

        /// <summary>
        /// Creates a mesh pipe by lofting between polygon profiles oriented to points on a curve.
        /// </summary>
        /// <param name="curve">Curve to pipe</param>
        /// <param name="numSections">Number of polygon profiles to create</param>
        /// <param name="radius">Radius of polygon profile</param>
        /// <param name="numSides">Number of sides of polygon profile</param>
        /// <returns></returns>
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

            return buildClosedMeshFromPolylineSections(sections, true , true, true);
        }

        /// <summary>
        /// Creates a mesh by lofting between a list of sections
        /// </summary>
        /// <param name="sections">List of polyline sections to loft between</param>
        /// <param name="capStart">Fills the first polyline</param>
        /// <param name="capEnd">Fills the last polyline</param>
        /// <returns></returns>
        public static Mesh buildClosedMeshFromPolylineSections(List<Polyline> sections, bool capStart, bool capEnd, bool closeLoft)
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
                if (closeLoft && (i % (numVertsPerPoly) == numVertsPerPoly - 1))
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

        /// <summary>
        /// Convenience method that builds a mesh from a collection of MFace objects.
        /// </summary>
        /// <param name="faces">Faces in mesh</param>
        /// <returns></returns>
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
