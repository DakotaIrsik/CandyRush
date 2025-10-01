# VContainer Dependency Injection Patterns

## ✅ The Gold Standard: AudioManager Pattern

AudioManager is our reference implementation for proper dependency injection with VContainer. Follow this pattern for all manager/service MonoBehaviours.

### Key Principles

1. **NO Singleton Pattern**
   - Remove all `private static instance` fields
   - No `if (instance != null) Destroy()` checks
   - Let VContainer handle the lifecycle completely

2. **Clean Dependency Injection**
   ```csharp
   [Inject]
   public void Construct(ISaveManager saveManager)
   {
       this.saveManager = saveManager;
   }
   ```

3. **Proper Initialization Order**
   ```csharp
   private void Awake()
   {
       // Only Unity-specific setup that doesn't need dependencies
       DontDestroyOnLoad(gameObject);
   }

   private void Start()
   {
       // Initialize after all dependencies are injected
       Initialize();
   }
   ```

4. **Component Registration**
   ```csharp
   // In ProjectLifetimeScope.cs
   [SerializeField] private AudioManager audioManager;

   protected override void Configure(IContainerBuilder builder)
   {
       if (audioManager != null)
       {
           builder.RegisterComponent(audioManager)
                  .As<IAudioManager>()
                  .AsSelf();
       }
   }
   ```

## ❌ Anti-Patterns to Avoid

### 1. Service Locator Disguised as DI
```csharp
// BAD - This is service locator, not DI!
builder.Register<IAudioManager>(resolver =>
{
    var existing = FindObjectOfType<AudioManager>();
    if (existing != null) {
        existing.Construct(resolver.Resolve<ISaveManager>());
        return existing;
    }
    return null;
}, Lifetime.Singleton);
```

### 2. Mixed Singleton and DI
```csharp
// BAD - Don't mix patterns!
public class AudioManager : MonoBehaviour, IAudioManager
{
    private static AudioManager instance; // NO!

    [Inject]
    public void Construct(ISaveManager saveManager) // Confusing!
    {
        // ...
    }
}
```

### 3. Over-Engineering with Multiple Classes
```csharp
// BAD - Too complex for Unity components
AudioService.cs      // Pure C# service
AudioBootstrapper.cs  // MonoBehaviour wrapper
AudioSourceProvider.cs // Another helper
// Just use AudioManager.cs!
```

## 📋 Conversion Checklist

When converting a manager to proper DI:

- [ ] Remove `private static instance` field
- [ ] Remove singleton check in `Awake()`
- [ ] Add `[Inject]` constructor method
- [ ] Move initialization from `Awake()` to `Start()` or `Initialize()`
- [ ] Add `[SerializeField]` reference in ProjectLifetimeScope
- [ ] Register component in `Configure()` method
- [ ] Update all static references to use injection
- [ ] Remove `GameController.ManagerName` static accessors

## 🎯 The Goal

Simple, clean, testable code where:
- Dependencies are explicit (constructor parameters)
- No hidden static dependencies
- VContainer manages lifecycles
- Easy to mock for testing
- Clear initialization order

Remember: **Keep it simple!** If you're creating multiple classes for what used to be one manager, you're probably over-engineering it.