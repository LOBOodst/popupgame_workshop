using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RamUsage : MonoBehaviour
{
    [Header("Param�tres RAM")]
    public Slider ramSlider;
    public float baseRamUsage = 0.33f; // Utilisation de base de la RAM
    public float ramPerPopup = 0.15f; // RAM utilis�e par chaque popup
    public float maxRamCapacity = 40.0f; // Capacit� maximale de RAM

    [Header("R�f�rences UI")]
    [SerializeField] private TextMeshProUGUI ramUsageText; // Texte d'affichage de l'utilisation RAM
    [SerializeField] private Image sliderFill; // Image de remplissage du slider
    [SerializeField] private GameObject gameOverScreen; // �cran de game over

    [Header("Seuils de Couleur")]
    public float redThreshold = 32.0f; // Seuil pour la couleur rouge
    public float yellowThreshold = 16.0f; // Seuil pour la couleur jaune

    [Header("Sound")]
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float soundBoost = 1.0f;

    private float currentRamUsage; // Utilisation actuelle de la RAM
    private bool gameOverTriggered = false; // Flag pour �viter de d�clencher le game over multiple fois

    void Start()
    {
        ramSlider.maxValue = maxRamCapacity;
        UpdateRamUsage();

        // S'assurer que l'�cran de game over est d�sactiv� au d�but
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    void Update()
    {
        UpdateRamUsage();
        ramSlider.value = currentRamUsage;
        ramUsageText.text = $"Utilisation RAM: {currentRamUsage:F1}GB / {maxRamCapacity:F1}GB";

        UpdateSliderColor();

        // V�rifier si la RAM a atteint le maximum et d�clencher le game over
        if (currentRamUsage >= maxRamCapacity && !gameOverTriggered)
        {
            TriggerGameOver();
        }
    }

    // Met � jour l'utilisation de la RAM en fonction du nombre de popups
    void UpdateRamUsage()
    {
        int popupCount = FindObjectsOfType<SimplePopup>().Length +
                        FindObjectsOfType<MovingPopup>().Length +
                        FindObjectsOfType<AdminPopup>().Length +
                        FindObjectsOfType<MathPopup>().Length;

        currentRamUsage = baseRamUsage + (popupCount * ramPerPopup);
        currentRamUsage = Mathf.Min(currentRamUsage, maxRamCapacity); // Limiter � la capacit� max
    }

    // Met � jour la couleur du slider en fonction de l'utilisation de la RAM
    void UpdateSliderColor()
    {
        if (currentRamUsage >= redThreshold)
            sliderFill.color = Color.red;
        else if (currentRamUsage >= yellowThreshold)
            sliderFill.color = Color.yellow;
        else
            sliderFill.color = Color.green;
    }

    // D�clenche l'�cran de game over
    void TriggerGameOver()
    {
        gameOverTriggered = true;

        // Activer l'�cran de game over
        if (gameOverScreen != null)
        {
            audioSource.PlayOneShot(gameOverSound, soundBoost);
            gameOverScreen.SetActive(true);
        }

        // Mettre le jeu en pause (optionnel)
        Time.timeScale = 0f;

        Debug.Log("GAME OVER - RAM a atteint la capacit� maximale!");

        // Arr�ter la g�n�ration de popups (optionnel)
        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.StopAllCoroutines();
        }
    }

    // Modifie la capacit� maximale de RAM
    public void SetMaxRamCapacity(float newCapacity)
    {
        maxRamCapacity = Mathf.Max(1.0f, newCapacity);
        ramSlider.maxValue = maxRamCapacity;
        UpdateThresholds();
    }

    // Modifie la quantit� de RAM utilis�e par popup
    public void SetRamPerPopup(float newValue)
    {
        ramPerPopup = Mathf.Max(0.01f, newValue);
    }

    // Met � jour les seuils de couleur en fonction de la capacit� max
    void UpdateThresholds()
    {
        redThreshold = maxRamCapacity * 0.8f; // 80% de la capacit� max
        yellowThreshold = maxRamCapacity * 0.4f; // 40% de la capacit� max
    }

    // M�thode pour red�marrer le jeu (peut �tre appel�e par un bouton sur l'�cran de game over)
    public void RestartGame()
    {
        gameOverTriggered = false;
        Time.timeScale = 1f; // Reprendre le temps

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        // Supprimer tous les popups existants
        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ClearAllPopups();
        }

        // Red�marrer le jeu
        // (Vous devrez peut-�tre ajouter plus de logique de red�marrage ici selon votre jeu)
    }
}