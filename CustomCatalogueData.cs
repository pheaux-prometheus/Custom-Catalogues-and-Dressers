using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomFurnitureCatalogues
{
    public sealed class catalogueinfo
    {
        public string? ShopID;
        public string? UseExistingTabs;
        public string TabTexture;
        public Dictionary<string, tabdata>? Tabs;
    }
    public sealed class dresserinfo
    {
        public List<int>? AcceptedItemCategories;
        public string? UseExistingTabs;
        public string TabTexture;
        public Dictionary<string, tabdata>? Tabs;
    }
    public sealed class tabdata
    {
        public string? TabTexture;
        public int SourceX;
        public int SourceY;
        public int SourceWidth { get; set; } = 16;
        public int SourceHeight { get; set; } = 16;
        public filterdata? Filters;
        public bool MatchAllFilters { get; set; } = false; //true uses && logic, false uses || logic
    }
    public sealed class filterdata
    {
        public List<int> Categories { get; set; } = new List<int>();
        public List<string> FurnitureCategories { get; set; } = new List<string>();
        public List<string> ContextTags { get; set; } = new List<string>();
        public List<string> NameIncludes { get; set; } = new List<string>();
        public List<string> DisplayNameIncludes { get; set; } = new List<string>();
        public bool IncludePants { get; set; } = false;
        public bool IncludeShirts { get; set; } = false;
        public bool IncludeWallpaper { get; set; } = false;
        public bool IncludeFlooring { get; set; } = false;
    }



}