using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemyNameText;

    private void Start() {
        GameManager.Instance.GetGameType(out Constants.GameType gameType);
        var enemyName = gameType == Constants.GameType.SinglePlay ? "AI" : "Player";
        enemyNameText.text = enemyName;
    }
}
