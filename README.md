# Candy Rush

A 2D roguelike survivor game built with Unity 6000.2.5f1, featuring wave-based combat with ability evolution systems.

## Game Concept

Candy Rush is a survivor-style game where players battle waves of enemies using evolving abilities and projectile-based combat. The game features boss battles, character progression, and a variety of offensive and defensive abilities.

### Core Mechanics

- **Wave-Based Combat**: Fight against increasingly difficult enemy waves including special bosses (Void, Crab, Mask, Mega Slime, Wasp Queen)
- **Ability System**: Wide variety of abilities including Boomerang, Fireball, Boulder, Flying Dagger with evolution paths
- **Character Progression**: XP multipliers, damage reduction, speed upgrades, and size scaling
- **Projectile Combat**: Physics-based projectile system with various behaviors (homing, piercing, area damage)
- **Boss Encounters**: Unique boss enemies with special mechanics (Black Hole abilities for Void boss)

## Development Setup

### Requirements

- Unity 6000.2.5f1
- Visual Studio 2022 or Visual Studio Code
- Git for version control
- Google Drive CLI (`gdrive`) for external documentation
- GitHub CLI (`gh`) for repository management

### Project Structure

```
CandyRush/
├── Assets/                    # Unity assets, scripts, prefabs
│   ├── Common/               # Main game assets
│   │   ├── Scripts/         # Game logic
│   │   │   ├── Abilities/   # Ability system and behaviors
│   │   │   ├── Enemies/     # Enemy AI and boss behaviors
│   │   │   ├── Player/      # Player controller and stats
│   │   │   ├── UI/          # User interface components
│   │   │   ├── Upgrades/    # Character upgrade system
│   │   │   └── Wave/        # Wave spawning and management
│   │   ├── Prefabs/         # Reusable game objects
│   │   ├── Sprites/         # Character, items, UI, VFX sprites
│   │   ├── Audio/           # Music and sound effects
│   │   ├── Scenes/          # Game, Main Menu, Loading scenes
│   │   └── VFX/             # Visual effects
│   ├── Plugins/             # Platform-specific plugins
│   └── TextMesh Pro/        # Text rendering assets
├── Packages/                 # Unity Package Manager
├── ProjectSettings/          # Unity project configuration
└── .claude/                  # AI assistant configuration (Git-ignored)
```

## Current Features

- **Ability System**:
  - Multiple ability types with evolution paths (Fireball→Meteor, Boomerang→Recoiler, etc.)
  - Projectile behaviors (piercing, homing, area damage)
  - Cooldown and duration multipliers
- **Enemy System**:
  - Regular enemy waves with varied behaviors
  - 5 unique boss types with special abilities
  - Wave spawning system (burst, continuous, maintain modes)
- **Player Progression**:
  - Health system with damage reduction
  - Speed and size scaling
  - XP and gold multipliers
  - Magnet radius for item collection
- **Visual Effects**: Particle systems and sprite-based VFX
- **Scene Structure**: Main Menu, Loading Screen, Game scenes

## Game Design

### Target Audience
- Primary: Fans of roguelike survivor games (Vampire Survivors, Brotato)
- Secondary: Casual gamers who enjoy progression systems
- Players who like bullet-hell and wave defense games

### Gameplay Loop
1. Start with basic abilities and stats
2. Fight waves of enemies to gain XP and gold
3. Level up to unlock new abilities or evolve existing ones
4. Defeat boss enemies for major rewards
5. Survive as long as possible against escalating difficulty
6. Use collected resources for permanent upgrades

### Visual Style
- 2D sprite-based graphics
- Particle effects for abilities and impacts
- Clear visual feedback for damage and effects
- Distinct enemy and boss designs
- UI elements for health, abilities, and progression

## Technical Architecture

### Core Systems (Implemented)
- **Player Controller**: Movement, stats management, collision handling
- **Ability System**: Modular ability behaviors with evolution mechanics
- **Enemy AI**: Various enemy types with unique behaviors and boss patterns
- **Wave System**: Burst, continuous, and maintain wave spawning modes
- **Projectile System**: Physics-based projectiles with different behaviors
- **Health System**: Player and enemy health with damage calculations
- **Upgrade System**: Character progression and stat improvements
- **UI System**: Menus, health bars, ability indicators

## External Documentation

Project documentation is maintained in Google Drive for easy collaboration:
- Main folder: [Candy Rush Documentation](https://drive.google.com/drive/folders/1l_EQ8dg-jEw-qt8caFYc7xrOiOc_6WLF)
- Configuration folder: `.claude` subfolder within the main drive

### Loading External Memory with Claude

When starting a new Claude session, use the command "load external memory" to:
- Load project context from `.claude/settings.local.json`
- Check for documentation in Google Drive folders
- Sync any updated configuration files

For detailed Claude configuration instructions, see [CLAUDE.md](./CLAUDE.md)

## Development Roadmap

### Phase 1: Core Systems ✓
- [x] Player movement and controls
- [x] Basic ability system
- [x] Enemy spawning and AI
- [x] Wave management

### Phase 2: Content Expansion
- [ ] Additional ability types and evolutions
- [ ] More enemy varieties
- [ ] New boss mechanics
- [ ] Map variations

### Phase 3: Polish & Balance
- [ ] Ability balancing
- [ ] Difficulty curve tuning
- [ ] Visual effects enhancement
- [ ] Sound design implementation

### Phase 4: Meta Progression
- [ ] Permanent upgrades system
- [ ] Character unlocks
- [ ] Achievement system
- [ ] Leaderboards

## Build & Deployment

### Local Development
1. Open project in Unity 6000.2.5f1
2. Load the main scene from `Assets/Scenes/`
3. Press Play to test in editor

### Building
- **Windows**: File → Build Settings → PC, Mac & Linux Standalone
- **WebGL**: File → Build Settings → WebGL (for browser testing)

### Future Platforms
- Steam (PC/Mac/Linux)
- Potential mobile expansion (iOS/Android)
- Potential console ports

## Contributing

This project uses AI-assisted development with Claude. Configuration files in `.claude/` help maintain consistency and context across development sessions.

### Git Workflow
1. Create feature branches from `main`
2. Make changes and test thoroughly
3. Create pull requests for review
4. Merge to main after approval

## License

TBD - To be determined based on publishing strategy

## Contact

Project under development. Details to be added.

## Key Technologies

- **Unity 6000.2.5f1**: Latest Unity version for 2D development
- **Unity Input System**: Modern input handling for multiple control schemes
- **2D Pixel Perfect**: Crisp pixel art rendering
- **TextMesh Pro**: Advanced text rendering
- **Unity Timeline**: Cutscene and animation sequencing

---

*Candy Rush - Survive the sweet apocalypse!*