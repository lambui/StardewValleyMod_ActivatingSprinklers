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
            StardewModdingAPI.Events.GameEvents.UpdateTick += UpdateTickEvent;
        }

        static void UpdateTickEvent(object sender, EventArgs e)
        {
            if (StardewValley.Game1.currentLocation == null)
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
            StardewValley.Object check;
            if (currentMouseState.RightButton == ButtonState.Pressed)
                if (Game1.currentLocation.objects.TryGetValue(tile, out check))
                {
                    if (check.name.ToLower().Contains("sprinkler"))
                    {
                        List<Vector2> tileNeedWater = new List<Vector2>();
                        bool setup = false;
                        if (check.name.ToLower().Contains("quality") && setup == false)
                        {
                            tileNeedWater = MakeVector2TileGrid(tile, 1);
                            setup = true;
                        }

                        if (check.name.ToLower().Contains("iridium") && setup == false)
                        {
                            tileNeedWater = MakeVector2TileGrid(tile, 2);
                            setup = true;
                        }

                        if (setup == false)
                        {
                            tileNeedWater.Add(new Vector2(tile.X + 1, tile.Y));
                            tileNeedWater.Add(new Vector2(tile.X - 1, tile.Y));
                            tileNeedWater.Add(new Vector2(tile.X, tile.Y + 1));
                            tileNeedWater.Add(new Vector2(tile.X, tile.Y - 1));
                            setup = true;
                        }

                        WateringCan waterCan = new WateringCan();
                        waterCan.WaterLeft = 100;
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

        static List<Vector2> MakeVector2TileGrid(Vector2 origin, int size)
        {
            List<Vector2> grid = new List<Vector2>();
            for (int i = 0; i < 2 * size + 1; i++)
            {
                for (int j = 0; j < 2 * size + 1; j++)
                {
                    Vector2 newVec = new Vector2(origin.X - size,
                                                origin.Y - size);

                    newVec.X += (float)i;
                    newVec.Y += (float)j;

                    grid.Add(newVec);
                }
            }

            return grid;
        }
    }
}
