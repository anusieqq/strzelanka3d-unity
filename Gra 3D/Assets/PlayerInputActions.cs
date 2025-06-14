//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.0
//     from Assets/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""c6ee4a79-2dd9-4bb2-bc6d-9a0b1d702bd2"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""19fa96db-a491-4339-a430-6877c45f6ff3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""3fb2bc70-266e-40eb-a313-2a75b647c6cd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""b05cb62d-bde1-498a-8d32-7407cbcf6905"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""89b15b4f-78ad-4114-9a32-48b05a2581b9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""c6696086-0e0d-4769-8c14-2d0bb888faf0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Moving Left"",
                    ""type"": ""Button"",
                    ""id"": ""2d51c6bf-9e6f-4e01-82c2-0e99d9dccc90"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Moving Right"",
                    ""type"": ""Button"",
                    ""id"": ""16a00fb7-c58e-4aef-bcc4-aa1e27aa052a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Moving Up"",
                    ""type"": ""Button"",
                    ""id"": ""1201e45a-0c36-4991-a88b-780e365623f6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Moving Down"",
                    ""type"": ""Button"",
                    ""id"": ""56c9454b-e40f-4265-82dd-b1476e3fd5b7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire Torch"",
                    ""type"": ""Button"",
                    ""id"": ""3c9e36f2-4cd7-4a7d-9231-dc9a56e2cdf9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pistol"",
                    ""type"": ""Button"",
                    ""id"": ""ae5647e9-84fa-4fdc-9526-40b29176139d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire Torch Attack"",
                    ""type"": ""Button"",
                    ""id"": ""febf63e8-d2b8-4eaa-9b6b-faeb41181f5a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Speed Hour"",
                    ""type"": ""Button"",
                    ""id"": ""1f67caff-29d1-437c-bd73-3ff061d43aad"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slow Hour"",
                    ""type"": ""Button"",
                    ""id"": ""0e1a36e0-8f54-4966-9aa1-7e801edc61fd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0754d131-76f5-4517-a943-6cf3218e5036"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c30a1dd7-c607-4de5-89e9-25633131c728"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a396e956-ecaf-43c3-ab86-f869f4f8ad51"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df74dacb-0840-4223-9d11-cf97038ab060"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed5858b8-a49c-4873-9782-149568824753"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""426b30c4-53ee-4bfa-bcd1-44d3433acbb8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moving Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13c1354a-0e1a-4714-8604-80bca48bd225"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moving Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a674870-f547-4bf4-b9aa-5c33e5e00b13"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moving Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c974c2e-a637-4248-87c4-8caa91608bcf"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moving Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""559ed3b8-3908-440a-ac3d-ba5464f84c61"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire Torch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a978a67-b909-4840-8b09-a1b7379398e9"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pistol"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31fc02a0-4612-49a9-b993-5aa6af18a03b"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire Torch Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13b9df74-a14d-4720-a7f1-54e6969feb85"",
                    ""path"": ""<Keyboard>/numpadPlus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Speed Hour"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb20abcf-ba46-4c03-809e-f2142f34d883"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slow Hour"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_MovingLeft = m_Player.FindAction("Moving Left", throwIfNotFound: true);
        m_Player_MovingRight = m_Player.FindAction("Moving Right", throwIfNotFound: true);
        m_Player_MovingUp = m_Player.FindAction("Moving Up", throwIfNotFound: true);
        m_Player_MovingDown = m_Player.FindAction("Moving Down", throwIfNotFound: true);
        m_Player_FireTorch = m_Player.FindAction("Fire Torch", throwIfNotFound: true);
        m_Player_Pistol = m_Player.FindAction("Pistol", throwIfNotFound: true);
        m_Player_FireTorchAttack = m_Player.FindAction("Fire Torch Attack", throwIfNotFound: true);
        m_Player_SpeedHour = m_Player.FindAction("Speed Hour", throwIfNotFound: true);
        m_Player_SlowHour = m_Player.FindAction("Slow Hour", throwIfNotFound: true);
    }

    ~@PlayerInputActions()
    {
        UnityEngine.Debug.Assert(!m_Player.enabled, "This will cause a leak and performance issues, PlayerInputActions.Player.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Shoot;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_MovingLeft;
    private readonly InputAction m_Player_MovingRight;
    private readonly InputAction m_Player_MovingUp;
    private readonly InputAction m_Player_MovingDown;
    private readonly InputAction m_Player_FireTorch;
    private readonly InputAction m_Player_Pistol;
    private readonly InputAction m_Player_FireTorchAttack;
    private readonly InputAction m_Player_SpeedHour;
    private readonly InputAction m_Player_SlowHour;
    public struct PlayerActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @MovingLeft => m_Wrapper.m_Player_MovingLeft;
        public InputAction @MovingRight => m_Wrapper.m_Player_MovingRight;
        public InputAction @MovingUp => m_Wrapper.m_Player_MovingUp;
        public InputAction @MovingDown => m_Wrapper.m_Player_MovingDown;
        public InputAction @FireTorch => m_Wrapper.m_Player_FireTorch;
        public InputAction @Pistol => m_Wrapper.m_Player_Pistol;
        public InputAction @FireTorchAttack => m_Wrapper.m_Player_FireTorchAttack;
        public InputAction @SpeedHour => m_Wrapper.m_Player_SpeedHour;
        public InputAction @SlowHour => m_Wrapper.m_Player_SlowHour;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
            @MovingLeft.started += instance.OnMovingLeft;
            @MovingLeft.performed += instance.OnMovingLeft;
            @MovingLeft.canceled += instance.OnMovingLeft;
            @MovingRight.started += instance.OnMovingRight;
            @MovingRight.performed += instance.OnMovingRight;
            @MovingRight.canceled += instance.OnMovingRight;
            @MovingUp.started += instance.OnMovingUp;
            @MovingUp.performed += instance.OnMovingUp;
            @MovingUp.canceled += instance.OnMovingUp;
            @MovingDown.started += instance.OnMovingDown;
            @MovingDown.performed += instance.OnMovingDown;
            @MovingDown.canceled += instance.OnMovingDown;
            @FireTorch.started += instance.OnFireTorch;
            @FireTorch.performed += instance.OnFireTorch;
            @FireTorch.canceled += instance.OnFireTorch;
            @Pistol.started += instance.OnPistol;
            @Pistol.performed += instance.OnPistol;
            @Pistol.canceled += instance.OnPistol;
            @FireTorchAttack.started += instance.OnFireTorchAttack;
            @FireTorchAttack.performed += instance.OnFireTorchAttack;
            @FireTorchAttack.canceled += instance.OnFireTorchAttack;
            @SpeedHour.started += instance.OnSpeedHour;
            @SpeedHour.performed += instance.OnSpeedHour;
            @SpeedHour.canceled += instance.OnSpeedHour;
            @SlowHour.started += instance.OnSlowHour;
            @SlowHour.performed += instance.OnSlowHour;
            @SlowHour.canceled += instance.OnSlowHour;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
            @MovingLeft.started -= instance.OnMovingLeft;
            @MovingLeft.performed -= instance.OnMovingLeft;
            @MovingLeft.canceled -= instance.OnMovingLeft;
            @MovingRight.started -= instance.OnMovingRight;
            @MovingRight.performed -= instance.OnMovingRight;
            @MovingRight.canceled -= instance.OnMovingRight;
            @MovingUp.started -= instance.OnMovingUp;
            @MovingUp.performed -= instance.OnMovingUp;
            @MovingUp.canceled -= instance.OnMovingUp;
            @MovingDown.started -= instance.OnMovingDown;
            @MovingDown.performed -= instance.OnMovingDown;
            @MovingDown.canceled -= instance.OnMovingDown;
            @FireTorch.started -= instance.OnFireTorch;
            @FireTorch.performed -= instance.OnFireTorch;
            @FireTorch.canceled -= instance.OnFireTorch;
            @Pistol.started -= instance.OnPistol;
            @Pistol.performed -= instance.OnPistol;
            @Pistol.canceled -= instance.OnPistol;
            @FireTorchAttack.started -= instance.OnFireTorchAttack;
            @FireTorchAttack.performed -= instance.OnFireTorchAttack;
            @FireTorchAttack.canceled -= instance.OnFireTorchAttack;
            @SpeedHour.started -= instance.OnSpeedHour;
            @SpeedHour.performed -= instance.OnSpeedHour;
            @SpeedHour.canceled -= instance.OnSpeedHour;
            @SlowHour.started -= instance.OnSlowHour;
            @SlowHour.performed -= instance.OnSlowHour;
            @SlowHour.canceled -= instance.OnSlowHour;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnShoot(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnMovingLeft(InputAction.CallbackContext context);
        void OnMovingRight(InputAction.CallbackContext context);
        void OnMovingUp(InputAction.CallbackContext context);
        void OnMovingDown(InputAction.CallbackContext context);
        void OnFireTorch(InputAction.CallbackContext context);
        void OnPistol(InputAction.CallbackContext context);
        void OnFireTorchAttack(InputAction.CallbackContext context);
        void OnSpeedHour(InputAction.CallbackContext context);
        void OnSlowHour(InputAction.CallbackContext context);
    }
}
