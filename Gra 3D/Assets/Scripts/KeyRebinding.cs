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
        // Wy³¹cz wszystkie akcje przed ³adowaniem
        inputActions.Disable();

        LoadBindings();

        foreach (var binding in actionsToRebind)
        {
            var capturedBinding = binding;

            // Pomiñ akcjê "Look"
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

        // W³¹cz akcje po za³adowaniu
        inputActions.Enable();
    }

    public void StartRebinding(ActionBinding bindingInfo)
    {
        // Zablokuj rebinding dla akcji "Look"
        if (bindingInfo.actionName == "Player/Look")
        {
            Debug.LogWarning("Akcja 'Look' jest zablokowana i nie mo¿na jej zmieniæ!");
            return;
        }

        var action = inputActions.FindAction(bindingInfo.actionName);

        if (action == null)
        {
            Debug.LogError($"Nie znaleziono akcji: {bindingInfo.actionName}");
            return;
        }

        // Wy³¹cz akcjê podczas rebindingu
        action.Disable();

        bindingInfo.bindingDisplayText.text = "Naciœnij klawisz...";

        // Anuluj trwaj¹ce operacje rebindingu
        if (ongoingRebinding != null)
            ongoingRebinding.Dispose();

        ongoingRebinding = action.PerformInteractiveRebinding(bindingInfo.bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Gamepad>")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // Usuñ wszystkie bindingi dla tej akcji
                action.RemoveAllBindingOverrides();

                // Ustaw nowy binding, u¿ywaj¹c œcie¿ki z operation.selectedControl
                action.ApplyBindingOverride(bindingInfo.bindingIndex, operation.selectedControl.path);

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
        // Zapisz tylko bindingi dla akcji innych ni¿ "Look"
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Upewnij siê, ¿e "Look" nie ma nadpisywanych bindingów
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
            inputActions.RemoveAllBindingOverrides(); // Usuñ wszystkie domyœlne bindingi
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("[KeyRebindInput] Za³adowano zapisane bindingi");
        }

        // Upewnij siê, ¿e akcja "Look" ma domyœlne bindingi
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Resetuj bindingi dla "Look" do domyœlnych
        }

        foreach (var binding in actionsToRebind)
        {
            var action = inputActions.FindAction(binding.actionName);
            if (action != null && binding.actionName != "Player/Look")
            {
                // Usuñ wszystkie bindingi poza tym o okreœlonym indeksie, ale tylko dla akcji innych ni¿ "Look"
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