using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;

public class KeyRebindInput : MonoBehaviour
{
    [Serializable]
    public class ActionBinding
    {
        public string actionName;
        public int bindingIndex;
        public Button rebindButton;
        public TextMeshProUGUI bindingDisplayText;
    }

    public InputActionAsset inputActions;
    public ActionBinding[] actionsToRebind;

    private const string RebindsKey = "Rebinds";
    private InputActionRebindingExtensions.RebindingOperation ongoingRebinding;

    void Start()
    {
        // Wy³¹cz wszystkie akcje przed ³adowaniem
        inputActions.Disable();

        LoadBindings();

        foreach (var binding in actionsToRebind)
        {
            // Utwórz lokaln¹ kopiê, aby unikn¹æ problemów z closure w lambdach
            var capturedBinding = binding;

            if (capturedBinding.rebindButton != null)
            {
                capturedBinding.rebindButton.onClick.AddListener(() =>
                {
                    StartRebinding(capturedBinding);
                });
            }

            UpdateBindingDisplay(capturedBinding);
        }

        // W³¹cz tylko po za³adowaniu
        inputActions.Enable();
    }

    public void StartRebinding(ActionBinding bindingInfo)
    {
        var action = inputActions.FindAction(bindingInfo.actionName);

        if (action == null)
        {
            Debug.LogError($"Nie znaleziono akcji: {bindingInfo.actionName}");
            return;
        }

        // Wy³¹cz akcjê podczas rebindingu
        action.Disable();

        bindingInfo.bindingDisplayText.text = "Naciœnij klawisz...";

        // Anuluj wszelkie trwaj¹ce operacje rebindingu
        if (ongoingRebinding != null)
            ongoingRebinding.Dispose();

        ongoingRebinding = action.PerformInteractiveRebinding(bindingInfo.bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Gamepad>")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // Usuñ wszystkie inne bindingi dla tej akcji
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (i != bindingInfo.bindingIndex)
                    {
                        action.RemoveBindingOverride(i);
                    }
                }

                operation.Dispose();
                ongoingRebinding = null;

                UpdateBindingDisplay(bindingInfo);
                SaveBindings();

                // W³¹cz akcjê po zakoñczeniu
                action.Enable();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                ongoingRebinding = null;
                UpdateBindingDisplay(bindingInfo);

                // W³¹cz akcjê po anulowaniu
                action.Enable();
            })
            .Start();
    }

    void UpdateBindingDisplay(ActionBinding bindingInfo)
    {
        var action = inputActions.FindAction(bindingInfo.actionName);
        if (action != null && action.bindings.Count > bindingInfo.bindingIndex)
        {
            string bindingStr = action.GetBindingDisplayString(bindingInfo.bindingIndex,
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            bindingInfo.bindingDisplayText.text = bindingStr;
        }
    }

    public void SaveBindings()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsKey, rebinds);
        PlayerPrefs.Save();
        Debug.Log("[KeyRebindInput] Zapisano bindingi:\n" + rebinds);
    }

    public void LoadBindings()
    {
        if (PlayerPrefs.HasKey(RebindsKey))
        {
            string rebinds = PlayerPrefs.GetString(RebindsKey);
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("[KeyRebindInput] Za³adowano zapisane bindingi");
        }
        else
        {
            // Jeœli nie ma zapisanych bindingów, usuñ wszystkie domyœlne poza pierwszym
            foreach (var binding in actionsToRebind)
            {
                var action = inputActions.FindAction(binding.actionName);
                if (action != null)
                {
                    // Zostaw tylko binding pod wskazanym indeksem
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        if (i != binding.bindingIndex)
                        {
                            action.RemoveBindingOverride(i);
                        }
                    }
                }
            }
        }
    }

    public void ResetBindings()
    {
        inputActions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(RebindsKey);

        // Ponownie za³aduj domyœlne bindingi i usuñ niepotrzebne
        foreach (var binding in actionsToRebind)
        {
            var action = inputActions.FindAction(binding.actionName);
            if (action != null)
            {
                // Zostaw tylko binding pod wskazanym indeksem
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (i != binding.bindingIndex)
                    {
                        action.RemoveBindingOverride(i);
                    }
                }
            }
            UpdateBindingDisplay(binding);
        }
        Debug.Log("[KeyRebindInput] Resetowano bindingi");
    }

    void OnDestroy()
    {
        if (ongoingRebinding != null)
        {
            ongoingRebinding.Dispose();
        }
    }
}