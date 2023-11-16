# Project Attack of the Sentient Breakfast Foods

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Autumn Conway
-   Section: 2

## Simulation Design

In this simulation, waffles will flee from the evil pancakes, who are constantly chasing them and trying to devour them. Both waffles and pancakes will avoid syrup, which can trap them.

### Controls

- The player can left-click to place a puddle of syrup. (A stretch goal would be to add functionality so that the longer the player holds down the mouse for, the bigger the puddle of syrup becomes.) The player can use this strategically to trap the evil pancakes and keep them from getting to the waffles, or, if they're feeling mean, they can use it to trap the waffles instead.

## Waffle

The waffle will wander around the screen randomly and avoid the evil pancakes at all costs.

### Wander

**Objective:** The waffle will aimlessly explore the play area at random.

#### Steering Behaviors

- Wander: The waffle will randomly wander around the play area with a wander algorithm.
- Obstacles:
    - The waffle will avoid syrup in this state.
- Seperation
    - The waffle will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there are no pancakes within a certain range of it.
   
### Flee

**Objective:** The waffle wants to avoid getting eaten by a pancake.

#### Steering Behaviors

- Flee: The waffle will run from nearby pancakes using flee algorithms, weighted by the distance between it and the pancakes (very similar to Separate).
    - Input: All pancakes within a certain range
- Obstacles
    - The waffle will avoid syrup in this state.
- Seperation
    - The waffle will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there is a pancake within a certain radius of it.

## Pancake

The pancake roves the play area and will chase after any waffles it encounters.

### Patrol

**Objective:** The pancake will move back and forth across the screen, searching for waffles.

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles
    - Pancakes will avoid syrup in this state.
- Seperation
    - Pancakes will separate from other pancakes.
   
#### State Transistions

- If the pancake has eaten/collided with a waffle, it will transition to this state for a short period of time before checking for nearby waffles again
- It will also transition to this state if there are no waffles within a certain radius of it
   
### Seek

**Objective:** The pancake will try to catch up to a waffle and collide with it.

#### Steering Behaviors

- Seek: The pancake will use Seek to go after the waffle closest to it.
    - Input: The nearest waffle. 
- Obstacles
    - Pancakes will avoid syrup in this state.
- Seperation
    - The pancake will separate from other pancakes.   

#### State Transistions

- It will transition to this state if there is a waffle within a certan radius of it

## Sources

-   Waffle art asset by me
-   Pancake art asset by me

## Make it Your Own

- I will be making my own art assets.
- _List out what you added to your game to make it different for you_
- _If you will add more agents or states make sure to list here and add it to the documention above_
- _If you will add your own assets make sure to list it here and add it to the Sources section

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

