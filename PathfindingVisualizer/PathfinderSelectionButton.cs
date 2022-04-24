using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class PathfinderSelectionButton
    {
        public Vector2 Position;
        public Color Tint;
        public bool isSelected = false;
        public string PathfinderType;

        public int bottomY;

        private SpriteFont font;
        private SpriteFont smallerFont;
        private SpriteFont smallestFont;

        private Texture2D titleBar;
        private Texture2D dropDownArrow;
        public Texture2D background;
        private Texture2D circleButton;
        private Texture2D selectedCircleButton;
        private Texture2D squareBox;
        private Texture2D selectedSquareBox;

        private float arrowRotation;
        private bool isPressed = false;

        private Button Manhattan;
        private bool mIsSelected = true;
        private Button Euclidean;
        private bool eIsSelected = false;
        private Button Octile;
        private bool oIsSelected = false;
        private Button Chebyshev;
        private bool cIsSelected = false;

        public string selectedHeuristic = "Manhattan";

        private Button AllowDiagonal;
        public bool diagonalIsAllowed = false;
        //private Button BiDirectional;
        //public bool biDirectionalIsAllowed = false;

        public Rectangle Hitbox { get => new Rectangle((int)Position.X, (int)Position.Y, titleBar.Width, titleBar.Height); set { } }

        public PathfinderSelectionButton(Vector2 pos, Color tint, string pfType)
        {
            Position = pos;
            Tint = tint;
            arrowRotation = 0;
            PathfinderType = pfType;
        }

        public void LoadContent(ContentManager Content)
        {
            titleBar = Content.Load<Texture2D>("titlebar");
            dropDownArrow = Content.Load<Texture2D>("dropDownArrow");
            font = Content.Load<SpriteFont>("Font");
            smallerFont = Content.Load<SpriteFont>("smallerFont");
            smallestFont = Content.Load<SpriteFont>("smallestFont");
            background = Content.Load<Texture2D>("selectionBody");
            circleButton = Content.Load<Texture2D>("circleButton");
            selectedCircleButton = Content.Load<Texture2D>("selectedCircleButton");
            squareBox = Content.Load<Texture2D>("squareButton");
            selectedSquareBox = Content.Load<Texture2D>("selectedSquareBox");

            Manhattan = new Button(circleButton, new Vector2(Position.X + 40, Position.Y + 85), Color.White);
            Euclidean = new Button(circleButton, new Vector2(Position.X + 40, Position.Y + 110), Color.White);
            Octile = new Button(circleButton, new Vector2(Position.X + 40, Position.Y + 135), Color.White);
            Chebyshev = new Button(circleButton, new Vector2(Position.X + 40, Position.Y + 160), Color.White);

            AllowDiagonal = new Button(squareBox, new Vector2(Position.X + 40, Position.Y + 220), Color.White);
            //BiDirectional = new Button(squareBox, new Vector2(Position.X + 40, Position.Y + 245), Color.White);

            bottomY = (int)Position.Y + titleBar.Height;

            if (PathfinderType == "Dijkstra" || PathfinderType == "BreathFirstSearch")
            {
                AllowDiagonal.Position.Y = Position.Y + 87;
                //BiDirectional.Position.Y = Position.Y + 112;
            }
        }

        public bool IsClicked(MouseState ms)
        {
            if ((ms.LeftButton == ButtonState.Pressed && (Hitbox.Contains(ms.Position))) && !isPressed)
            {
                isPressed = true;
                return true;
            }
            else if (ms.LeftButton == ButtonState.Released)
            {
                Game1.MsStates = Game1.MouseStates.none;
                isPressed = false;
            }
            return false;
        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();

            //if (IsClicked(ms))
            //{
            //    isSelected = !isSelected;
            //}

            if (isSelected)
            {
                bottomY = (int)Position.Y + 35 + background.Height;

                arrowRotation = -1.58f;
                Tint = Color.Orange;


                #region Heuristics
                if (Manhattan.IsClicked(ms))
                {
                    mIsSelected = true;
                    selectedHeuristic = "Manhattan";
                }

                if (mIsSelected)
                {
                    Manhattan.Image = selectedCircleButton;
                    eIsSelected = false;
                    oIsSelected = false;
                    cIsSelected = false;
                }
                else
                {
                    Manhattan.Image = circleButton;
                }


                if (Euclidean.IsClicked(ms))
                {
                    selectedHeuristic = "Euclidean";
                    eIsSelected = true;
                }

                if (eIsSelected)
                {
                    Euclidean.Image = selectedCircleButton;
                    mIsSelected = false;
                    oIsSelected = false;
                    cIsSelected = false;
                }
                else
                {
                    Euclidean.Image = circleButton;
                }


                if (Octile.IsClicked(ms))
                {
                    selectedHeuristic = "Octile";
                    oIsSelected = true;
                }

                if (oIsSelected)
                {
                    Octile.Image = selectedCircleButton;
                    mIsSelected = false;
                    eIsSelected = false;
                    cIsSelected = false;
                }
                else 
                {
                    Octile.Image = circleButton;
                }


                if (Chebyshev.IsClicked(ms))
                {
                    selectedHeuristic = "Chebyshev";
                    cIsSelected = true;
                }

                if (cIsSelected)
                {
                    Chebyshev.Image = selectedCircleButton;
                    mIsSelected = false;
                    eIsSelected = false;
                    oIsSelected = false;
                }
                else
                {
                    Chebyshev.Image = circleButton;
                }
                #endregion

                if (AllowDiagonal.IsClicked(ms))
                {
                    diagonalIsAllowed = !diagonalIsAllowed;
                }

                if (diagonalIsAllowed)
                {
                    AllowDiagonal.Image = selectedSquareBox;
                }
                else
                {
                    AllowDiagonal.Image = squareBox;
                }

                //if (BiDirectional.IsClicked(ms))
                //{
                //    biDirectionalIsAllowed = !biDirectionalIsAllowed;
                //}

                //if (biDirectionalIsAllowed)
                //{
                //    BiDirectional.Image = selectedSquareBox;
                //}
                //else
                //{
                //    BiDirectional.Image = squareBox;
                //}
            }
            else
            {
                bottomY = (int)Position.Y + titleBar.Height;
                arrowRotation = 0;
                Tint = Color.Gray;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (isSelected)
            {
                batch.Draw(background, new Vector2(Position.X, Position.Y + 35), Color.White);

                if (PathfinderType == "A*")
                {
                    batch.DrawString(smallerFont, "Heuristic", new Vector2(Position.X + 20, Position.Y + 50), Color.White);
                    Manhattan.Draw(batch);
                    batch.DrawString(smallestFont, "Manhattan", new Vector2(Position.X + 63, Position.Y + 81), Color.White);
                    Euclidean.Draw(batch);
                    batch.DrawString(smallestFont, "Euclidean", new Vector2(Position.X + 63, Position.Y + 106), Color.White);
                    Octile.Draw(batch);
                    batch.DrawString(smallestFont, "Octile", new Vector2(Position.X + 63, Position.Y + 131), Color.White);
                    Chebyshev.Draw(batch);
                    batch.DrawString(smallestFont, "Chebyshev", new Vector2(Position.X + 63, Position.Y + 156), Color.White);

                    batch.DrawString(smallerFont, "Options", new Vector2(Position.X + 20, Position.Y + 185), Color.White);
                    AllowDiagonal.Draw(batch);
                    batch.DrawString(smallestFont, "Allow Diagonal", new Vector2(Position.X + 63, Position.Y + 216), Color.White);
                    //BiDirectional.Draw(batch) 
                }
                if (PathfinderType == "Dijkstra" || PathfinderType == "BreathFirstSearch")
                {
                    batch.DrawString(smallerFont, "Options", new Vector2(Position.X + 20, Position.Y + 50), Color.White);
                    AllowDiagonal.Draw(batch);
                    batch.DrawString(smallestFont, "Allow Diagonal", new Vector2(Position.X + 63, Position.Y + 81), Color.White);
                }

            }

            batch.Draw(titleBar, Position, Color.White);
            batch.Draw(dropDownArrow, new Vector2(Position.X + 24, Position.Y + 20), null, Tint, arrowRotation, origin: new Vector2(dropDownArrow.Width / 2, dropDownArrow.Height / 2), scale: new Vector2(0.8f), SpriteEffects.None, 0);
            batch.DrawString(font, PathfinderType, new Vector2(Position.X + 40, Position.Y + 5), Color.White);


        }
    }
}
