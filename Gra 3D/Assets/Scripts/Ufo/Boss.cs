using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Boss : MonoBehaviour
{
    public GameObject panel;
    public GameObject pendrive;
    public Button[] answerButtons;
    public TMP_Text questionTextUI;
    public Slider bossHealthSlider;
    public Slider playerHealthSlider; // Slider na panelu walki
    private int bossHealth = 100;
    private Interaction playerInteraction;
    private int currentQuestionIndex;
    private List<int> availableQuestionIndices;

    private Rigidbody demonRigidbody;

    [System.Serializable]
    public struct Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    public Question[] questions = new Question[10];

    void Start()
    {
        panel.SetActive(false);
        if (pendrive != null)
        {
            pendrive.SetActive(false);
            Debug.Log("Pendrive wyłączony na starcie.");
        }
        else
        {
            Debug.LogError("Pendrive nie jest przypisany w Inspectorze!");
        }

        playerInteraction = Interaction.Instance;
        if (playerInteraction == null)
        {
            Debug.LogError("Nie znaleziono komponentu Interaction!");
        }

        availableQuestionIndices = new List<int>();
        for (int i = 0; i < questions.Length; i++)
        {
            availableQuestionIndices.Add(i);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        InitializeQuestions();

        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = 100;
            bossHealthSlider.value = bossHealth;
        }
        else
        {
            Debug.LogError("BossHealthSlider nie jest przypisany!");
        }

        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = 100;
            playerHealthSlider.value = playerInteraction.playerHealth;
        }
        else
        {
            Debug.LogError("PlayerHealthSlider nie jest przypisany!");
        }

        GameObject demonObject = GameObject.FindGameObjectWithTag("Demon");
        if (demonObject != null)
        {
            demonRigidbody = demonObject.GetComponent<Rigidbody>();
            GameObject klatkaObject = GameObject.FindGameObjectWithTag("Klatka");

            if (klatkaObject != null && demonRigidbody != null)
            {
                demonRigidbody.isKinematic = true;
            }
            else
            {
                Debug.LogWarning("Nie znaleziono Klatki lub Rigidbody Demona!");
            }
        }
        else
        {
            Debug.LogWarning("Nie znaleziono Demona!");
        }
    }

    void InitializeQuestions()
    {
        questions[0] = new Question
        {
            questionText = "Jakie jest stolicą Polski?",
            answers = new string[] { "Warszawa", "Kraków", "Gdańsk", "Wrocław" },
            correctAnswerIndex = 0
        };
        questions[1] = new Question
        {
            questionText = "Ile wynosi 2 + 2?",
            answers = new string[] { "3", "4", "5", "6" },
            correctAnswerIndex = 1
        };
        questions[2] = new Question
        {
            questionText = "Jak nazywa się największa planeta Układu Słonecznego?",
            answers = new string[] { "Mars", "Jowisz", "Wenus", "Saturn" },
            correctAnswerIndex = 1
        };
        questions[3] = new Question
        {
            questionText = "Kto napisał 'Pana Tadeusza'?",
            answers = new string[] { "Mickiewicz", "Słowacki", "Norwid", "Prus" },
            correctAnswerIndex = 0
        };
        questions[4] = new Question
        {
            questionText = "Ile wynosi pierwiastek kwadratowy z 16?",
            answers = new string[] { "2", "3", "4", "5" },
            correctAnswerIndex = 2
        };
        questions[5] = new Question
        {
            questionText = "Jakie jest chemiczne oznaczenie wody?",
            answers = new string[] { "H2O", "CO2", "O2", "N2" },
            correctAnswerIndex = 0
        };
        questions[6] = new Question
        {
            questionText = "W którym roku odkryto Amerykę?",
            answers = new string[] { "1492", "1453", "1519", "1607" },
            correctAnswerIndex = 0
        };
        questions[7] = new Question
        {
            questionText = "Jak nazywa się stolica Francji?",
            answers = new string[] { "Paryż", "Londyn", "Berlin", "Madryt" },
            correctAnswerIndex = 0
        };
        questions[8] = new Question
        {
            questionText = "Ile boków ma trójkąt?",
            answers = new string[] { "2", "3", "4", "5" },
            correctAnswerIndex = 1
        };
        questions[9] = new Question
        {
            questionText = "Kto jest autorem teorii względności?",
            answers = new string[] { "Newton", "Einstein", "Tesla", "Galileusz" },
            correctAnswerIndex = 1
        };

        // Weryfikacja inicjalizacji
        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i].answers == null || questions[i].answers.Length != 4)
            {
                Debug.LogError($"Pytanie {i} nie jest poprawnie zdefiniowane!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        if (availableQuestionIndices.Count > 0)
        {
            currentQuestionIndex = availableQuestionIndices[Random.Range(0, availableQuestionIndices.Count)];
            Debug.Log($"Wybrane pytanie: {currentQuestionIndex}, Liczba odpowiedzi: {questions[currentQuestionIndex].answers.Length}");
            DisplayQuestion();
        }
        else
        {
            panel.SetActive(false);
            Debug.Log("Brak dostępnych pytań!");
            return;
        }

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player")) return;

        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void CheckAnswer(int selectedIndex)
    {
        if (selectedIndex == questions[currentQuestionIndex].correctAnswerIndex)
        {
            TakeBossDamage(10);
            availableQuestionIndices.Remove(currentQuestionIndex);

            if (availableQuestionIndices.Count > 0)
            {
                currentQuestionIndex = availableQuestionIndices[Random.Range(0, availableQuestionIndices.Count)];
                DisplayQuestion();
            }
            else
            {
                panel.SetActive(false);
                Debug.Log("Wszystkie pytania zostały użyte!");
            }
        }
        else
        {
            TakePlayerDamage(10);
            DisplayQuestion();
        }
    }

    void DisplayQuestion()
    {
        questionTextUI.text = questions[currentQuestionIndex].questionText;
        int answerCount = questions[currentQuestionIndex].answers.Length;
        for (int i = 0; i < answerButtons.Length && i < answerCount; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = questions[currentQuestionIndex].answers[i];
        }
        // Ukryj lub zablokuj dodatkowe przyciski, jeśli odpowiedzi jest mniej
        for (int i = answerCount; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }
    }

    public void TakeBossDamage(int damage)
    {
        bossHealth -= damage;
        bossHealth = Mathf.Clamp(bossHealth, 0, 100);

        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = bossHealth;
        }

        if (bossHealth <= 0)
        {
            Debug.Log("Boss pokonany!");
            Destroy(gameObject);
            panel.SetActive(false);

            GameObject klatka = GameObject.FindGameObjectWithTag("Klatka");
            if (klatka != null)
            {
                Destroy(klatka);
                Debug.Log("Klatka zniszczona!");
            }
            else
            {
                Debug.LogWarning("Nie znaleziono obiektu z tagiem 'Klatka'");
            }

            if (pendrive != null)
            {
                pendrive.SetActive(true);
                Debug.Log("Pendrive aktywowany!");
            }
            else
            {
                Debug.LogError("Pendrive nie jest przypisany!");
            }

            if (demonRigidbody != null)
            {
                demonRigidbody.isKinematic = false;
                Debug.Log("Fizyka Demona włączona!");
            }
        }
    }

    private void TakePlayerDamage(int damage)
    {
        if (playerInteraction != null)
        {
            playerInteraction.TakeDamage(damage); // Aktualizacja HP w Interaction
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = playerInteraction.playerHealth; // Synchronizacja slidera na panelu
            }
        }

        if (playerInteraction.playerHealth <= 0)
        {
            Debug.Log("Gracz pokonany!");
            panel.SetActive(false);
            // Logika końca gry jest obsługiwana przez Interaction
        }
    }
}