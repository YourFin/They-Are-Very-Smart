Tutorial Breakdown
1. Background: Basic use of the Unity development environment
2. The Unity Tower Defense Template tutorial (provide link)
    - We based our game off of the Unity Tower Defense Template found at (LINK) for several reasons:
        - A wealth of prebuilt assets meshed well with our ideas
            - Premade UI and general game interface allowed us to avoid implementing basic systems on our own and gave us an idea of how Unity games can be built
            - Tower and Agents
                - Agents have four main attributes: Max Health, Starting Health, Alignment, and Target Transform. When an agent is instantiated, 
                - Towers will automatically attack Agents with enemy alignment. TowerLevelData allows us to specify what towers do at different levels (unused here; only important from a gameplay standpoint)
                - Alignment, wave management/spawning system (used at least concepts from if not the system itself)
            - Game economy
            - Tower prefabs --> easy to modify to create custom wall "towers"
            - Grid placement system
            - (Negative: navmesh and node-based pathfinding were useless in relation to our vector-based pathfinding, and had to be scrapped)
    - Notable elements of the Action Game Framework
        - Agent, Attacker, Damage Collider, DamageableBehaviour
3. Step-by-step: Creating a WallTower
    - Creating new GameObject
    - Copying components from tower prefab
    - Add WallTower_0 TowerLevel object, attach model (basic cube with dimensions stretched)
    - Adding HealthBar using HealthVisualizer (Script)
    - Saving as prefab
    - Creating UpWall by copying WallTower and modifying dimensions
    - Adding build button to UI
4. Step-by-step: Creating a Zombunny
    - Model, texture, and animations taken from the Action Shooter tutorial
    - Similar process to creating WallTower, but required some custom scripts (detailed in the scripts section, or interwoven here)
    - Colliders: Vision (sphere), Damage (capsule), Zombie Attacker (box)
    - Zombie Agent
5. Custom scripts
    - Genome
        - 
    - GenomeManager
        - 
    - ZombieAgent
        - 
    - ZombieAttacker
        - 