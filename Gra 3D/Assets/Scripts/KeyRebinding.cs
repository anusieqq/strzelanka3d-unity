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

    // Zdarzenie wywo�ywane po zmianie binding�w
    public static event Action OnBindingsChanged;

    void Start()
    {
        // Walidacja InputActionAsset
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w KeyRebindInput! Przypisz go w Inspectorze.");
            return;
        }

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
            else
            {
                Debug.LogWarning($"Brak przypisanego przycisku dla akcji {capturedBinding.actionName} w KeyRebindInput.");
            }

            UpdateBindingDisplay(capturedBinding);
        }

        // W��cz akcje po za�adowaniu
        inputActions.Enable();
        Debug.Log("InputActionAsset w��czone w KeyRebindInput.");
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
            Debug.LogError($"Nie znaleziono akcji: {bindingInfo.actionName}. Sprawd� nazw� akcji w InputActionAsset.");
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
                // Ustaw nowy binding
                action.ApplyBindingOverride(bindingInfo.bindingIndex, operation.selectedControl.path);

                operation.Dispose();
                ongoingRebinding = null;

                UpdateBindingDisplay(bindingInfo);
                SaveBindings();

                // Wywo�aj zdarzenie po zmianie binding�w
                OnBindingsChanged?.Invoke();

                // W��cz akcj� po zako�czeniu
                action.Enable();
                Debug.Log($"Zako�czono rebinding dla akcji {bindingInfo.actionName}. Nowy binding: {operation.selectedControl.path}");
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                ongoingRebinding = null;
                UpdateBindingDisplay(bindingInfo);

                // W��cz akcj� po anulowaniu
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
            Debug.Log($"Zaktualizowano wy�wietlanie bindingu dla akcji {bindingInfo.actionName}: {bindingStr}");
        }
        else
        {
            Debug.LogWarning($"Nie mo�na zaktualizowa� wy�wietlanego bindingu dla akcji {bindingInfo.actionName}. Sprawd� bindingIndex lub akcj�.");
        }
    }

    public void SaveBindings()
    {
        // Zapisz tylko bindingi dla akcji innych ni� "Look"
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
            inputActions.RemoveAllBindingOverrides(); // Usu� wszystkie domy�lne bindingi
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("[KeyRebindInput] Za�adowano zapisane bindingi");

            // Wywo�aj zdarzenie po za�adowaniu binding�w
            OnBindingsChanged?.Invoke();
        }

        // Upewnij si�, �e akcja "Look" ma domy�lne bindingi
        var lookAction = inputActions.FindAction("Player/Look");
        if (lookAction != null)
        {
            lookAction.RemoveAllBindingOverrides(); // Resetuj bindingi dla "Look" do domy�lnych
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

        // Wywo�aj zdarzenie po resecie binding�w
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