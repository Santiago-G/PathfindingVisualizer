using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using MonoGame.Extended;
using System;

namespace PathfindingVisualizer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public enum MouseStates
        {
            Build,
            Erase,
            MoveStart,
            MoveEnd,
            none
        }

        public enum PathfindingStatus
        {
            NotStarted,
            Ongoing,
            Paused,
            Finished
        }

        static public MouseStates MsStates;
        static public PathfindingStatus pfStatus;

        #region Animation
        List<(Vector2 start, Vector2 end)> linePos = new List<(Vector2, Vector2)>();

        List<Tile> blueTiles = new List<Tile>();

        BinaryHeap<Vertex<int>> tempest = new BinaryHeap<Vertex<int>>(comparerer);

        int animationCounter = 0;

        bool bfs = false;

        TimeSpan animationInterval = TimeSpan.FromMilliseconds(10);
        TimeSpan animationElapsed = TimeSpan.Zero;

        bool finishedAnimating = false;

        #endregion

        static public Vector2 startCounter;
        static public Vector2 endCounter;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D tile;

        Pathfinders<int> pathfinder = new Pathfinders<int>();

        static public Tile[,] Tiles = new Tile[20, 31]; //20, 31

        Graph<int> graphy = new Graph<int>();

        PathfinderSelectionScreen pfSelectionScreen;

        SpriteFont font;

        Vertex<int> endVertex;

        #region Functions
        public void ChangeResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }
        static public void ClearWalls()
        {
            foreach (var tile in Tiles)
            {
                if (tile.TileStates == Tile.States.Wall)
                {
                    tile.TileStates = Tile.States.Normal;
                }
            }
        }
        void buildEdges(bool AllowDiagonal)
        {
            if (AllowDiagonal)
            {
                foreach (var vertex in graphy.Vertices)
                {
                    if (vertex.Cord.Y > 0)
                    {
                        if (vertex.Cord.X > 0)
                        {
                            graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X - 1, vertex.Cord.Y - 1)), true);
                        }

                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X, vertex.Cord.Y - 1)), false);

                        if (vertex.Cord.X < Tiles.GetLength(0))
                        {
                            graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X + 1, vertex.Cord.Y - 1)), true);
                        }
                    }

                    if (vertex.Cord.X < Tiles.GetLength(0))
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X + 1, vertex.Cord.Y)), false);

                        if (vertex.Cord.Y < Tiles.GetLength(1))
                        {
                            graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X + 1, vertex.Cord.Y + 1)), true);
                        }
                    }

                    if (vertex.Cord.Y < Tiles.GetLength(1))
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X, vertex.Cord.Y + 1)), false);

                        if (vertex.Cord.X > 0)
                        {
                            graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X - 1, vertex.Cord.Y + 1)), true);
                        }
                    }

                    if (vertex.Cord.X > 0)
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X - 1, vertex.Cord.Y)), false);
                    }
                }
            }
            else
            {
                foreach (var vertex in graphy.Vertices)
                {
                    if (vertex.Cord.Y > 0)
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X, vertex.Cord.Y - 1)), false);
                    }

                    if (vertex.Cord.X < Tiles.GetLength(0))
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X + 1, vertex.Cord.Y)), false);
                    }

                    if (vertex.Cord.Y < Tiles.GetLength(1))
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X, vertex.Cord.Y + 1)), false);
                    }

                    if (vertex.Cord.X > 0)
                    {
                        graphy.AddEdge(vertex, graphy.Search(new Vector2(vertex.Cord.X - 1, vertex.Cord.Y)), false);
                    }
                }
            }

        }

        static Comparer<Vertex<int>> comparerer = Comparer<Vertex<int>>.Create((Vertex<int> a, Vertex<int> b) =>
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

        int CountOfStartAndEndTiles()
        {
            int c = 0;
            for (int i = 0; i < Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    if (Tiles[i, j].TileStates == Tile.States.StartPos || Tiles[i, j].TileStates == Tile.States.EndPos)
                    {
                        c++;
                    }
                }
            }
            return c;
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            ChangeResolution(1800, 1000);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tile = Content.Load<Texture2D>("pfTile");


            font = Content.Load<SpriteFont>("Font");

            int x = 0;
            int y = 0;

            for (int i = 0; i < Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {

                    Tiles[i, j] = new Tile(tile, new Vector2(x, y), Color.White, new Vector2(i, j));
                    x += 50;
                }

                y += 50;
                x = 0;
            }

            startCounter = new Vector2(0, 0); //9, 10
            endCounter = new Vector2(0, 2); //9, 20

            Tiles[(int)startCounter.X, (int)startCounter.Y].TileStates = Tile.States.StartPos;
            Tiles[(int)endCounter.X, (int)endCounter.Y].TileStates = Tile.States.EndPos;



            pfSelectionScreen = new PathfinderSelectionScreen();
            pfSelectionScreen.LoadContent(Content);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            animationElapsed += gameTime.ElapsedGameTime;
            //greenElapsed += gameTime.ElapsedGameTime;

            MouseState ms = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            // TODO: Add your update logic here

            pfSelectionScreen.Update();

            if (pfStatus == PathfindingStatus.Finished)
            {
                if (!finishedAnimating)
                {
                    if (bfs)
                    {
                        if (animationElapsed >= animationInterval)
                        {
                            if (pathfinder.temp.Count != 0)
                            {
                                Vertex<int> tempy = pathfinder.temp[0];
                                pathfinder.temp.RemoveAt(0);

                                if (!tempy.isWall && (tempy.Cord != startCounter && tempy.Cord != endCounter))
                                {
                                    if (tempy.Visited)
                                    {
                                        Tiles[(int)tempy.Cord.X, (int)tempy.Cord.Y].Tint = Color.FromNonPremultiplied(175, 238, 239, 255);
                                    }
                                    else
                                    {
                                        Tiles[(int)tempy.Cord.X, (int)tempy.Cord.Y].Tint = Color.FromNonPremultiplied(153, 251, 152, 255);
                                    }
                                }
                            }

                            animationCounter++;
                            animationElapsed = TimeSpan.Zero;
                        }

                        if (pathfinder.temp.Count == 0)
                        {
                            finishedAnimating = true;
                            bfs = false;
                        }
                    }
                    else
                    {
                        if (animationElapsed >= animationInterval)
                        {
                            if (tempest.count > 0)
                            {
                                Vertex<int> tempy = tempest.Pop();

                                if (tempy.Visited)
                                {
                                    Tiles[(int)tempy.Cord.X, (int)tempy.Cord.Y].Tint = Color.FromNonPremultiplied(175, 238, 239, 255);
                                }
                                else
                                {
                                    Tiles[(int)tempy.Cord.X, (int)tempy.Cord.Y].Tint = Color.FromNonPremultiplied(153, 251, 152, 255);
                                }
                            }

                            animationCounter++;
                            animationElapsed = TimeSpan.Zero;
                        }

                        if (tempest.count == 0)
                        {
                            finishedAnimating = true;
                        }
                    }
                }
            }

            if (pfSelectionScreen.ReStartButton.IsClicked(ms))
            {
                if (pfStatus == PathfindingStatus.NotStarted)
                {
                    pfStatus = PathfindingStatus.Ongoing;

                    //vertices
                    int i = 0;
                    foreach (var tile in Tiles)
                    {
                        if (tile.TileStates == Tile.States.Wall)
                        {
                            graphy.AddVertex(new Vertex<int>(i, int.MaxValue, new Vector2(tile.Cord.X, tile.Cord.Y), true));
                        }
                        else
                        {
                            graphy.AddVertex(new Vertex<int>(i, int.MaxValue, new Vector2(tile.Cord.X, tile.Cord.Y), false));
                        }

                        i++;
                    }

                    //edges
                    buildEdges(pfSelectionScreen.currPathfinder.diagonalIsAllowed);

                    if (pfSelectionScreen.currPathfinder.PathfinderType == "BreathFirstSearch")
                    {
                        endVertex = pathfinder.BreadthFirstSearch(graphy, graphy.Search(startCounter), graphy.Search(endCounter));
                        bfs = true;
                    }

                    if (pfSelectionScreen.currPathfinder.PathfinderType == "A*")
                    {
                        endVertex = pathfinder.AStar(graphy, graphy.Search(startCounter), graphy.Search(endCounter), pfSelectionScreen.currPathfinder.selectedHeuristic);
                    }
                    else if (pfSelectionScreen.currPathfinder.PathfinderType == "Dijkstra")
                    {
                        endVertex = pathfinder.Dijkstra(graphy, graphy.Search(startCounter), graphy.Search(endCounter));
                    }

                    tempest.Clear();

                    foreach (var vertex in graphy.Vertices)
                    {
                        if (((vertex.inBinaryHeap || vertex.Visited) && !vertex.isWall) && (vertex.Cord != startCounter && vertex.Cord != endCounter))
                        {
                            tempest.Insert(vertex);
                        }
                    }

                    Vertex<int> currentVertex = endVertex;

                    while (currentVertex.Cord != startCounter)
                    {
                        Vector2 end = Tiles[(int)currentVertex.Cord.X, (int)currentVertex.Cord.Y].Position + new Vector2(25);
                        Vector2 start = Tiles[(int)currentVertex.Founder.Cord.X, (int)currentVertex.Founder.Cord.Y].Position + new Vector2(25);
                        ;
                        linePos.Add((start, end));

                        currentVertex = currentVertex.Founder;
                    }

                    pfStatus = PathfindingStatus.Finished;

                    animationCounter = 0;//blueTiles.Count - 1;
                    animationElapsed = TimeSpan.Zero;
                }
                else
                {
                    foreach (var vertex in graphy.Vertices)
                    {
                        if (((vertex.inBinaryHeap || vertex.Visited) && !vertex.isWall) && (vertex.Cord != startCounter && vertex.Cord != endCounter))
                        {
                            Tiles[(int)vertex.Cord.X, (int)vertex.Cord.Y].Tint = Color.White;
                        }
                    }

                    pathfinder.ClearPriorityQueue();

                    graphy.Reset();
                    linePos.Clear();
                    blueTiles.Clear();
                    pathfinder.queue.Clear();

                    pfStatus = PathfindingStatus.NotStarted;
                    finishedAnimating = false;
                }
            }

            foreach (var tile in Tiles)
            {
                Vector2 t = tile.Cord;

                tile.Update();
            }

            base.Update(gameTime);
        }

        Comparer<Vertex<Vector2>> c = Comparer<Vertex<Vector2>>.Create((x, y) => x.DistanceFromStart.CompareTo(y.DistanceFromStart));

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            GraphicsDevice.Clear(Color.FromNonPremultiplied(100, 100, 100, 255));

            foreach (var tile in Tiles)
            {
                tile.Draw(spriteBatch);
            }
            spriteBatch.DrawString(font, "Select Algorithm", new Vector2(1590, 0), Color.White);

            pfSelectionScreen.Draw(spriteBatch);

            if (finishedAnimating)
            {
                for (int i = 0; i < linePos.Count; i++)
                {
                    spriteBatch.DrawLine(linePos[i].start, linePos[i].end, Color.Yellow, 5f);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
