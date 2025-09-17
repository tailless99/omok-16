using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayController : MonoBehaviour
{
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI turnNumber;
    
    private BlockController _blockController;

    public int[,] boardState = new int[15, 15];

    private List<TurnState> turnHistory = new List<TurnState>();

    private int currentIndex = 0;

    //  List<TurnState>
    // public int[,] boardState;
    // public int turnNumber;
    // public int currPlayer;
    // public int lastMoveRow;
    // public int lastMoveCol;
    
    // 이전 버튼 클릭시 List의 마지막 index를 바탕으로 보드의 row, col의 playerType을 0으로 변경
    // 다음 버튼 클릭시 List의 index를 바탕으로 i % 2 하여 1 일경우 row, col의 playerType을 1로 0일 경우 2로 변경

    void Awake()
    {
        _blockController = FindFirstObjectByType<BlockController>();
    }

    public void GetReplayData(List<TurnState> turnData)
    {
        turnHistory = new List<TurnState>(turnData);
        currentIndex = turnHistory.Count;
        
        ApplyBoard();
        UpdateUI();
    }

    // 이전 수로 되돌리기
    public void OnPrevButtonClicked()
    {
        if (currentIndex <= 0) return;

        currentIndex--; // 포인터 이동
        var lastTurn = turnHistory[currentIndex];

        boardState[lastTurn.row, lastTurn.col] = 0;
        _blockController.RemoveMarker(lastTurn.row, lastTurn.col); // 돌 제거 (별도 구현 필요)

        UpdateUI();
    }

    // 다음 수로 진행하기
    public void OnNextButtonClicked()
    {
        if (currentIndex >= turnHistory.Count) return;

        var nextTurn = turnHistory[currentIndex];
        boardState[nextTurn.row, nextTurn.col] = nextTurn.currPlayer;

        if (nextTurn.currPlayer == 1)
            _blockController.PlaceMarker(Block.MarkerType.blackMarker, nextTurn.row, nextTurn.col);
        else
            _blockController.PlaceMarker(Block.MarkerType.whiteMarker, nextTurn.row, nextTurn.col);

        currentIndex++;

        UpdateUI();
    }

    // currentIndex 기준으로 보드 재구성
    private void ApplyBoard()
    {
        // 보드 초기화
        for (int r = 0; r < 15; r++)
        for (int c = 0; c < 15; c++)
            boardState[r, c] = 0;

        _blockController.ClearMarkers(); // 모든 돌 제거 (별도 구현 필요)

        for (int i = 0; i < currentIndex; i++)
        {
            var t = turnHistory[i];
            boardState[t.row, t.col] = t.currPlayer;

            if (t.currPlayer == 1)
                _blockController.PlaceMarker(Block.MarkerType.blackMarker, t.row, t.col);
            else
                _blockController.PlaceMarker(Block.MarkerType.whiteMarker, t.row, t.col);
        }
    }

    private void UpdateUI()
    {
        turnNumber.text = $"{currentIndex} / {turnHistory.Count}";
    }
}
