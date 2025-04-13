using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using StardewModdingAPI;
using System.Runtime.CompilerServices;
using StardewValley.Mods;
using Microsoft.Xna.Framework;
using static StardewValley.Menus.ShopMenu;

namespace CustomFurnitureCatalogues
{
    public class CustomCatalogue
    {
        private static IMonitor modMonitor;
        private static IModHelper modHelper;

        public static void init(IMonitor monitor, IModHelper helper)
        {
            modMonitor = monitor;
            modHelper = helper;
        }

        private static ShopMenu? ShopMenu;
        private static int tabid = 100005;
        public static void cataloguemenu(catalogueinfo catalogueinfo)
        {
            if (Utility.TryOpenShopMenu(catalogueinfo.ShopID, Game1.player.currentLocation))
            {
                //Utility.TryOpenShopMenu(catalogueinfo.ShopID, Game1.player.currentLocation);
                ShopMenu = Game1.activeClickableMenu as ShopMenu;
                if (ShopMenu == null) return;

                //handle UseExistingTabs
                useexistingtabs(catalogueinfo.UseExistingTabs);

                //handle TabTexture and Tabs
                if (catalogueinfo.Tabs != null && catalogueinfo.Tabs.Count > 0)
                {
                    addcustomtabs(catalogueinfo.TabTexture, catalogueinfo.Tabs);
                }
            }
            else
            {
                modMonitor.Log($"Shop (ID: {catalogueinfo.ShopID}) cannot be opened.", LogLevel.Warn);
            }
        }

        public static void dressermenu(ShopMenu dressermenu, dresserinfo dresserinfo)
        {
            ShopMenu = dressermenu;

            if (dresserinfo.AcceptedItemCategories != null)
            {
                ShopMenu.categoriesToSellHere = dresserinfo.AcceptedItemCategories;
            }
            useexistingtabs(dresserinfo.UseExistingTabs);
            if (dresserinfo.Tabs != null && dresserinfo.Tabs.Count > 0)
            {
                addcustomtabs(dresserinfo.TabTexture, dresserinfo.Tabs);
            }
        }

        private static void useexistingtabs(string? UseExistingTabs)
        {
            if (UseExistingTabs == null)
            {
                ShopMenu.UseNoTabs();
            }
            else
            {
                switch (UseExistingTabs.ToLower())
                {
                    case "furniture":
                    case "furniturecatalogue":
                        ShopMenu.UseFurnitureCatalogueTabs();
                        break;
                    case "wallpaper":
                    case "wallpapercatalogue":
                    case "flooring":
                    case "flooringcatalogue":
                        ShopMenu.UseCatalogueTabs();
                        break;
                    case "dresser":
                        ShopMenu.UseDresserTabs();
                        break;
                    case "basic":
                    case "default":
                        ShopMenu.UseNoTabs();
                        ShopMenu.tabButtons.Add(new ShopTabClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(96, 48, 16, 16), 4f)
                        {
                            myID = 99999,
                            upNeighborID = -99998,
                            downNeighborID = -99998,
                            rightNeighborID = 3546,
                            Filter = (ISalable _) => true
                        });
                        break;
                    default:
                        modMonitor.LogOnce($"UseExistingTabs value ({UseExistingTabs}) not recognized. No automatic tabs will be used for this shop.", LogLevel.Warn);
                        break;
                }
                //snapping to tabs
                foreach (ClickableComponent forSaleButton in ShopMenu.forSaleButtons)
                {
                    forSaleButton.leftNeighborID = -99998;
                }
                ShopMenu.populateClickableComponentList();
            }

        }

        private static void addcustomtabs(string? TabTexture, Dictionary<string, tabdata> Tabs)
        {
            var alltabtext = Game1.content.Load<Texture2D>(TabTexture);
            foreach (var tab in Tabs)
            {
                var newtab = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64, 64), tab.Value.TabTexture == null ? alltabtext : Game1.content.Load<Texture2D>(tab.Value.TabTexture), new Rectangle(tab.Value.SourceX, tab.Value.SourceY, 16, 16), 4f)
                {
                    myID = tabid++,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 3546,
                };
                if (tab.Value.Filters != null)
                {
                    applyfiltersyay(newtab, tab.Value);
                }
                if (ShopMenu.tabButtons != null)
                    ShopMenu.tabButtons.Add(newtab);
            }
            ShopMenu.repositionTabs();
            ShopMenu.populateClickableComponentList();
        }


        private static void applyfiltersyay(ShopTabClickableTextureComponent tab, tabdata td)
        {
            List<int> furnituretypesinfilter = new List<int>();
            if (td.Filters.FurnitureCategories.Any())
            {
                foreach (var type in td.Filters.FurnitureCategories)
                {
                    furnituretypesinfilter.Add(Furniture.getTypeNumberFromName(type));
                }
            }
            if (td.MatchAllFilters is true)
            {
                tab.Filter = (ISalable item) => item is StardewValley.Object obj && (

                (!td.Filters.Categories.Any() || td.Filters.Categories.Contains(obj.Category))
                &&
                (!td.Filters.FurnitureCategories.Any() || obj is Furniture f && furnituretypesinfilter.Contains(f.furniture_type.Value))
                &&
                (!td.Filters.ContextTags.Any() || td.Filters.ContextTags.Any(x => obj.GetContextTags().Any(y => y == x)))
                &&
                (!td.Filters.NameIncludes.Any() || td.Filters.NameIncludes.Any(x => obj.BaseName.Contains(x)))
                &&
                (!td.Filters.DisplayNameIncludes.Any() || td.Filters.DisplayNameIncludes.Any(x => obj.BaseName.Contains(x)))
                &&
                (td.Filters.IncludeShirts == false || (td.Filters.IncludeShirts == true && (item is Clothing clothing && clothing.clothesType.Value == Clothing.ClothesType.SHIRT)))
                &&
                (td.Filters.IncludePants == false || (td.Filters.IncludePants == true && (item is Clothing clothing2 && clothing2.clothesType.Value == Clothing.ClothesType.PANTS)))
                &&
                (td.Filters.IncludeWallpaper == false || (td.Filters.IncludeWallpaper == true && (item is Wallpaper wp && !wp.isFloor.Value)))
                &&
                (td.Filters.IncludeFlooring == false || (td.Filters.IncludeFlooring == true && (item is Wallpaper wp2 && wp2.isFloor.Value)))
                );
            }
            else
            {
                tab.Filter = (ISalable item) => item is StardewValley.Object obj && (

                (td.Filters.Categories.Contains(obj.Category))
                ||
                (obj is Furniture f && furnituretypesinfilter.Contains(f.furniture_type.Value))
                ||
                (td.Filters.ContextTags.Any(x => obj.GetContextTags().Any(y => y == x)))
                ||
                (td.Filters.NameIncludes.Any(x => obj.BaseName.Contains(x)))
                ||
                (td.Filters.DisplayNameIncludes.Any(x => obj.BaseName.Contains(x)))
                ||
                (td.Filters.IncludeShirts == true && item is Clothing clothing && clothing.clothesType.Value == Clothing.ClothesType.SHIRT)
                ||
                (td.Filters.IncludePants == true && item is Clothing clothing2 && clothing2.clothesType.Value == Clothing.ClothesType.PANTS)
                ||
                (td.Filters.IncludeWallpaper == true && item is Wallpaper wp && !wp.isFloor.Value)
                ||
                (td.Filters.IncludeFlooring == true && item is Wallpaper wp2 && wp2.isFloor.Value)
                );
            }
        }
    }
}
