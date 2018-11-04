# Game
- Remove player character
- Make goal
- Wave spawning mechanic
- Change game terrain to something simpler
- Wall placement system
    - Clickable UI
    - Determine feasibility of ghost/pre-visualization
    - Prevent wall placement inside units
    - Have walls take damage/despawn 
- Display health bars
- Directed camera
    - Zoom in and out
    - Pan with screen edges
- Make defenses (things that do damage)

# Genome
- Health/damage/speed genome
- Fitness function
- Mutation
- Competition
- Inheritance?

# Zombie object created
 - Enemy
   - "Genome"
   - Class function that given genome and parameters gives back the direction vector
   - 

# Without presentation
 - Remove default pathfinding
 - Change wave spawning so infinite waves
 - Attack things that they're looking at
 - Stupid genome thats just an single direction
 - Evolve stupid genome
   - Write mechanism for keeping track of fitness
   - Find fitness function
   - Culling and reproduction
   - Integrate with waves
 - Nearby object detection
   - Add "dectectable" collider to all game objects
   - Write findAllCollisons function
 - Look into neural net library that patrick dug up
 - Work on integrating neural net input -> direction function

Pathfinding sans neural nets

# Neuro-evolution
- TODO