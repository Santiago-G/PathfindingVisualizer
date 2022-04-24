using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class Button
    {
        public Texture2D Image;
        public Vector2 Position;
        public Color Tint;

        bool isPressed = false;

        public Rectangle Hitbox { get => new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height); set { }}

        public Button(Texture2D image, Vector2 pos, Color tint)
        {
            Image = image;
            Position = pos;
            Tint = tint;
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
                isPressed = false;
            }

            return false;
        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();

            IsClicked(ms);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Image, Position, Tint);
        }
    }
}
