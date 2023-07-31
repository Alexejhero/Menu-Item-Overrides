> **Warning**  
> This project is barebones. It was originally meant for personal use but we thought we should release it for everyone.
> Feel free to open an issue or PR if you find an issue or want to contribute, we use this package constantly within our development team so we are actively maintaining it.


# Menu Item Overrides

Don't you just hate it when you install a few assets or packages and  then your menu ends up looking like one of those old Internet Explorer installations with like 20 toolbar extensions?

![](.github/images/header-before.png)

**Well we have just the solution for you!**

With this package, you can turn that 👆 into this 👇!

![](.github/images/header-after.png)

## Installation

1. Open the Package Manager from Window > Package Manager
2. "+" button > Add package from git URL
3. Enter the following
https://github.com/5PDEV/Menu-Item-Overrides.git?path=/Assets/MenuItemOverrides

Or, open Packages/manifest.json and add the following to the dependencies block.

```json
{
    "dependencies": {
      "com.5pdev.menu-item-overrides": "https://github.com/5PDEV/Menu-Item-Overrides.git?path=/Assets/MenuItemOverrides"
    }
}
```

> **Warning**  
> Menu Item Overrides depends on [HarmonyX](https://github.com/BepInEx/HarmonyX), and comes bundled with some precompiled dependencies. If you already have some of these plugins installed in your project and they are conflicting, set the `MENU_ITEM_OVERRIDES_DISABLE_DEPENDENCIES` flag in your editor settings. (you will need to manually install the missing dependencies)

## Configuration

With the package installed, you can open the Menu Item Overrides window by using the `Tools/Menu Item Overrides/Configuration` menu item.

This window allows you to add a list of overrides for menu items, which can modify the path of the menu item, the priority, or both.

![](.github/images/window.png)

- By pressing the `H` button, you can hide the menu item entirely.
- By enabling `Override Path`, you can specify a new location for the menu item.
- By enabling `Override Priority`, you can specify a new priority for the menu item. `+=` indicates that the specified priority is an offset and will be added to the original, and `=` indicates that the original priority will be overriden entirely.
- Paths ending in `/` are expected to be submenus, and will be treated as such (which will be indicated by the lit up `/*` on the right). Since submenus don't have their own priority, but instead are ordered based on their children, `+=` is the only availably priority override strategy for them. 

**Overrides are applied top-to-bottom**, meaning that the following two setups are equivalent:

![](.github/images/example-1.png)
![](.github/images/example-2.png)

### Additional features

- You can use the `Debug Mode` toggle to append the priorities of each item to the end of their path, so you can better see how to modify them. (This is not present in any of the above screenshots because we took them before we added this feature, but it's in the top right)
- By going to `Tools/Menu Item Overrides/See Report...` you can see a list of all of the menu items and their priorities _before_ they were modified by this package.



### Limitations

Only menu items created with the `[MenuItem]` attribute can be modified. built-in menu items cannot be modified 

## Known issues

- When installing or uninstalling the package the assemblies will not be refreshed, so the menu items will not be updated.
- Submenus seem to somehow be cached. This affects the following:
  - Changing the priority of an entire submenu will only take effect if the editor is restarted or if the submenu is moved, or hidden and unhidden.
  - Removing an entire submenu and replacing it with a single item will make the editor behave weirdly, if you really want to do that you can include a ZWSP at the end of the name for the single item

## Getting involved

The project is pretty barebones now, so we welcome any and all contributions, whether it's reporting bugs, suggesting new features, or submitting code improvements.
