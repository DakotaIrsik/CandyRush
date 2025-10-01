# Static Accessor Migration Plan

## 🎯 **Current Status**

✅ **Core DI System**: Option A (Pure Services) architecture implemented
✅ **Immediate Compilation Fixes**: Critical errors resolved
⚠️ **Legacy Static Accessors**: Still active for backward compatibility

## 🔧 **Fixed Issues**

### **Immediate Compilation Errors Resolved:**
- ✅ `UpgradeItemBehavior.cs:128` - Converted to use injected `IUpgradesManager` and `IAudioManager`
- ✅ `StageFieldManager.cs:91,96` - Removed `instance` references, now uses instance methods
- ✅ `GameController.cs:222,224,239,241,256,258` - Fixed ProjectSettings static access in scene loading methods

### **Architecture Updated:**
- ✅ `GameController` maintains static accessors for backward compatibility
- ✅ New components use proper dependency injection
- ✅ `UpgradeItemBehavior` serves as reference implementation for DI conversion

## 📋 **44 Files Still Using Static Accessors**

These files need gradual migration from `GameController.StaticManager` to dependency injection:

### **High Priority (UI Components - User-facing)**
```
UI/Windows/UpgradesWindowBehavior.cs
UI/Screens/StageFailedScreen.cs
UI/CharacterItemBehavior.cs
UI/ToggleBehavior.cs
UI/JoystickBehavior.cs
UI/Windows/CharactersWindowBehavior.cs
UI/Windows/Chest Window/ChestWindowBehavior.cs
UI/UITimer.cs
UI/Screens/StageCompleteScreen.cs
```

### **Medium Priority (Core Game Systems)**
```
Stage/StageController.cs
Drop/Gem/DropBehavior.cs
Toolkit/Input/HighlightableButtonUI.cs
Toolkit/Input/HighlightsParentBehavior.cs
```

### **Lower Priority (Ability Behaviors - Many files)**
```
Abilities/Behaviors/Active Abilities/*/[Multiple files]
```

## 🔄 **Migration Pattern**

Follow the `UpgradeItemBehavior.cs` pattern for all conversions:

### **Before (Static Accessor):**
```csharp
public class ToggleBehavior : MonoBehaviour
{
    private void OnToggleClicked()
    {
        GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
    }
}
```

### **After (Dependency Injection):**
```csharp
public class ToggleBehavior : MonoBehaviour
{
    private IAudioManager audioManager;

    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        this.audioManager = audioManager;
    }

    private void OnToggleClicked()
    {
        audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);
    }
}
```

## 📊 **Migration Strategy**

### **Phase 1: UI Components (Week 1)**
- Convert high-priority UI components first
- These are most visible to users
- Easier to test and validate

### **Phase 2: Core Systems (Week 2)**
- Convert core game systems
- Test thoroughly in gameplay scenarios

### **Phase 3: Ability Behaviors (Week 3-4)**
- Convert ability behaviors in batches
- Many files but similar patterns
- Can be done in parallel

### **Phase 4: Cleanup (Week 5)**
- Remove static accessors from `GameController`
- Remove `GameControllerBridge`
- Mark legacy managers as obsolete

## 🚨 **Important Notes**

### **Current State is Stable:**
- ✅ No compilation errors
- ✅ DI system works for new components
- ✅ Legacy components still function
- ✅ Gradual migration possible

### **Registration Requirements:**
When converting UI components, ensure they're registered in the appropriate LifetimeScope:
- **Main Menu UI**: `ProjectLifetimeScope.cs`
- **Game UI**: `GameLifetimeScope.cs`

### **Testing Protocol:**
1. Convert component to use DI
2. Test in both Main Menu and Game scenes
3. Verify all functionality works
4. Check console for any missing injections

## 🎯 **Success Metrics**

- ✅ No compilation errors (ACHIEVED)
- ⏳ All UI components use DI (0/9 complete)
- ⏳ All core systems use DI (0/4 complete)
- ⏳ All ability behaviors use DI (0/31 complete)
- ⏳ Static accessors removed (final step)

## 🔄 **Next Actions**

1. **For You**: Test current build - everything should compile and run
2. **Phase 1**: Convert `ToggleBehavior.cs` as next example
3. **Phase 1**: Convert remaining UI components one by one
4. **Validation**: Test each converted component thoroughly

The architecture is now solid and ready for gradual migration! 🚀