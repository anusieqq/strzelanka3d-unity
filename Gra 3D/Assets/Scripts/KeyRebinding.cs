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
        public int bindingIndex; // Indeks bindingu w akcji
        public Button rebindButton;
        public TextMeshProUGUI bindingDisplayText;
    }

    public InputActionAsset inputActions;
    public ActionBinding[] actionsToRebind; // Tablica dla wszystkich akcji

    private const string RebindsKey = "Rebinds";
    private InputActionRebindingExtensions.RebindingOperation ongoingRebinding;

    void Start()
    {
        // Wy��cz wszystkie akcje przed �adowaniem
        inputActions.Disable();

        LoadBindings();

        foreach (var binding in actionsToRebind)
        {
            var capturedBinding = binding;

            // Pomi� akcj� "Look"
            if (capturedBinding.actionName == "Player/Look")
            {
                UpdateBindingDisplay(capturedBinding);
                continue; // Nie przypisuj przycisku rebindingu dla akcji "Look"
            }

            if (capturedBinding.rebindButton != null)
            {
                capturedBinding.rebindButton.onClick.AddListener(() =>
                {
                    StartRebinding(capturedBinding);
                });
            }

            UpdateBindingDisplay(capturedBinding);
        }

        // W��cz akcje po za�adowaniu
        inputActions.Enable();
    }

    public void StartRebinding(ActionBinding bindingInfo)
    {
        // Zablokuj rebinding dla akcji "Look"
        if (bindingInfo.actionName == "Player/Look")
        {
            Debug.LogWarning("Akcja 'Look' jest zablokowana i nie mo�na jej zmieni�!");
            return;
        }

        var action = inputActions.FindAction(bindingInfo.actionName);

        if (action == null)
        {
            Debug.LogError($"Nie znaleziono akcji: {bindingInfo.actionName}");
            return;
        }

        // Wy��cz akcj� podczas rebindingu
        action.Disable();

        bindingInfo.bindingDisplayText.text = "Naci�nij klawisz...";

        // Anuluj trwaj�ce operacje rebindingu
        if (ongoingRebinding != null)
            ongoingRebinding.Dispose();

        ongoingRebinding = action.PerformInteractiveRebinding(bindingInfo.bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Gamepad>")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // Usu� wszystkie bindingi dla tej akcji
                action.RemoveAllBindingOverrides();

                // Ustaw nowy binding, u�ywaj�c �cie�ki z operation.selectedControl
                action.ApplyBindingOverride(bindingInfo.bindingIndex, operation.selectedControl.path);

                operation.Dispose();
                ongoingRebinding = null;

                UpdateBindingDisplay(bindingInfo);
                SaveBindings();

                // W��cz akcj� po zako�czeniu
                action.Enable();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                ongoingRebinding = null;
                UpdateBindingDisplay(bindingInfo);

                // W��cz akcj� po anulowaniu
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
        // Zapisz tylko bindingi dla akcji innych ni� "Look"
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Upewnij si�, �e "Look" nie ma nadpisywanych binding�w
        }

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
            inputActions.RemoveAllBindingOverrides(); // Usu� wszystkie domy�lne bindingi
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("[KeyRebindInput] Za�adowano zapisane bindingi");
        }

        // Upewnij si�, �e akcja "Look" ma domy�lne bindingi
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Resetuj bindingi dla "Look" do domy�lnych
        }

        foreach (var binding in actionsToRebind)
        {
            var action = inputActions.FindAction(binding.actionName);
            if (action != null && binding.actionName != "Player/Look")
            {
                // Usu� wszystkie bindingi poza tym o okre�lonym indeksie, ale tylko dla akcji innych ni� "Look"
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

    public void ResetBindings()
    {
        inputActions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(RebindsKey);

        foreach (var binding in actionsToRebind)
        {
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