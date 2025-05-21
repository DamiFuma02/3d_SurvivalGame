# 3D_SurvivalGame

[YT Playlist 3d Survival Game Tutorial](https://www.youtube.com/playlist?list=PLtLToKUhgzwnk4U2eQYridNnObc2gqWo-)

## 1. Player Movements

> Define a Player empty gameObject with two children: player 3d shape (with Character Controller component) and Camera in order to achieve the first person camera POV

* Keyboard movements: move along X and Z axis reading the input from the keyboard horizontal and vertical axis
* Mouse movements: rotate along X and Y axis to chage the POV of the first person camera

> There are two ways to lock the camera and the player
* lockCameraPosition=true: the camera and the player shape are attached and move and rotate together with the same local coordinates referencr
* lockCameraPosition=false: the camera and the player shape are detached, so the rotation is on the camera object and the movements on the player parent. They have different local coordinates. Use only when the player doesn't need a default looking direction.


## 2. Terrain Texture 

> Install and import the assets specified in the video tutorial in the current unity project.

> Using the icons in the Unity inspector UI for the Terrain Object 
* "Paint Terrain" icon
    * "Raise or lower Terrain": used for deform the flat terrain adding mountains and valleys 
    * "Paint texture": use to import a texture to apply to the terrain
* "Paint Details" icon
    * Add new Detail using the prefabs from the imported assets
    * Using the brush you can place or delete multiple prefabs together, changing the density for each prefab.
    * Edit a single detail prefab using "align to the ground" 
* "Paint Trees" icon:
    * Same as details
    * Use "Provides Contact" tag inside Tree Collider to 


## 3. Raycast and simple AI

> Install and import the rabbits assets in the current unity project

* Create a UI > Canvas with inside
    * UI > Image with an icon from the assets
    * UI > TextMeshPro to show the name of the object looked
* Create a custom class "InteractableObject.cs" with some properties and add it as a component inside the prefab desired to look at
* Create a prefab with the rabbit inside the "Prefabs" folder
    * Add RigidBody and BoxCollider
    * Inside the animator select the "DemoAnimator" inside Rabbits/Demo/Animator/ folder 
        * Assign it to the animator component of the rabbit prefab.
        * Change the conditions of state change inside the animator
    * Create a script "NPC_Movements.cs" and add it as a component of the prefab
    * Optional: add also "InteractableObject.cs" to make the rabbit interactable, and then assign a name

## 4. Picking up Items

> Use a prefab to define the items to pick up

* Create a second Box Collider with "isTrigger=true" per usare OnTrigger per capire se il player è vicino all'oggetto
* Edit > Project Settings > Phyisics > Query Trigger disabled in order to raycast only the first smaller default Box Collider in the prefab


## 5. Inventory System UI

> [GUI Parts Asset Store](https://assetstore.unity.com/packages/2d/gui/icons/gui-parts-159068)

* Inside the UI Canvas add a new UI > Image with the inventory icon
    *  Create an UI > Image with the slot icon, and duplicate it. Attach the ItemSlot script
* Create a prefab with the inventory icon and for each prefab assign it DragDrop script
* Create InventorySystem empty object
    * Attach InventorySystem SingleTon object
    * Handle mouse movements when inventory open or closed pressing "E" key
    * Optional (Not Implemented): handle keyboard movements when inventory open


## 6. Inventory System Summary

[Resource Icons UI Asset](https://assetstore.unity.com/packages/p/resource-icons-101998)

[Low Poly Simple Pack Asset](https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153)

> Pickup an item and add it to the inventory if there is enough space

* For each 3d prefab to pick up assing InteractableObject with pickable=true and a meaningful name 
* For each pickable object create an UI > Image inside the inventory slot and use as image a sprite from the UI asset imported in the project
    * Assign DragDrop script for each 2d prefab and save it inside Resources folder with a filename equal to the 3d prefab itemName
    * Optional (not implemented): use directories to create a simple UI > Image and assign it the image red from the folders
* Fixed Mouse Movements: Mouse Y (UP DOWN) rotates only the camera not the player
* Inventory System: create empty gameobject with assigned the script
    * create a List< GameObject > slotList and populate it reading only the slots with "itemSlot" tag
    * CheckFull() scanning the list and returning false if at least one slot is empty
    * FindFirstEmptySlot() scanning the list and returning the first empty slot
    * When pcking up a 3d prefab it must be destroyed and add it to the inventory
        * Destroy the 3d prefab 
        * AddToInventory(itemName): Instantiate a 2d prefab UI icon for the inventory and add it in the first empty slot found by the responsable
* Inside SelectionManager Singleton class create a field public GameObject selectedObject; to save the current 3d prefab 
    * Inside the InteractableObject.Update() add the condition SelectionManager.instance.selectedObject == gameObject in order to check if the item is the right one
    * This prevents the game to pickup multiple oblject very close to each other when the trigger boxcollider overlaps between different gameObjects



        