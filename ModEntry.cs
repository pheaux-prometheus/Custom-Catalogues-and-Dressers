using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CustomFurnitureCatalogues
{
    internal class ModEntry : Mod
    {
        private Dictionary<string, catalogueinfo>? cataloguelist = new();
        private Dictionary<string, dresserinfo>? dresserlist = new();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            CustomCatalogue.init(this.Monitor, this.Helper);

            Helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Helper.Events.Content.AssetRequested += loadblank;


            helper.ConsoleCommands.Add("pheaux.catalogues", "Reloads the pheaux.customcatalogues data for custom catalogues and dressers.", this.commandreload);
        }

        //only added if any catalogues are registered
        private void Buttonpressed(object? sender, ButtonPressedEventArgs e)
        {
            //make sure checking object in appropriate context
            if (!Context.CanPlayerMove || !e.Button.IsActionButton()) return;

            //get object, return if none
            var alsdkfj = Game1.player.GetToolLocation();

            var o = Game1.player.currentLocation.getObjectAtTile((int)alsdkfj.X / 64, (int)alsdkfj.Y / 64, true);
            if (o == null) return;

            //if itemid has an entry then handle that elsewhere
            if (cataloguelist.TryGetValue(o.QualifiedItemId, out catalogueinfo? catalogueinfo))
            {
                CustomCatalogue.cataloguemenu(catalogueinfo);
            }
        }

        //only added if any dressers are registered
        private void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu != null && e.NewMenu is ShopMenu sm && sm.source is StorageFurniture sf && dresserlist.TryGetValue(sf.QualifiedItemId, out dresserinfo? dresserinfo))
            {
                CustomCatalogue.dressermenu(sm, dresserinfo);
            }
        }

        //reads data for catalogues/dressers
        private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            cataloguelist = Helper.GameContent.Load<Dictionary<string, catalogueinfo>>("pheaux.customcatalogues/catalogues");
            dresserlist = Helper.GameContent.Load<Dictionary<string, dresserinfo>>("pheaux.customcatalogues/dressers");
            Monitor.Log($"Loading {cataloguelist.Count} catalogues and {dresserlist.Count} dressers.", LogLevel.Trace);

            if (cataloguelist != null && cataloguelist.Count > 0)
            {
                foreach (var catalogue in cataloguelist)
                {
                    if ((catalogue.Value.TabTexture == null || !Game1.content.DoesAssetExist<Texture2D>(catalogue.Value.TabTexture)) && catalogue.Value.Tabs != null && catalogue.Value.Tabs.Count > 0)
                    {
                        Monitor.Log($"TabTexture \"{catalogue.Value.TabTexture}\" for catalogue ID {catalogue.Key} does not exist. Did you remember to Load it?", LogLevel.Warn);
                        catalogue.Value.Tabs.Clear();
                    }
                }
                    Monitor.Log($"ButtonPressed event added for catalogues.", LogLevel.Trace);
                    Helper.Events.Input.ButtonPressed += Buttonpressed;
            }

            if (dresserlist != null && dresserlist.Count > 0)
            {
                foreach (var dresser in dresserlist)
                {
                    if ((dresser.Value.TabTexture == null || !Game1.content.DoesAssetExist<Texture2D>(dresser.Value.TabTexture)) && dresser.Value.Tabs != null && dresser.Value.Tabs.Count > 0)
                    {
                        Monitor.Log($"TabTexture \"{dresser.Value.TabTexture}\" for dresser ID {dresser.Key} does not exist. Did you remember to Load it?", LogLevel.Warn);
                        dresser.Value.Tabs.Clear();
                    }
                }
                    Monitor.Log($"MenuChanged event added for dressers.", LogLevel.Trace);
                    Helper.Events.Display.MenuChanged += Display_MenuChanged;
            }

            Helper.Events.GameLoop.SaveLoaded -= SaveLoaded;
        }

        private void commandreload(string command, string[] args)
        {
            if (args[0].ToLower() == "reload")
            {
                Helper.Events.Display.MenuChanged -= Display_MenuChanged;
                Helper.Events.Input.ButtonPressed -= Buttonpressed;

                Helper.GameContent.InvalidateCache("pheaux.customcatalogues/catalogues");
                Helper.GameContent.InvalidateCache("pheaux.customcatalogues/dressers");
                cataloguelist = Helper.GameContent.Load<Dictionary<string, catalogueinfo>>("pheaux.customcatalogues/catalogues");
                dresserlist = Helper.GameContent.Load<Dictionary<string, dresserinfo>>("pheaux.customcatalogues/dressers");

                if (cataloguelist != null && cataloguelist.Count > 0)
                {
                    foreach (var catalogue in cataloguelist)
                    {
                        if ((catalogue.Value.TabTexture == null || !Game1.content.DoesAssetExist<Texture2D>(catalogue.Value.TabTexture)) && catalogue.Value.Tabs != null && catalogue.Value.Tabs.Count >0)
                        {
                            Monitor.Log($"TabTexture \"{catalogue.Value.TabTexture}\" for catalogue ID {catalogue.Key} does not exist. Custom tabs will not be used. Did you remember to Load the TabTexture?", LogLevel.Warn);
                            catalogue.Value.Tabs.Clear();
                        }
                    }
                        Monitor.Log($"ButtonPressed event added for catalogues.", LogLevel.Trace);
                        Helper.Events.Input.ButtonPressed += Buttonpressed;
                }

                if (dresserlist != null && dresserlist.Count > 0)
                {
                    foreach (var dresser in dresserlist)
                    {
                        if ((dresser.Value.TabTexture == null || !Game1.content.DoesAssetExist<Texture2D>(dresser.Value.TabTexture)) && dresser.Value.Tabs != null && dresser.Value.Tabs.Count > 0)
                        {
                            Monitor.Log($"TabTexture \"{dresser.Value.TabTexture}\" for dresser ID {dresser.Key} does not exist. Did you remember to Load it?", LogLevel.Warn);
                            dresser.Value.Tabs.Clear();
                        }
                    }
                        Monitor.Log($"MenuChanged event added for dressers.", LogLevel.Trace);
                        Helper.Events.Display.MenuChanged += Display_MenuChanged;
                }

                Monitor.Log($"Custom Catalogue and Dresser info has been reloaded: {cataloguelist.Count} catalogues and {dresserlist.Count} dressers.", LogLevel.Info);
            }
            else
            {
                Monitor.Log($"Command not recognized! Current available commands are: \n reload", LogLevel.Info);
            }
        }

        private void loadblank(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("pheaux.customcatalogues/catalogues"))
            {
                Monitor.Log($"Loading pheaux.customcatalogues/catalogues", LogLevel.Trace);
                e.LoadFrom(
                    () =>
                    {
                        return new Dictionary<string, catalogueinfo>
                        {

                        };
                    },
                    AssetLoadPriority.Exclusive
                );
            }
            if (e.NameWithoutLocale.IsEquivalentTo("pheaux.customcatalogues/dressers"))
            {
                Monitor.Log($"Loading pheaux.customcatalogues/catalogues", LogLevel.Trace);
                e.LoadFrom(
                    () =>
                    {
                        return new Dictionary<string, dresserinfo>
                        {

                        };
                    },
                    AssetLoadPriority.Exclusive
                );
            }
        }
    }
}
