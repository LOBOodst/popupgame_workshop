using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MathPopup : MonoBehaviour
{
    [Header("Composants UI")]
    public Text mathProblemText;
    public InputField answerInput;
    public Button submitButton;

    [Header("Paramètres Mathématiques")]
    public int pointsPerCorrectAnswer = 10;
    public bool closeOnCorrectAnswer = true;

    private PopupManager popupManager;
    private MathProblem currentProblem;
    private int score = 0;

    [System.Serializable]
    public struct MathProblem
    {
        public string problem;
        public int correctAnswer;

        public MathProblem(string prob, int answer)
        {
            problem = prob;
            correctAnswer = answer;
        }
    }

    private List<MathProblem> allMathProblems = new List<MathProblem>
    {
        new MathProblem("9 + 6 = ?", 15),
        new MathProblem("8 - 4 = ?", 4),
        new MathProblem("3 x 4 = ?", 12),
        new MathProblem("7 + 3 = ?", 10),
        new MathProblem("5 - 2 = ?", 3),
        new MathProblem("4 x 2 = ?", 8),
        new MathProblem("9 - 6 = ?", 3),
        new MathProblem("3 + 2 = ?", 5),
        new MathProblem("1 + 8 = ?", 9),
        new MathProblem("2 x 3 = ?", 6)
    };

    void Start()
    {
        popupManager = FindObjectOfType<PopupManager>();

        SelectRandomProblem();
        DisplayProblem();

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckAnswer);
        }

        if (answerInput != null)
        {
            answerInput.onValueChanged.AddListener(OnAnswerChanged);
        }
    }

    void SelectRandomProblem()
    {
        int randomIndex = Random.Range(0, allMathProblems.Count);
        currentProblem = allMathProblems[randomIndex];
    }

    void DisplayProblem()
    {
        mathProblemText.text = currentProblem.problem;
        answerInput.text = "";
        answerInput.Select();
        answerInput.ActivateInputField();
    }

    void OnAnswerChanged(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (int.TryParse(text.Trim(), out int playerAnswer))
        {
            if (playerAnswer == currentProblem.correctAnswer && closeOnCorrectAnswer)
            {
                Debug.Log("Réponse correcte ! Fermeture automatique...");
                score += pointsPerCorrectAnswer;
                Invoke("CompleteMathChallenge", 0.5f);
            }
        }
    }

    void CheckAnswer()
    {
        if (answerInput == null || string.IsNullOrEmpty(answerInput.text)) return;

        if (int.TryParse(answerInput.text.Trim(), out int playerAnswer))
        {
            if (playerAnswer == currentProblem.correctAnswer)
            {
                score += pointsPerCorrectAnswer;
                Debug.Log("Réponse correcte ! +" + pointsPerCorrectAnswer + " points");

                if (closeOnCorrectAnswer)
                {
                    Invoke("CompleteMathChallenge", 0.5f);
                }
                else
                {
                    CompleteMathChallenge();
                }
            }
            else
            {
                Debug.Log("Réponse incorrecte. La bonne réponse était: " + currentProblem.correctAnswer);
                answerInput.text = "";
                answerInput.Select();
                answerInput.ActivateInputField();
            }
        }
        else
        {
            Debug.Log("Veuillez entrer un nombre valide !");
            answerInput.text = "";
            answerInput.Select();
            answerInput.ActivateInputField();
        }
    }

    void CompleteMathChallenge()
    {
        Debug.Log("Défi mathématique réussi ! Score: " + score + " points");

        if (popupManager != null)
        {
            int popupsToRemove = score / pointsPerCorrectAnswer;

            if (popupsToRemove > 0)
            {
                popupManager.RemoveRandomPopups(popupsToRemove);
                Debug.Log(popupsToRemove + " popups supprimés grâce à vos compétences mathématiques !");
            }
            else
            {
                popupManager.SpawnExtraPopups();
                Debug.Log("Aucun popup supprimé - de nouveaux popups apparaissent !");
            }
        }

        ClosePopup();
    }

    void ClosePopup()
    {
        if (popupManager != null)
        {
            popupManager.MathPopupClosed();
        }
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (popupManager != null)
        {
            popupManager.MathPopupClosed();
        }
    }
}