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
            Debug.Log("Pendrive wy³¹czony na starcie.");
        }
        else
        {
            Debug.LogError("Pendrive nie jest przypisany w Inspectorze!");
        }

        playerInteraction = GameObject.FindGameObjectWithTag("player").GetComponent<Interaction>();
        if (playerInteraction == null)
        {
            Debug.LogError("Nie znaleziono komponentu Interaction na graczu!");
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
            questionText = "Jakie jest stolic¹ Polski?",
            answers = new string[] { "Warszawa", "Kraków", "Gdañsk", "Wroc³aw" },
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
            questionText = "Jak nazywa siê najwiêksza planeta Uk³adu S³onecznego?",
            answers = new string[] { "Mars", "Jowisz", "Wenus", "Saturn" },
            correctAnswerIndex = 1
        };
        questions[3] = new Question
        {
            questionText = "Kto napisa³ 'Pana Tadeusza'?",
            answers = new string[] { "Mickiewicz", "S³owacki", "Norwid", "Prus" },
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
            questionText = "W którym roku odkryto Amerykê?",
            answers = new string[] { "1492", "1453", "1519", "1607" },
            correctAnswerIndex = 0
        };
        questions[7] = new Question
        {
            questionText = "Jak nazywa siê stolica Francji?",
            answers = new string[] { "Pary¿", "Londyn", "Berlin", "Madryt" },
            correctAnswerIndex = 0
        };
        questions[8] = new Question
        {
            questionText = "Ile boków ma trójk¹t?",
            answers = new string[] { "2", "3", "4", "5" },
            correctAnswerIndex = 1
        };
        questions[9] = new Question
        {
            questionText = "Kto jest autorem teorii wzglêdnoœci?",
            answers = new string[] { "Newton", "Einstein", "Tesla", "Galileusz" },
            correctAnswerIndex = 1
        };
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        if (availableQuestionIndices.Count > 0)
        {
            currentQuestionIndex = availableQuestionIndices[Random.Range(0, availableQuestionIndices.Count)];
            DisplayQuestion();
        }
        else
        {
            panel.SetActive(false);
            Debug.Log("Brak dostêpnych pytañ!");
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
                Debug.Log("Wszystkie pytania zosta³y u¿yte!");
            }
        }
        else
        {
            if (playerInteraction != null)
            {
                playerInteraction.TakeDamage(10);
            }

            DisplayQuestion();
        }
    }

    void DisplayQuestion()
    {
        questionTextUI.text = questions[currentQuestionIndex].questionText;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = questions[currentQuestionIndex].answers[i];
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
                Debug.Log("Fizyka Demona w³¹czona!");
            }
        }
    }
}