# Snake (2D)

A modern take on the classic Snake game, developed in Unity with enhanced gameplay mechanics including obstacles, portals, and dynamic difficulty.

> Snake is the common name for a video game concept where the player maneuvers a line which grows in length, with the line itself being a primary obstacle. The concept originated in the 1976 arcade game Blockade, and the ease of implementing Snake has led to hundreds of versions (some of which have the word snake or worm in the title) for many platforms. After a variant was preloaded on Nokia mobile phones in 1998, there was a resurgence of interest in the snake concept as it found a larger audience.

## Features

- **Classic Snake Gameplay**: Control a growing snake that must eat food to grow longer
- **Smart Food Spawning**: Food intelligently avoids spawning on occupied positions
- **Obstacles**: Dynamic obstacles spawn periodically to increase difficulty
- **Portal System**: Teleport through connected portals that relocate periodically
- **Score Tracking**: Points awarded for each food item consumed
- **Configurable Settings**:
  - Adjustable snake speed and speed multipliers
  - Customizable obstacle spawn intervals
  - Optional wall pass-through mode
  - Toggleable obstacle and portal spawning
- **Grid-Based Movement**: Precise, retro-style movement on a fixed grid

## How to Play

### Controls
- **Arrow Keys** or **WASD**: Change snake direction
  - W/↑: Move Up
  - A/←: Move Left
  - S/↓: Move Down
  - D/→: Move Right

### Gameplay
1. The snake starts at the center of the grid moving right
2. Eat food (colored squares) to grow longer and earn points
3. Avoid running into:
   - Your own body
   - Obstacles (unless you want to relocate them)
   - The walls (unless `moveThroughWalls` is enabled)
4. Use portals to teleport across the map
5. The game gets harder as obstacles spawn over time

## Project Structure

```
Assets/
├── Prefabs/
│   └── SnakeSegment.prefab    # Prefab for snake body segments
├── Scenes/
│   └── Snake.unity            # Main game scene
├── Scripts/
│   ├── Game.cs                # Main game manager
│   ├── Snake.cs               # Snake movement and growth logic
│   ├── Food.cs                # Food spawning and collection
│   ├── Obstacle.cs            # Obstacle behavior
│   └── Portal.cs              # Portal teleportation system
└── Sprites/
    └── Square.png             # Basic square sprite for game entities
```

## Technical Details

### Game Architecture

- **Game Manager Pattern**: Singleton `Game` class manages all game entities and spawning logic
- **Component-Based Design**: Each game entity (Snake, Food, Obstacle, Portal) is a self-contained component
- **Grid-Based System**: All positions are rounded to integers for precise grid alignment
- **Collision Detection**: Uses Unity's 2D BoxCollider system for triggering events

### Key Components

#### Snake.cs
- Grid-based movement with fixed update intervals
- Dynamic segment list that grows when food is consumed
- Input buffering to prevent invalid direction changes
- Configurable speed with multiplier support

#### Game.cs
- Manages score tracking
- Handles food, obstacle, and portal spawning
- Coordinates portal pairing
- Tracks all active game entities

#### Food.cs
- Randomly repositions after being consumed
- Checks for occupied positions to avoid overlap

#### Obstacle.cs
- Spawns at intervals to increase difficulty
- Relocates when touched by the snake

#### Portal.cs
- Works in pairs for teleportation
- Prevents immediate re-teleportation
- Relocates periodically with its pair

## Requirements

- **Unity Version**: Unity 2022.3 or later recommended
- **Platform**: Cross-platform (tested on Windows, macOS, Linux)

## Setup

1. Clone or download this repository
2. Open the project in Unity
3. Open the `Assets/Scenes/Snake.unity` scene
4. Press Play to start the game

## Configuration

You can adjust game parameters in the Unity Inspector when selecting the Game object:

- **Points Per Food**: Score awarded per food item (default: 10)
- **Snake Speed**: Base movement speed
- **Initial Size**: Starting length of the snake
- **Move Through Walls**: Allow snake to wrap around screen edges
- **Spawn Obstacles**: Enable/disable obstacle spawning
- **Obstacle Spawn Interval**: Time between obstacle spawns (seconds)
- **Spawn Portals**: Enable/disable portal spawning
- **Portal Spawn Interval**: Time between portal relocations (seconds)

## Credits

Developed as a university project demonstrating Unity 2D game development principles.