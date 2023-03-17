//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/InputSystem/PlayerControls.inputactions
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

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""GameInputs"",
            ""id"": ""2b75fd33-a5c8-4878-a66d-d560ad9f51f7"",
            ""actions"": [
                {
                    ""name"": ""SingleTouch"",
                    ""type"": ""Button"",
                    ""id"": ""7c2ac2aa-20a9-4ce0-af7c-04faa936bcb2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f7eb1f3b-df00-4da7-adc3-05835d70bea7"",
                    ""path"": ""<Touchscreen>/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SingleTouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf737c76-4a1a-4709-a917-f3465334f5d8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SingleTouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GameInputs
        m_GameInputs = asset.FindActionMap("GameInputs", throwIfNotFound: true);
        m_GameInputs_SingleTouch = m_GameInputs.FindAction("SingleTouch", throwIfNotFound: true);
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

    // GameInputs
    private readonly InputActionMap m_GameInputs;
    private List<IGameInputsActions> m_GameInputsActionsCallbackInterfaces = new List<IGameInputsActions>();
    private readonly InputAction m_GameInputs_SingleTouch;
    public struct GameInputsActions
    {
        private @PlayerControls m_Wrapper;
        public GameInputsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SingleTouch => m_Wrapper.m_GameInputs_SingleTouch;
        public InputActionMap Get() { return m_Wrapper.m_GameInputs; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameInputsActions set) { return set.Get(); }
        public void AddCallbacks(IGameInputsActions instance)
        {
            if (instance == null || m_Wrapper.m_GameInputsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameInputsActionsCallbackInterfaces.Add(instance);
            @SingleTouch.started += instance.OnSingleTouch;
            @SingleTouch.performed += instance.OnSingleTouch;
            @SingleTouch.canceled += instance.OnSingleTouch;
        }

        private void UnregisterCallbacks(IGameInputsActions instance)
        {
            @SingleTouch.started -= instance.OnSingleTouch;
            @SingleTouch.performed -= instance.OnSingleTouch;
            @SingleTouch.canceled -= instance.OnSingleTouch;
        }

        public void RemoveCallbacks(IGameInputsActions instance)
        {
            if (m_Wrapper.m_GameInputsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameInputsActions instance)
        {
            foreach (var item in m_Wrapper.m_GameInputsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameInputsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameInputsActions @GameInputs => new GameInputsActions(this);
    public interface IGameInputsActions
    {
        void OnSingleTouch(InputAction.CallbackContext context);
    }
}
