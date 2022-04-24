using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PathfindingVisualizer
{
    public class Tile
    {
        public enum States
        {
            Normal,
            Wall,
            StartPos,
            EndPos
        }

        public Texture2D Image;
        public Vector2 Position;
        public Color Tint;

        public Rectangle Hitbox { get => new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height); set { } }

        public States TileStates = States.Normal;
        private States prevState = States.Normal;

        public Vector2 Cord;

        private bool hasBeenClicked = false;

        public Tile(Texture2D image, Vector2 position, Color tint, Vector2 numb)
        {
            Image = image;
            Position = position;
            Cord = numb;
            Tint = tint;
        }

        public bool IsClicked(MouseState ms)
        {
            //if ((Hitbox.Contains(ms.Position) && ms.LeftButton == ButtonState.Pressed) && !hasBeenClicked)
            if (ms.LeftButton == ButtonState.Pressed && (Hitbox.Contains(ms.Position)))
            {
                hasBeenClicked = true;
                if (Game1.MsStates == Game1.MouseStates.none)
                {
                    switch (TileStates)
                    {
                        case States.Normal:
                            Game1.MsStates = Game1.MouseStates.Build;
                            break;
                        case States.Wall:
                            Game1.MsStates = Game1.MouseStates.Erase;
                            break;
                        case States.StartPos:
                            Game1.MsStates = Game1.MouseStates.MoveStart;
                            break;
                        case States.EndPos:
                            Game1.MsStates = Game1.MouseStates.MoveEnd;
                            break;
                    }
                }
                return true;
            }
            else if (ms.LeftButton == ButtonState.Released)
            {
                hasBeenClicked = false;
                Game1.MsStates = Game1.MouseStates.none;
            }

            return false;
        }
        public void Update()
        {
            MouseState ms = Mouse.GetState();

            //fix mouse thing 
            if (IsClicked(ms) && Game1.pfStatus == Game1.PathfindingStatus.NotStarted)
            {
                switch (Game1.MsStates)
                {
                    case Game1.MouseStates.Build:
                        if (TileStates != States.StartPos && TileStates != States.EndPos)
                        {
                            TileStates = States.Wall;
                            prevState = TileStates;
                        }
                        break;
                    case Game1.MouseStates.Erase:
                        if (TileStates != States.StartPos && TileStates != States.EndPos)
                        {
                            TileStates = States.Normal;
                            prevState = TileStates;
                        }
                        break;
                    case Game1.MouseStates.MoveStart:
                        if (TileStates != States.StartPos)
                        {
                            TileStates = States.StartPos;
                            Game1.startCounter = Cord;
                        }
                        break;
                    case Game1.MouseStates.MoveEnd:
                        if (TileStates != States.EndPos)
                        {
                            TileStates = States.EndPos;
                            Game1.endCounter = Cord;
                        }
                        //do later
                        break;
                }

                //TileStates = TileStates == States.Wall ? States.Normal : States.Wall;
            }

            if (TileStates == States.StartPos && Cord != Game1.startCounter)
            {
                TileStates = prevState;
            }
            if (TileStates == States.EndPos && Cord != Game1.endCounter)
            {
                TileStates = prevState;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (Tint != Color.FromNonPremultiplied(153, 251, 152, 255) && Tint != Color.FromNonPremultiplied(175, 238, 239, 255))
            {
                switch (TileStates)
                {
                    case States.Normal:
                        batch.Draw(Image, Position, Color.White);
                        break;
                    case States.Wall:
                        batch.Draw(Image, Position, Color.Gray);
                        break;
                    case States.StartPos:
                        batch.Draw(Image, Position, Color.Green);
                        break;
                    case States.EndPos:
                        batch.Draw(Image, Position, Color.Red);
                        break;
                }
            }
            else
            {
                batch.Draw(Image, Position, Tint);
            }

        }
    }
}
