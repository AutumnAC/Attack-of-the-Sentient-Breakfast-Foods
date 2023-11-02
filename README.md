# Project Attack of the Sentient Breakfast Foods

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Autumn Conway
-   Section: 2

## Simulation Design

In this simulation, waffles will flee from the evil pancakes, who are constantly chasing them and trying to devour them. Both waffles and pancakes will avoid syrup, which can trap them.

### Controls

- The player can left-click to place a puddle of syrup. (A stretch goal would be to add functionality so that the longer the player holds down the mouse for, the bigger the puddle of syrup becomes.) The player can use this strategically to trap the evil pancakes and keep them from getting to the waffles.
-   _List all of the actions the player can have in your simulation_
    -   _Include how to preform each action ( keyboard, mouse, UI Input )_
    -   _Include what impact an action has in the simulation ( if is could be unclear )_

## Waffle

The waffle will wander randomly and avoid the evil pancakes at all costs.

### Wander

**Objective:** The waffle will slowly wander aimlessly within bounds.

#### Steering Behaviors

- _List all behaviors used by this state_
   - _If behavior has input data list it here_
   - _eg, Flee - nearest Agent2_
- Obstacles:
    - Waffles will avoid syrup in this state.
    - _List all obstacle types this state avoids_
- Seperation
    - Waffles will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there are no pancakes within a certain radius of it.
   
### Flee

**Objective:** The waffle will flee at top speed away from a pancake.

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
    - Waffles will avoid syrup in this state.
- Seperation
    - Waffles will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there is a pancake within a certain radius of it.

## Pancake

The pancake roves the play area and will chase after any waffles it encounters.

### Patrol

**Objective:** The pancake will move back and forth across the screen, searching for waffles.

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
    - Pancakes will avoid syrup in this state.
- Seperation - _List all agents this state seperates from_
    - Pancakes will separate from other pancakes.
   
#### State Transistions

- If the pancake has eaten/collided with a waffle, it will transition to this state
- It will also transition to this state if there are no waffles within a certain radius of it
   
### Seek

**Objective:** The pancake will try to catch up to a waffle and collide with it.

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
    - Pancakes will avoid syrup in this state.
- Seperation - _List all agents this state seperates from_
    - Pancakes will separate from other pancakes.   

#### State Transistions

- It will transition to this state if there is a waffle within a certan radius of it

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

- I will be making my own art assets.
- _List out what you added to your game to make it different for you_
- _If you will add more agents or states make sure to list here and add it to the documention above_
- _If you will add your own assets make sure to list it here and add it to the Sources section

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

