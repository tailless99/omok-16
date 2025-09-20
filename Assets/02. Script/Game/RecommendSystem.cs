using System.Threading.Tasks;
using TMPro;
using UnityEngine;

//작성자: 이명호
//용도: 자리 추천 시스템
public class RecommendSystem : MonoBehaviour
{
    public BlockController blockController;
    // 추천 위치 저장용 변수
    private int? recommendedRow = null;
    private int? recommendedCol = null;
    [SerializeField] private GameObject lodingPanel;
    [SerializeField]  private TextMeshProUGUI recommendText;
    /// <summary>
    /// 추천 버튼 클릭 시 호출
    /// </summary>
    public void OnClickButton()
    {
        _ = OnClickRecommendButton();

    }


    /// <summary>
    /// AI 추천 위치 계산 및 표시
    /// </summary>
    public async Task OnClickRecommendButton()
    {
        GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.ATurn);

        var gameLogic = GameManager.Instance.GetGameLogic();
        var basePlayerState = gameLogic._currentPlayerState;

        if (basePlayerState == gameLogic.firstPlayerState) // A플레이어일때
        {
            recommendText.text = string.Empty;
            lodingPanel.SetActive(true);
            var board = gameLogic.GetBoard();
            if (gameLogic == null)
            {
                Debug.LogError("gameLogic is not set!");
                return;
            }

            // 비동기적으로 추천 위치 계산
            (int row, int col)? result = await OmokAI.GetBestMove(board, 15);

            if (result.HasValue) //추천 자리가 있을 경우
            {
                recommendedRow = result.Value.row;
                recommendedCol = result.Value.col;
                gameLogic.GetCheckForbiddenMarkersBoard(out int forbiddenRow, out int forbiddenCol); //AI가 금수 자리를 추천할 경우
                if (recommendedRow == forbiddenRow && recommendedCol == forbiddenCol)
                {
                    Debug.Log("추천 자리는 금수 자리입니다.");
                    lodingPanel.SetActive(false);
                    recommendText.text = "금수 자리";
                }
                else
                {
                    lodingPanel.SetActive(false);
                    recommendText.text = "자리 추천";
                    blockController.ShowRecommend(recommendedRow.Value, recommendedCol.Value);
                }
            }
            else
            {
                lodingPanel.SetActive(false);
                recommendText.text = "자리 추천";
                gameLogic.EndGame(GameLogic.GameResult.Draw);
            }
            
             gameLogic.SetCheckForbiddenMarkersBoard(0,0);
        }
    }

    /// <summary>
    /// 이전 추천 마커 제거
    /// </summary>
    public void ClearRecommendMarker()
    {
        if (recommendedRow.HasValue && recommendedCol.HasValue)
        {
            blockController.RemoveMarker(recommendedRow.Value, recommendedCol.Value);
            recommendedRow = null;
            recommendedCol = null;
        }
    }
}
