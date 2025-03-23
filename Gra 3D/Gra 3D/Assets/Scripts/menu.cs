using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypingEffectWithTimedScroll : MonoBehaviour
{
    public Text textComponent; // Referencja do komponentu tekstowego
    public string fullText;    // Tekst do wyświetlenia
    private float typingSpeed = 0.005f; // Szybkość wypisywania liter (w sekundach)
    public ScrollRect scrollRect; // Referencja do Scroll Rect w Canvas
    private float scrollSpeed = 0.01f; // Prędkość przewijania scrolla (im mniejsza wartość, tym wolniej)
    private float scrollDelay = 0.5f; // Odstęp czasu między kolejnymi przewinięciami
    public GameObject startButton; // Referencja do przycisku "Start"

    private void Start()
    {
        startButton.SetActive(false); // Ukryj przycisk na początku
        startButton.GetComponent<Button>().onClick.AddListener(LoadGameScene); // Dodanie obsługi kliknięcia
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textComponent.text = ""; // Ustawienie tekstu na pusty
        float timer = 0f; // Timer do przewijania

        foreach (char letter in fullText)
        {
            textComponent.text += letter; // Dodawanie litery do tekstu

            // Aktualizacja scrolla w regularnych odstępach czasu
            timer += typingSpeed;
            if (scrollRect != null && timer >= scrollDelay)
            {
                timer = 0f; // Reset timer
                StartCoroutine(SmoothScroll());
            }

            yield return new WaitForSeconds(typingSpeed); // Odstęp między literami
        }

        // Po zakończeniu wypisywania tekstu, czekamy na zakończenie przewijania
        if (scrollRect != null)
        {
            yield return StartCoroutine(SmoothScrollToEnd());
        }

        // Po zakończeniu przewijania pokaż przycisk
        startButton.SetActive(true);
    }

    IEnumerator SmoothScroll()
    {
        float targetPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollSpeed, 0f, 1f); // Obliczenie pozycji docelowej
        float duration = 0.5f; // Czas animacji przewijania
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPosition; // Ustawienie pozycji docelowej
    }

    IEnumerator SmoothScrollToEnd()
    {
        // Upewnij się, że scroll ustawi się na sam dół na końcu
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f; // Ustaw scroll na dole
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Game"); // Załaduj scenę "Game"
    }
}
