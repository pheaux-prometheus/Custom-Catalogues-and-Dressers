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
| Categories | (int list) The item categories to accept for a dresser | REQUIRED for dressers; will default to the usual dresser items if not included. Not used for catalogues
| UseExistingTabs | (string) `basic`; `furniture`; `wallpaper`; `dresser` | Tabs from the vanilla game to use here. These tabs are placed before any custom ones. Basic is only the 'see all items' tab that is common to all the options. This can be left empty to only show custom tabs.
| TabTexture | (string) The texture to use for custom tabs | REQUIRED if using custom tabs 
| Tabs | (dictionary) Custom tabs to add | See section below. 

### Tabs
