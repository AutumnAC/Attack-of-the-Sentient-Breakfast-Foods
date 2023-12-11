# Project Attack of the Sentient Breakfast Foods

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)


### Student Info

-   Name: Autumn Conway
-   Section: 2

## Simulation Design

In this simulation, waffles will flee from the evil pancakes, who are constantly chasing them and trying to devour them. Both waffles and pancakes will avoid syrup, which will slow them down if they collide with it.

### Controls

- The player can left-click to place a puddle of syrup. The player can use this strategically to block off or slow down the evil pancakes and keep them from getting to the waffles, or, if they're feeling mean, they can use it to trap the waffles instead.

## Waffle

The waffle will wander around the screen randomly and avoid the evil pancakes at all costs, since it will be destroyed if the pancake collides with it.

### Wander

**Objective:** The waffle will aimlessly explore the play area at random.

#### Steering Behaviors

- Wander: The waffle will randomly wander around the play area with the Wander steering behavior.
- Bounds: The waffle will stay in bounds.
- Obstacles:
    - The waffle will avoid syrup in this state.
- Seperation
    - The waffle will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there are no pancakes within a certain range of it.
   
### Flee

**Objective:** The waffle will try to avoid getting eaten by any nearby pancake. The waffle turns cyan when in this state.

#### Steering Behaviors

- Flee: The waffle will run from nearby pancakes by using the Flee steering behavior to avoid each of them, weighted by the distance between it and each pancake (in a way similar to the Separate steering behavior).
    - Input: All pancakes within a certain radius.
- Bounds: The waffle will stay in bounds.
- Obstacles
    - The waffle will avoid syrup in this state.
- Seperation
    - The waffle will separate from other waffles.
   
#### State Transistions

- The waffle will transition to this state if there is a pancake within a certain radius of it.

## Pancake

The pancake roves the play area as a pack with other pancakes and will chase after any waffles it encounters, destroying any it touches.

### Patrol

**Objective:** The pancake will flock with other pancakes and wander across the screen, searching for waffles.

#### Steering Behaviors

- Flock: The pancake will use Flocking to stay in a pack with other pancakes.
- Wander: The pancake will rove around the play area using the Wander steering behavior.
- Bounds: The pancake will stay in bounds.
- Obstacles
    - Pancakes will avoid syrup in this state.
- Seperation
    - Pancakes will separate from other pancakes.
   
#### State Transistions

- If the pancake has eaten/collided with a waffle, it will transition to this state for a short period of time before seeking after any waffles, using a cooldown timer to determine this.
- It will also transition to this state if there are no waffles within a certain radius of it.
   
### Seek

**Objective:** The pancake will try to catch up to a waffle and collide with it. The pancake turns red when in this state.

#### Steering Behaviors

- Seek: The pancake will use Seek to go after the waffle closest to it.
    - Input: The nearest waffle. 
- Bounds: The pancake will stay in bounds.
- Obstacles
    - Pancakes will avoid syrup in this state.
- Seperation
    - The pancake will separate from other pancakes.

#### State Transistions

- It will transition to this state if there is a waffle within a certan radius of it and if its cooldown timer for having just eaten a waffle has finished counting down.

## Sources

-   Waffle art asset by me
-   Pancake art asset by me
-   Syrup art asset by me

## Make it Your Own

- I made my own art assets.

## Known Issues

The balancing isn't quite perfect.

### Requirements not completed

To the best of my knowledge, I've completed all requirements.

