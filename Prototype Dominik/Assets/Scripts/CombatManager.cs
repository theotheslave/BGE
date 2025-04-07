using UnityEngine;
using UnityEngine.UI;

public enum ActionType { Attack, Defend }

public class CombatManager : MonoBehaviour
{
    public int playerHealth = 50;
    public int enemyHealth = 40;
    public int maxPlayerHealth = 50;
    public int maxEnemyHealth = 40;

    public Slider playerHealthBar;
    public Slider enemyHealthBar;

    public Text turnText; // UI Text to display turn feedback
    public Text enemyIntentText; // UI Text to display enemy's next action
    public float feedbackDuration = 1.5f; // How long to show turn text

    public PuzzleManager puzzleManager;

    private ActionType nextEnemyAction;

    void Start()
    {
        playerHealth = maxPlayerHealth;
        enemyHealth = maxEnemyHealth;
        UpdateUI();
        ChooseNextEnemyAction();
    }

    public void ResolveTurn(ActionType playerAction)
    {
        ActionType enemyAction = nextEnemyAction;

        int damageToEnemy = 0;
        int damageToPlayer = 0;

        int basePower = puzzleManager.CalculateTotalValue(playerAction);
        int universalPower = puzzleManager.CalculateUniversalValue();
        int totalPower = basePower + universalPower;

        if (playerAction == ActionType.Attack && enemyAction == ActionType.Defend)
        {
            damageToEnemy = Mathf.Max(0, totalPower - Random.Range(10, 30));
            enemyHealth -= damageToEnemy;
        }
        else if (playerAction == ActionType.Defend && enemyAction == ActionType.Attack)
        {
            damageToPlayer = Mathf.Max(0, Random.Range(20, 40) - totalPower);
            playerHealth -= damageToPlayer;
        }
        else if (playerAction == ActionType.Attack && enemyAction == ActionType.Attack)
        {
            damageToPlayer = Random.Range(20, 40);
            damageToEnemy = totalPower;
            playerHealth -= damageToPlayer;
            enemyHealth -= damageToEnemy;
        }

        ShowTurnFeedback($"Player chose {FormatAction(playerAction)} - Enemy chose {FormatAction(enemyAction)}\nYou dealt {damageToEnemy} dmg, took {damageToPlayer} dmg.");

        UpdateUI();
        puzzleManager.ResetPuzzle();
        ChooseNextEnemyAction();
    }

    public void PlayerAttack()
    {
        ResolveTurn(ActionType.Attack);
    }

    public void PlayerDefend()
    {
        ResolveTurn(ActionType.Defend);
    }

    void ChooseNextEnemyAction()
    {
        nextEnemyAction = (ActionType)Random.Range(0, 2);

        if (enemyIntentText != null)
        {
            string icon = FormatAction(nextEnemyAction);
            enemyIntentText.text = $"Enemy Intent: {icon}";
            enemyIntentText.gameObject.SetActive(true);
        }
    }

    void UpdateUI()
    {
        Debug.Log($"Player HP: {playerHealth}, Enemy HP: {enemyHealth}");

        if (playerHealthBar != null)
            playerHealthBar.value = (float)playerHealth / maxPlayerHealth;

        if (enemyHealthBar != null)
            enemyHealthBar.value = (float)enemyHealth / maxEnemyHealth;
    }

    void ShowTurnFeedback(string message)
    {
        if (turnText == null) return;

        turnText.text = message;
        turnText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideTurnFeedback));
        Invoke(nameof(HideTurnFeedback), feedbackDuration);
    }

    void HideTurnFeedback()
    {
        if (turnText != null)
            turnText.gameObject.SetActive(false);
    }

    string FormatAction(ActionType action)
    {
        return action == ActionType.Attack ? "Attack" : "Defend";
    }
}
