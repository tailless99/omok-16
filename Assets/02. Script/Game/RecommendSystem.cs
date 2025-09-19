using System.Threading.Tasks;
using UnityEngine;

public class RecommendSystem : MonoBehaviour
{
    public void OnClickButton()
    {
        _ = OnClickRecommendButton();
    }
    public async Task OnClickRecommendButton()
    {
        GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.ATurn);
        // AI 처리
        var gameLogic = GameManager.Instance.GetGameLogic();
        var board = gameLogic.GetBoard();
        if (gameLogic == null) Debug.LogError("gameLogic is not set!");
        // GetBestMove 연산을 백그라운드 스레드에서 실행, await 때문에 GetBestMove 함수가 완료 될 때까지 대기
        (int row, int col)? result = await OmokAI.GetBestMove(board, 15);


        if (result.HasValue)
        {
            Debug.Log($"AI 수: row={result.Value.row}, col={result.Value.col}");
        }
        else
        {
            gameLogic.EndGame(GameLogic.GameResult.Draw);
        }
    }
}
