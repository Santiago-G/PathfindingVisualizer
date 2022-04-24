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
    public class PathfinderSelectionScreen
    {
        //PathfinderSelectionButton AStar;
        //PathfinderSelectionButton Dijkstra;
        //PathfinderSelectionButton BreathFirstSearch;

        List<PathfinderSelectionButton> Pathfinders = new List<PathfinderSelectionButton>();
        public PathfinderSelectionButton currPathfinder;

        Texture2D startButtonBackground;
        Texture2D buttonBackground;

        public Button ReStartButton;
        public Button PauseSearch;
        public Button ClearWalls;

        SpriteFont smallestFont;
        Color pauseSearchFontTint = Color.Black;
        public void LoadContent(ContentManager Content)
        {
            Pathfinders.Add(new PathfinderSelectionButton(new Vector2(1560, 50), Color.Gray, "A*"));
            Pathfinders.Add(new PathfinderSelectionButton(new Vector2(1560, 92), Color.Gray, "Dijkstra"));
            Pathfinders.Add(new PathfinderSelectionButton(new Vector2(1560, 134), Color.Gray, "BreathFirstSearch"));

            for (int i = 0; i < Pathfinders.Count; i++)
            {
                Pathfinders[i].LoadContent(Content);
            }
            currPathfinder = Pathfinders[0];

            startButtonBackground = Content.Load<Texture2D>("startButtonBackground");
            buttonBackground = Content.Load<Texture2D>("buttonBackground");
            smallestFont = Content.Load<SpriteFont>("smallestFontBold");

            ReStartButton = new Button(buttonBackground, new Vector2(1570, 940), Color.White);
            PauseSearch = new Button(buttonBackground, new Vector2(1645, 940), Color.White);
            ClearWalls = new Button(buttonBackground, new Vector2(1720, 940), Color.White);
        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();

            for (int i = 0; i < Pathfinders.Count; i++)
            {
                Pathfinders[i].Update();

                if (Pathfinders[i].IsClicked(ms))
                {
                    currPathfinder.isSelected = false;
                    currPathfinder = Pathfinders[i];
                }
            }

            currPathfinder.isSelected = true;

            for (int i = 0; i < Pathfinders.Count; i++)
            {
                if (i > 0)
                {
                    Pathfinders[i].Position.Y = Pathfinders[i - 1].bottomY + 5;
                }
            }

            #region bottom 3 buttons
            if (ReStartButton.Hitbox.Contains(ms.Position))
            {
                ReStartButton.Tint = Color.White;
            }
            else 
            {
                ReStartButton.Tint = Color.FromNonPremultiplied(201, 201, 201, 255);
            }

            if (Game1.pfStatus == Game1.PathfindingStatus.Ongoing)
            {
                if (PauseSearch.Hitbox.Contains(ms.Position))
                {
                    PauseSearch.Tint = Color.White;
                }
                else
                {
                    PauseSearch.Tint = Color.FromNonPremultiplied(201, 201, 201, 255);
                }
            }


            if (ClearWalls.Hitbox.Contains(ms.Position))
            {
                ClearWalls.Tint = Color.White;
            }
            else
            {
                ClearWalls.Tint = Color.FromNonPremultiplied(201, 201, 201, 255);
            }

            if (ClearWalls.IsClicked(ms))
            {
                Game1.ClearWalls();
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Pathfinders.Count; i++)
            {
                Pathfinders[i].Draw(spriteBatch);
            }

            spriteBatch.Draw(startButtonBackground, new Vector2(1560, 930), Color.White);
            ReStartButton.Draw(spriteBatch);
            if (Game1.pfStatus == Game1.PathfindingStatus.NotStarted)
            {
                PauseSearch.Tint = Color.Gray;
                pauseSearchFontTint = Color.DarkGray;
                spriteBatch.DrawString(smallestFont, "Start", new Vector2(1577, 946), Color.Black);
            }
            else
            {
                pauseSearchFontTint = Color.Black;
                spriteBatch.DrawString(smallestFont, "Restart", new Vector2(1570, 946), Color.Black);
            }

            PauseSearch.Draw(spriteBatch);
            if (Game1.pfStatus == Game1.PathfindingStatus.Finished)
            {
                spriteBatch.DrawString(smallestFont, "Clear", new Vector2(1650, 936), pauseSearchFontTint);
                spriteBatch.DrawString(smallestFont, "Path", new Vector2(1650, 956), pauseSearchFontTint);

            }
            else 
            {
                spriteBatch.DrawString(smallestFont, "Pause", new Vector2(1650, 946), pauseSearchFontTint);
            }
            ClearWalls.Draw(spriteBatch);
            spriteBatch.DrawString(smallestFont, "Clear", new Vector2(1727, 936), Color.Black);
            spriteBatch.DrawString(smallestFont, "Walls", new Vector2(1726, 956), Color.Black);
        }
    }
}
