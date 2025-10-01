using OctoberStudio.Save;
using OctoberStudio.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace OctoberStudio.Input
{
    public class InputService : IInputManager
    {
        private readonly ISaveManager saveManager;
        private InputSave save;
        private InputAsset inputAsset;
        private HighlightsParentBehavior highlights;
        private float initializationTime;

        public HighlightsParentBehavior Highlights => highlights;
        public InputAsset InputAsset => inputAsset;

        public InputType ActiveInput
        {
            get => save?.ActiveInput ?? InputType.UIJoystick;
            private set
            {
                if (save != null)
                    save.ActiveInput = value;
            }
        }

        public Vector2 MovementValue { get; private set; }
        public JoystickBehavior Joystick { get; set; }

        public event UnityAction<InputType, InputType> onInputChanged;

        public InputService(ISaveManager saveManager, HighlightsParentBehavior highlights = null)
        {
            this.saveManager = saveManager;
            this.highlights = highlights;
            Initialize();
        }

        private void Initialize()
        {
            initializationTime = Time.time;
            inputAsset = new InputAsset();
            inputAsset.Enable(); // Enable input asset immediately
            Debug.Log("[InputService] InputAsset created and enabled");

            if (saveManager.IsSaveLoaded)
            {
                LoadSaveData();
            }
            else
            {
                saveManager.OnSaveLoaded += LoadSaveData;
            }

            // Initialize input detection
            if (Gamepad.current != null)
            {
                ActiveInput = InputType.Gamepad;
                Debug.Log("[InputService] Gamepad detected, ActiveInput set to Gamepad");
            }
            else
            {
                ActiveInput = InputType.UIJoystick;
                Debug.Log("[InputService] No gamepad detected, ActiveInput set to UIJoystick");
            }
        }

        private void LoadSaveData()
        {
            save = saveManager.GetSave<InputSave>("Input");
            saveManager.OnSaveLoaded -= LoadSaveData;
        }

        public void RegisterJoystick(JoystickBehavior joystick)
        {
            Joystick = joystick;
            Debug.Log("[InputService] Joystick registered");
        }

        public void RemoveJoystick()
        {
            Joystick = null;
            Debug.Log("[InputService] Joystick removed");
        }

        public void SetHighlights(HighlightsParentBehavior highlights)
        {
            this.highlights = highlights;
        }

        private bool loggedOnce = false;

        // This would need to be called from a MonoBehaviour Update
        public void Update()
        {
            if (!loggedOnce)
            {
                Debug.Log($"[InputService] Update called - ActiveInput: {ActiveInput} (raw: {(int)ActiveInput}), Save: {(save != null ? "exists" : "null")}, Joystick: {(Joystick != null ? "exists" : "null")}, InputAsset: {(inputAsset != null ? "exists" : "null")}");
                loggedOnce = true;
            }

            // Input detection and switching logic (from original InputManager)
            if (ActiveInput != InputType.Keyboard && Keyboard.current != null &&
                Keyboard.current.wasUpdatedThisFrame &&
                !Keyboard.current.CheckStateIsAtDefaultIgnoringNoise())
            {
                Debug.Log("[InputService] Switching To Keyboard");
                var prevInput = ActiveInput;
                ActiveInput = InputType.Keyboard;

                if (Joystick != null) Joystick.Disable();
                highlights?.EnableArrows();
                onInputChanged?.Invoke(prevInput, InputType.Keyboard);
            }

            if (ActiveInput != InputType.UIJoystick &&
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame ||
                ActiveInput == InputType.Gamepad && Gamepad.current == null ||
                Touchscreen.current != null && Touchscreen.current.wasUpdatedThisFrame))
            {
                Debug.Log("[InputService] Switching To UI Joystick");
                var prevInput = ActiveInput;
                ActiveInput = InputType.UIJoystick;

                if (Joystick != null) Joystick.Enable();
                highlights?.DisableArrows();
                onInputChanged?.Invoke(prevInput, InputType.UIJoystick);
            }

            var oldValue = MovementValue;

            if (ActiveInput == InputType.UIJoystick)
            {
                if (Joystick != null)
                {
                    MovementValue = Joystick.Value;
                    if (oldValue != MovementValue && MovementValue.magnitude > 0.1f)
                    {
                        Debug.Log($"[InputService] Joystick movement: {MovementValue}, Joystick.Value: {Joystick.Value}");
                    }
                }
                else
                {
                    // In UIJoystick mode but joystick not registered yet - use zero movement
                    MovementValue = Vector2.zero;
                }
            }
            else
            {
                MovementValue = inputAsset?.Gameplay.Movement.ReadValue<Vector2>() ?? Vector2.zero;
                if (oldValue != MovementValue && MovementValue.magnitude > 0.1f)
                {
                    Debug.Log($"[InputService] Keyboard/Gamepad movement: {MovementValue}");
                }
            }
        }

        public void Enable()
        {
            inputAsset?.Enable();
        }

        public void Disable()
        {
            inputAsset?.Disable();
        }
    }
}