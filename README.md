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
