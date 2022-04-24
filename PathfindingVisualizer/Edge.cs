using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class Edge<T>
    {
        public Vertex<T> Start { get; set; }
        public Vertex<T> End { get; set; }
        public float Weight { get; set; }

        public bool isDiagonal;

        public Edge(Vertex<T> startingPoint, Vertex<T> endingPoint, float distance, bool isdiagonal)
        {
            Start = startingPoint;
            End = endingPoint;

            Weight = distance;
            isDiagonal = isdiagonal;
        }
    }
}
