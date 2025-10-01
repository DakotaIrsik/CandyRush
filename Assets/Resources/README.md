# Resources Folder Organization Guide

## 🏗️ **Folder Structure**

```
Assets/Resources/
├── Configuration/           # DI Configuration assets
│   ├── Core/               # Core system configs (cross-scene)
│   │   ├── ServiceConfiguration.asset
│   │   ├── SaveConfiguration.asset
│   │   └── PoolConfiguration.asset
│   ├── Systems/            # Individual system configs
│   │   ├── AudioConfiguration.asset
│   │   ├── InputConfiguration.asset
│   │   ├── CurrenciesConfiguration.asset
│   │   └── UpgradesConfiguration.asset
│   └── Scenes/             # Scene-specific configs (if needed)
│       ├── MainMenuConfiguration.asset
│       └── GameConfiguration.asset
├── Data/                   # Game data & databases
│   ├── Audio/
│   │   └── AudioDatabase.asset
│   ├── Currencies/
│   │   └── CurrenciesDatabase.asset
│   ├── Upgrades/
│   │   └── UpgradesDatabase.asset
│   ├── Abilities/
│   │   └── AbilitiesDatabase.asset
│   ├── Enemies/
│   │   └── EnemiesDatabase.asset
│   └── Stages/
│       └── StagesDatabase.asset
├── Localization/           # Text & language files
│   ├── EN/
│   │   ├── UI.json
│   │   ├── Gameplay.json
│   │   └── Errors.json
│   ├── ES/
│   └── FR/
└── Settings/               # Runtime settings & presets
    ├── Graphics/
    │   ├── LowQuality.asset
    │   ├── MediumQuality.asset
    │   └── HighQuality.asset
    ├── Audio/
    │   └── DefaultMixerSettings.asset
    └── Controls/
        ├── KeyboardDefaults.asset
        └── GamepadDefaults.asset
```

## 🎯 **Organization Principles**

### **Configuration/** - Dependency Injection Setup
- **Core/**: Cross-scene system configurations
- **Systems/**: Individual service configurations
- **Scenes/**: Scene-specific setup (rare)

**Naming Convention**: `[SystemName]Configuration.asset`

### **Data/** - Game Content & Databases
- Organized by **domain/feature**
- Contains ScriptableObject databases
- Read-only game content

**Naming Convention**: `[DomainName]Database.asset`

### **Localization/** - Text Content
- Organized by **language code** (ISO 639-1)
- JSON files for easy editing
- Grouped by feature/screen

**Naming Convention**: `[FeatureName].json`

### **Settings/** - User Preferences & Presets
- Runtime-modifiable settings
- Quality presets
- User preference defaults

**Naming Convention**: `[PresetName].asset`

## 🔧 **Usage Examples**

### **Loading Configuration**
```csharp
// Core system config
var serviceConfig = Resources.Load<ServiceConfiguration>("Configuration/Core/ServiceConfiguration");

// Individual system config
var audioConfig = Resources.Load<AudioConfiguration>("Configuration/Systems/AudioConfiguration");
```

### **Loading Game Data**
```csharp
// Domain-specific database
var audioDatabase = Resources.Load<AudioDatabase>("Data/Audio/AudioDatabase");
var currencyDatabase = Resources.Load<CurrenciesDatabase>("Data/Currencies/CurrenciesDatabase");
```

### **Loading Localization**
```csharp
// Language-specific text
var uiText = Resources.Load<TextAsset>("Localization/EN/UI");
var gameplayText = Resources.Load<TextAsset>("Localization/EN/Gameplay");
```

## 🚀 **Migration Strategy**

### **Phase 1: Create Folder Structure**
1. Create the folder hierarchy shown above
2. Move existing assets to appropriate locations
3. Update references in code

### **Phase 2: Update Loading Code**
1. RootLifetimeScope uses new paths with fallback to old paths
2. Gradually migrate other Resources.Load calls
3. Remove fallback once migration complete

### **Phase 3: Optimize**
1. Consider Addressables for larger assets
2. Review what actually needs to be in Resources
3. Move non-essential assets to standard asset folders

## 📋 **Current Migration Status**

**✅ Ready to Move:**
- ServiceConfiguration.asset → Configuration/Core/
- SaveConfiguration.asset → Configuration/Core/
- PoolConfiguration.asset → Configuration/Core/
- AudioConfiguration.asset → Configuration/Systems/
- InputConfiguration.asset → Configuration/Systems/
- CurrenciesConfiguration.asset → Configuration/Systems/
- UpgradesConfiguration.asset → Configuration/Systems/

**🔍 Need to Locate:**
- AudioDatabase.asset → Data/Audio/
- CurrenciesDatabase.asset → Data/Currencies/
- UpgradesDatabase.asset → Data/Upgrades/

## ⚠️ **Important Notes**

1. **Resources folder increases build size** - Only put assets that MUST be loaded by path
2. **Consider Addressables** for large assets or complex loading scenarios
3. **Keep it clean** - Regular cleanup prevents folder bloat
4. **Document changes** - Update this README when adding new categories

## 🎯 **Best Practices**

- **Consistent naming**: Follow the established conventions
- **Logical grouping**: Group related assets together
- **Avoid deep nesting**: Maximum 3 levels deep
- **Use descriptive names**: Asset names should be self-explanatory
- **Regular cleanup**: Remove unused assets periodically

---

*This structure supports the vContainer dependency injection system and scales with project growth.*