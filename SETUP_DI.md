# Setting Up Pure Dependency Injection

## 🎯 Goal
Convert from GameObject-based managers to pure DI services managed entirely by VContainer.

## ✅ Setup Steps

### 1. Create Service Configuration
1. Create a new ScriptableObject: **Assets > Create > DI > Service Configuration**
2. Name it `ServiceConfiguration`
3. Configure:
   - **Audio Database**: Drag your AudioDatabase asset
   - **Audio Source Prefab**: Create a simple prefab with AudioSource component
   - **Save Settings**: Configure as needed
4. Place in `Assets/Resources/` folder (create if doesn't exist)

### 2. Remove Manager GameObjects from Scene
In the Main Menu scene, delete or disable these GameObjects:
- [ ] AudioManager
- [ ] SaveManager
- [ ] InputManager
- [ ] VibrationManager

These are now created automatically by the DI container!

### 3. Test the System
1. Play the Main Menu scene
2. Check Console for:
   - `[AudioService] Initialized via dependency injection`
   - `[AudioService] Initialized X sounds from database`
3. Verify audio works when clicking buttons

## 🏗️ Architecture

### Pure DI Pattern
```
RootLifetimeScope (Auto-created by VContainer)
    ├── Loads ServiceConfiguration from Resources
    ├── Creates SaveService (pure C#)
    ├── Creates AudioService (pure C#)
    ├── Creates AudioSystemUpdater (minimal MonoBehaviour)
    └── All services persist across scenes

MainMenuLifetimeScope (In Main Menu scene)
    ├── Inherits all services from Root
    ├── Registers scene-specific UI components
    └── No duplicate service registrations
```

### Service Lifecycle
1. **VContainer starts** → Creates RootLifetimeScope automatically
2. **Root configures** → Loads ServiceConfiguration, creates services
3. **Services initialize** → No scene dependencies, pure DI
4. **Scene loads** → MainMenuLifetimeScope adds scene-specific components
5. **Scene changes** → Services persist, UI components destroyed

## 🔧 Converting Other Managers

To convert SaveManager, InputManager, etc. to pure DI:

1. **Create Service Class**
   ```csharp
   public class SaveService : ISaveManager
   {
       // Pure C# implementation
       // No MonoBehaviour inheritance
   }
   ```

2. **Register in RootLifetimeScope**
   ```csharp
   builder.Register<ISaveManager, SaveService>(Lifetime.Singleton);
   ```

3. **Create Updater if needed**
   ```csharp
   public class SaveSystemUpdater : MonoBehaviour
   {
       [Inject] ISaveManager saveManager;
       // Handle Unity callbacks
   }
   ```

4. **Remove from scenes**
   - Delete GameObject from all scenes
   - Let DI create and manage

## ⚡ Benefits

- **No singletons** - VContainer manages lifecycle
- **No scene dependencies** - Services work anywhere
- **Pure testability** - Mock any dependency
- **Clean architecture** - Clear separation of concerns
- **Automatic persistence** - DontDestroyOnLoad handled by DI

## 🚨 Important Notes

- **DO NOT** create manager GameObjects in scenes
- **DO NOT** use singleton patterns
- **DO NOT** use FindObjectOfType
- **DO** let VContainer create everything
- **DO** use constructor injection
- **DO** keep services pure C# when possible