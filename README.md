# Custom Catalogues and Dressers
(This is a work in progress; I will be adding and clarifying information for a while.)

This guide assumes you are using and are at least somewhat familiar with [Content Patcher](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md#readme) and modding Stardew Valley in general. I will provide links to the Stardew Valley wiki modding articles where relevant.

### Set Up
[Create the item](https://stardewvalleywiki.com/Modding:Items) that will become the custom catalogue/dresser. Catalogues can technically be anything, but keep in mind what the player can actually interact with as an object in the world (furniture and big craftables are good, but clothing/weapons/tools are pretty much impossible). Dressers _must_ be a dresser furniture item.

### Add the Data
Use `Action: EditData` to add new entries for each of your catalogues and dressers. The TargetFile will be `pheaux.catalogues/catalogues` if you are creating a catalogue, or `pheaux.catalogues/dressers` if you are creating a dresser. The info here is a dictionary, and the key for each entry is the <ins>qualified</ins> item ID for your catalogue/dresser created in Set Up.
| Field Name | Values | Details
| --- | --- | ---
| ShopID | (string) The ID of the shop to open for a catalogue | REQUIRED for catalogues, not used for dressers
| AcceptedItemCategories | (int list) The item categories to accept for a dresser | REQUIRED for dressers; will default to the usual dresser items if not included. Not used for catalogues
| UseExistingTabs | (string) `basic`; `furniture`; `wallpaper`; `dresser` | Tabs from the vanilla game to use here. These tabs are placed before any custom ones. Basic is only the 'see all items' tab that is common to all the options. This can be left empty to only show custom tabs.
| TabTexture | (string) The texture to use for custom tabs | REQUIRED if using custom tabs 
| Tabs | (dictionary) Custom tabs to add | See section below. 

### Tabs
`Tabs` is a dictionary; the key is used for logging in case of an error, but otherwise isn't important to the actual function of the tabs.
| Field Name | Values | Details
| --- | --- | ---
| TabTexture | (string) The texture to use for this tab instead of the TabTexture field above | Optional; mainly intended if you have a minority amount of tabs using a different texture, like those from the vanilla game's files
| SourceX | (int) The X coordinate in the TabTexture for this tab icon | REQUIRED
| SourceY | (int) The Y coordinate in the TabTexture for this tab icon | REQUIRED
| SourceWidth | (int) The width for this tab icon | Default is 16 (same as vanilla's tabs)
| SourceHeight | (int) The height for this tab icon | Default is 16 (same as vanilla's tabs)
| Filters | See Below | Optional if you want this tab to show all items 
| MatchAllFilters | (bool) | Whether or not to have items have to match all the filters or only one to be shown in this tab. Default is false (an item only has to match one filter criteria to show up)

### Filters
| Field Name | Values | Details
| --- | --- | ---
| Categories | (int list) Item Categories | [Wiki page for item categories](https://stardewvalleywiki.com/Modding:Items#Categories)
| FurnitureCategories | (string list) Furniture Categories | The furniture type in the [furniture's data](https://stardewvalleywiki.com/Modding:Furniture)
| ContextTags | (string list) Context Tags |
| NameIncludes | (string list) Phrase that the item's <ins>base</ins> name must include | Case sensitive
| DisplayNameIncludes | (string list) Phrase that the item's <ins>display</ins> name must include | Case sensitive
| IncludePants | (bool) If yes, pants will be shown (if also matching any other criteria) | Default false
| IncludeShirts | (bool) If yes, shirts will be shown (if also matching any other criteria) | Default false
| IncludeWallpaper | (bool) If yes, wallpaper will be shown (if also matching any other criteria) | Default false
| IncludeFlooring | (bool) If yes, flooring will be shown (if also matching any other criteria) | Default false

### Examples
Example of a custom dresser (from my [Tackle Box](https://www.nexusmods.com/stardewvalley/mods/33064) mod):

```
"(F){{ModID}}_tackleboxblue": {
  "AcceptedItemCategories": [ -21, -22 ], // -21 is bait, -22 is tackle
  "TabTexture": "{{MODID}}/sprites", //this was loaded in an earlier block
  "UseExistingTabs": "basic",
  "Tabs": {
    "Bait": {
      "SourceX": 0,
      "SourceY": 0,
      "Filters": {
        "Categories": [ -21 ]
      },
    },
    "Tackle": {
      "SourceX": 16,
      "SourceY": 0,
      "Filters": {
        "Categories": [ -22 ]
      }
    }
  }
}
```
