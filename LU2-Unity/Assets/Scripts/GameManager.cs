using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;  // Import this for EventTrigger

public class GameManager : MonoBehaviour
{
    public GameObject Rock_Play, Paper_Play, Scissors_Play;
    public GameObject Rock_Opp, Paper_Opp, Scissors_Opp;
    public TMP_Text ResultText;
    public GameObject ResetButton;

    private string playerChoice, computerChoice;
    private readonly string[] choices = { "rock", "paper", "scissors" };

    void Start()
    {
        // Start the game
        ResetGame();

        AddHoverEvents(Rock_Play);
        AddHoverEvents(Paper_Play);
        AddHoverEvents(Scissors_Play);
    }

    public void Play(string choice)
    {
        // Play the game
        playerChoice = choice;
        computerChoice = choices[Random.Range(0, choices.Length)];

        UpdateChoices(true, playerChoice);
        UpdateChoices(false, computerChoice);

        DetermineWinner();
        SetButtonsActive(false);
        ResetButton.SetActive(true);
    }

    private void UpdateChoices(bool isPlayer, string choice)
    {
        // Update the choices
        HideAllChoices(isPlayer);
        GetChoiceObject(isPlayer, choice).SetActive(true);
    }

    private GameObject GetChoiceObject(bool isPlayer, string choice)
    {
        // Get the choice object
        if (choice == "rock") return isPlayer ? Rock_Play : Rock_Opp;
        if (choice == "paper") return isPlayer ? Paper_Play : Paper_Opp;
        return isPlayer ? Scissors_Play : Scissors_Opp;
    }

    private void HideAllChoices(bool isPlayer)
    {
        // Hide all choices
        if (isPlayer)
        {
            Rock_Play.SetActive(false);
            Paper_Play.SetActive(false);
            Scissors_Play.SetActive(false);
        }
        else
        {
            Rock_Opp.SetActive(false);
            Paper_Opp.SetActive(false);
            Scissors_Opp.SetActive(false);
        }
    }

    private void DetermineWinner()
    {
        // Determine the winner
        if (playerChoice == computerChoice)
            ResultText.text = "Gelijkspel!";
        else if ((playerChoice == "rock" && computerChoice == "scissors") ||
                 (playerChoice == "paper" && computerChoice == "rock") ||
                 (playerChoice == "scissors" && computerChoice == "paper"))
            ResultText.text = "Jij wint!";
        else
            ResultText.text = "Computer wint!";
    }

    public void ResetGame()
    {
        // Reset the game
        Rock_Play.SetActive(true);
        Paper_Play.SetActive(true);
        Scissors_Play.SetActive(true);
        Rock_Opp.SetActive(true);
        Paper_Opp.SetActive(true);
        Scissors_Opp.SetActive(true);

        ResultText.text = "";
        SetButtonsActive(true);
        ResetButton.SetActive(false);
    }

    private void SetButtonsActive(bool isActive)
    {
        // Set the buttons to interactable state
        Rock_Play.GetComponent<Button>().interactable = isActive;
        Paper_Play.GetComponent<Button>().interactable = isActive;
        Scissors_Play.GetComponent<Button>().interactable = isActive;
    }


    // Add hover events to the buttons
    private void AddHoverEvents(GameObject button)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();

        if (eventTrigger == null)
        {
            eventTrigger = button.AddComponent<EventTrigger>();
        }

        // Add PointerEnter and PointerExit events
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => OnHoverEnter(button));
        eventTrigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => OnHoverExit(button));
        eventTrigger.triggers.Add(entryExit);
    }

    // Hover events
    private void OnHoverEnter(GameObject button)
    {
        // Make the button slightly bigger when hovered
        button.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
    }

    private void OnHoverExit(GameObject button)
    {
        // Reset the button size when not hovered
        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
