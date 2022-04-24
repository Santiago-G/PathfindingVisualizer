using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class Graph<T> where T : IComparable<T>
    {
        private List<Vertex<T>> vertices;
        private List<Edge<T>> edges;

        public IReadOnlyList<Vertex<T>> Vertices => vertices;
        public IReadOnlyList<Edge<T>> Edges => edges;

        public int VertexCount => vertices.Count;

        public Graph()
        {
            vertices = new List<Vertex<T>>();
            edges = new List<Edge<T>>();
        }

        public void AddVertex(Vertex<T> vertex)
        {
            if (vertex == null || vertex.NeighborCount > 0)//|| Search(vertex.Value) != null)
            {
                return;
            }

            vertices.Add(vertex);
        }

        public bool RemoveVertex(Vertex<T> vertex)
        {
            if (vertices.Contains(vertex))
            {
                for (int i = 0; i < vertex.NeighborCount; i++)
                {
                    RemoveEdge(vertex, vertex.Neighbors[i].End);
                }

                vertex.Neighbors.Clear();

                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].End == vertex)
                    {
                        RemoveEdge(edges[i].Start, vertex);
                    }
                }

                vertices.Remove(vertex);
                return true;
            }

            return false;
        }

        public bool AddEdge(Vertex<T> a, Vertex<T> b, bool isDiag, float weight = 0)
        {
            if ((a == null || b == null) || !vertices.Contains(a) || !vertices.Contains(b) || GetEdge(a, b) != null)
            {
                return false;
            }

            Edge<T> newEdge;

            if (weight == 0)
            {
                if (b.isWall)
                {
                    newEdge = new Edge<T>(a, b, int.MaxValue, isDiag);
                }
                else
                {
                    newEdge = new Edge<T>(a, b, 1, isDiag);
                }
            }
            else
            {
                newEdge = new Edge<T>(a, b, weight, isDiag);
            }


            a.Neighbors.Add(newEdge);
            edges.Add(newEdge);

            return true;
        }

        public bool RemoveEdge(Vertex<T> a, Vertex<T> b)
        {
            Edge<T> edgeToRemove = GetEdge(a, b);

            if (edgeToRemove != null)
            {
                a.Neighbors.Remove(edgeToRemove);
                edges.Remove(edgeToRemove);

                return true;
            }

            return false;
        }

        public Vertex<T> Search(Vector2 cord)
        {

            foreach (var vertex in vertices)
            {
                if (vertex.Cord == cord)
                {
                    return vertex;
                }
            }



            return null;
        }

        public Edge<T> GetEdge(Vertex<T> a, Vertex<T> b)
        {
            if ((a != null && b != null) && (a.Neighbors.Exists((currNeighbor) => currNeighbor.End == b)))
            {
                return a.Neighbors.First((currNeighbor) => currNeighbor.End == b);
            }

            return null;
        }

        public void Reset()
        {
            vertices.Clear();
            edges.Clear();
        }
    }
}
