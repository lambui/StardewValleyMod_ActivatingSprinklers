using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace ActivatingSprinklers
{
    public class ActivatingSprinklers : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        static void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null)
                return;

            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();
            DoAction(currentKeyboardState, currentMouseState);
        }
        static void DoAction(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if (Game1.currentLocation == null)
                return;

            Vector2 tile = Game1.currentCursorTile;
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                if (Game1.currentLocation.objects.TryGetValue(tile, out StardewValley.Object check))
                {
                    if (check.name.ToLower().Contains("sprinkler"))
                    {
                        int sprinklerRadius = 0;
                        if (true) //TODO: Find out how to check for pressure nozzle
                        {
                            sprinklerRadius++;
                        }
                        if (check.name.ToLower().Contains("iridium")) sprinklerRadius += 2;
                        else if (check.name.ToLower().Contains("quality")) sprinklerRadius++;
                        List<Vector2> tileNeedWater = MakeVector2TileGrid(tile, sprinklerRadius);
                        WateringCan waterCan = new WateringCan
                        {
                            WaterLeft = 100
                        };
                        float stamina = Game1.player.Stamina;
                        foreach (Vector2 waterTile in tileNeedWater)
                        {
                            waterCan.DoFunction(Game1.currentLocation, (int)(waterTile.X * Game1.tileSize), (int)(waterTile.Y * Game1.tileSize), 1, Game1.player);
                            waterCan.WaterLeft++;
                            Game1.player.Stamina = stamina;
                        }
                    }
                }
            }
        }

        static List<Vector2> MakeVector2TileGrid(Vector2 origin, int size)
        {
            List<Vector2> grid = new List<Vector2>();

            if (size == 0)
            {
                grid.Add(new Vector2(origin.X + 1, origin.Y));
                grid.Add(new Vector2(origin.X - 1, origin.Y));
                grid.Add(new Vector2(origin.X, origin.Y + 1));
                grid.Add(new Vector2(origin.X, origin.Y - 1));
                return grid;
            }

            for (int i = 0; i < 2 * size + 1; i++)
            {
                for (int j = 0; j < 2 * size + 1; j++)
                {
                    Vector2 newVec = new Vector2(origin.X - size,
                                                origin.Y - size);

                    newVec.X += (float)i;
                    newVec.Y += (float)j;
                    if (newVec == origin)
                    {
                        continue;
                    }
                    else
                    {
                        grid.Add(newVec);
                    }
                }
            }

            return grid;
        }
    }
}
