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

    // Zdarzenie wywo³ywane po zmianie bindingów
    public static event Action OnBindingsChanged;

    void Start()
    {
        // Walidacja InputActionAsset
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w KeyRebindInput! Przypisz go w Inspectorze.");
            return;
        }

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
            else
            {
                Debug.LogWarning($"Brak przypisanego przycisku dla akcji {capturedBinding.actionName} w KeyRebindInput.");
            }

            UpdateBindingDisplay(capturedBinding);
        }

        // W³¹cz akcje po za³adowaniu
        inputActions.Enable();
        Debug.Log("InputActionAsset w³¹czone w KeyRebindInput.");
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
            Debug.LogError($"Nie znaleziono akcji: {bindingInfo.actionName}. SprawdŸ nazwê akcji w InputActionAsset.");
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
                // Ustaw nowy binding
                action.ApplyBindingOverride(bindingInfo.bindingIndex, operation.selectedControl.path);

                operation.Dispose();
                ongoingRebinding = null;

                UpdateBindingDisplay(bindingInfo);
                SaveBindings();

                // Wywo³aj zdarzenie po zmianie bindingów
                OnBindingsChanged?.Invoke();

                // W³¹cz akcjê po zakoñczeniu
                action.Enable();
                Debug.Log($"Zakoñczono rebinding dla akcji {bindingInfo.actionName}. Nowy binding: {operation.selectedControl.path}");
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                ongoingRebinding = null;
                UpdateBindingDisplay(bindingInfo);

                // W³¹cz akcjê po anulowaniu
                action.Enable();
                Debug.Log($"Anulowano rebinding dla akcji {bindingInfo.actionName}.");
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
            Debug.Log($"Zaktualizowano wyœwietlanie bindingu dla akcji {bindingInfo.actionName}: {bindingStr}");
        }
        else
        {
            Debug.LogWarning($"Nie mo¿na zaktualizowaæ wyœwietlanego bindingu dla akcji {bindingInfo.actionName}. SprawdŸ bindingIndex lub akcjê.");
        }
    }

    public void SaveBindings()
    {
        // Zapisz tylko bindingi dla akcji innych ni¿ "Look"
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Resetuj bindingi dla "Look"
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

            // Wywo³aj zdarzenie po za³adowaniu bindingów
            OnBindingsChanged?.Invoke();
        }

        // Upewnij siê, ¿e akcja "Look" ma domyœlne bindingi
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Resetuj bindingi dla "Look" do domyœlnych
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

        // Wywo³aj zdarzenie po resecie bindingów
        OnBindingsChanged?.Invoke();

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