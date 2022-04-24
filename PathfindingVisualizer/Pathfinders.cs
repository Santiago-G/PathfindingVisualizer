using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class Pathfinders<T> where T : IComparable<T>
    {
        Heuristics TheHeuristic = new Heuristics();
        string selectedHeuristic;
        public BinaryHeap<Vertex<T>> PriorityQueue;
        public Queue<Vertex<T>> queue = new Queue<Vertex<T>>();
        Vertex<T> currVertex;

        public List<Vertex<T>> temp = new List<Vertex<T>>();

        Comparer<Vertex<T>> AStarComparer = Comparer<Vertex<T>>.Create((Vertex<T> a, Vertex<T> b) =>
        {
            if (a.FinalDistance > b.FinalDistance)
            {
                return 1;
            }
            else if (a.FinalDistance < b.FinalDistance)
            {
                return -1;
            }
            else
            {
                return 0;
            }

        });

        Comparer<Vertex<T>> DijkstraComparer = Comparer<Vertex<T>>.Create((Vertex<T> a, Vertex<T> b) =>
        {
            if (a.DistanceFromStart > b.DistanceFromStart)
            {
                return 1;
            }
            else if (a.DistanceFromStart < b.DistanceFromStart)
            {
                return -1;
            }
            else
            {
                return 0;
            }

        });

        public Pathfinders()
        {




        }

        public Vertex<T> AStar(Graph<T> graph, Vertex<T> Start, Vertex<T> End, string Heuristic)
        {
            PriorityQueue = new BinaryHeap<Vertex<T>>(AStarComparer);

            foreach (var vertex in graph.Vertices)
            {
                vertex.Visited = false;
                vertex.DistanceFromStart = int.MaxValue;
                vertex.FinalDistance = int.MaxValue;
                vertex.Founder = null;
            }

            selectedHeuristic = Heuristic;
            Start.DistanceFromStart = 0;

            Start.FinalDistance = CurrHeuristic(selectedHeuristic, Start, End);

            PriorityQueue.Insert(Start);

            while (!End.Visited && PriorityQueue.count > 0)
            {
                currVertex = PriorityQueue.Pop();

                if (currVertex.Visited == false)
                {
                    foreach (var neighbor in currVertex.Neighbors)
                    {
                        float tentativeDistance;
                        tentativeDistance = currVertex.DistanceFromStart + neighbor.Weight;

                        if (tentativeDistance < neighbor.End.DistanceFromStart && !neighbor.End.isWall)
                        {
                            neighbor.End.DistanceFromStart = tentativeDistance;
                            neighbor.End.Founder = currVertex;
                            neighbor.End.FinalDistance = tentativeDistance + CurrHeuristic(selectedHeuristic, neighbor.End, End);
                            neighbor.End.Visited = false;
                        }

                        if (!neighbor.End.Visited && !neighbor.End.isWall)
                        {
                            PriorityQueue.Insert(neighbor.End);
                            neighbor.End.inBinaryHeap = true;
                        }
                    }
                }
                currVertex.Visited = true;

                ;
            }

            return End;
        }

        public Vertex<T> Dijkstra(Graph<T> graph, Vertex<T> Start, Vertex<T> End)
        {
            PriorityQueue = new BinaryHeap<Vertex<T>>(DijkstraComparer);

            foreach (var vertex in graph.Vertices)
            {
                vertex.Visited = false;
                vertex.DistanceFromStart = int.MaxValue;
                vertex.Founder = null;
            }

            Start.DistanceFromStart = 0;

            PriorityQueue.Insert(Start);

            while (!End.Visited)
            {
                currVertex = PriorityQueue.Pop();

                if (currVertex.Visited == false)
                {

                    foreach (var Neighbor in currVertex.Neighbors)
                    {
                        float tentativeDistance = currVertex.DistanceFromStart + Neighbor.Weight;

                        if (tentativeDistance < Neighbor.End.DistanceFromStart && !Neighbor.End.isWall)
                        {
                            Neighbor.End.DistanceFromStart = tentativeDistance;
                            Neighbor.End.Founder = currVertex;
                            Neighbor.End.Visited = false;
                        }

                        if (!Neighbor.End.Visited)
                        {
                            PriorityQueue.Insert(Neighbor.End);
                            Neighbor.End.inBinaryHeap = true;
                        }
                    }

                    currVertex.Visited = true;
                }
            }

            return End;
        }

        public Vertex<T> BreadthFirstSearch(Graph<T> graph, Vertex<T> Start, Vertex<T> End)
        {
            queue = new Queue<Vertex<T>>();
            foreach (var vertex in graph.Vertices)
            {
                vertex.Visited = false;
                vertex.DistanceFromStart = int.MaxValue;
                vertex.Founder = null;
            }

            temp.Clear();

            Start.DistanceFromStart = 0;

            queue.Enqueue(Start);

            while (!End.Visited)
            {
                currVertex = queue.Dequeue();
                temp.Add(currVertex);

                if (currVertex.Visited == false)
                {
                    foreach (var Neighbor in currVertex.Neighbors)
                    {
                        if (!Neighbor.End.Visited && !queue.Contains(Neighbor.End) && !Neighbor.End.isWall)
                        {
                            //Neighbor.End.DistanceFromStart = Neighbor.Start.DistanceFromStart + 1;
                            queue.Enqueue(Neighbor.End);
                            Neighbor.End.inBinaryHeap = true;
                            Neighbor.End.Founder = currVertex;
                        }
                    }

                    currVertex.Visited = true;
                }
            }

            return End;
        }

        public void ClearPriorityQueue()
        {
            if (PriorityQueue != null)
            {
                PriorityQueue.Clear();
            }

            ;
        }

        public float CurrHeuristic(string selectedH, Vertex<T> Start, Vertex<T> End)
        {
            if (selectedH == "Manhattan")
            {
                return TheHeuristic.Manhattan(Start, End);
            }
            else if (selectedH == "Euclidean")
            {
                return TheHeuristic.Euclidean(Start, End);
            }
            else if (selectedH == "Octile")
            {
                return TheHeuristic.Octile(Start, End);
            }
            else if (selectedH == "Chebyshev")
            {
                return TheHeuristic.Chebyshev(Start, End);
            }

            return -1;
        }

        public class Heuristics
        {
            //plain boring 4
            public float Manhattan(Vertex<T> node, Vertex<T> goal)
            {
                float dx = Math.Abs(node.Cord.X - goal.Cord.X);
                float dy = Math.Abs(node.Cord.Y - goal.Cord.Y);

                float D = 1;

                return D * (dx + dy);
            }

            //cool 8 directions
            public float Octile(Vertex<T> node, Vertex<T> goal)
            {
                float dx = Math.Abs(node.Cord.X - goal.Cord.X);
                float dy = Math.Abs(node.Cord.Y - goal.Cord.Y);

                double D = int.MaxValue;

                double D2 = int.MaxValue;

                D = 1;
                D2 = Math.Sqrt(2);

                return (float)(D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy));
            }

            //walls? never heard of em'
            public float Euclidean(Vertex<T> node, Vertex<T> goal)
            {
                float dx = Math.Abs(node.Cord.X - goal.Cord.X);
                float dy = Math.Abs(node.Cord.Y - goal.Cord.Y);

                return (float)(1 * Math.Sqrt((dx * dx) + (dy * dy)));
            }

            public float Chebyshev(Vertex<T> node, Vertex<T> goal)
            {
                float dx = Math.Abs(node.Cord.X - goal.Cord.X);
                float dy = Math.Abs(node.Cord.Y - goal.Cord.Y);

                double D = int.MaxValue;

                double D2 = int.MaxValue;

                D = 1;
                D2 = 1;

                return (float)(D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy));
            }
        }
    }
}
