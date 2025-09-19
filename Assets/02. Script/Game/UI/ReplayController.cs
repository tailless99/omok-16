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
        SetupButtons(false);
    }

    public void SetupButtons(bool isON)
    {
        prevButton.gameObject.SetActive(isON);
        nextButton.gameObject.SetActive(isON);
    }

    public void GetReplayData(List<TurnState> turnData)
    {
        turnHistory = new List<TurnState>(turnData);
        currentIndex = turnHistory.Count;
        
        ApplyBoard();
        UpdateUI(currentIndex, turnHistory.Count);
    }

    // 이전 수로 되돌리기
    public void OnPrevButtonClicked()
    {
        if (currentIndex <= 0) return;

        currentIndex--; // 포인터 이동
        var lastTurn = turnHistory[currentIndex];

        boardState[lastTurn.row, lastTurn.col] = 0;
        _blockController.RemoveMarker(lastTurn.row, lastTurn.col); // 돌 제거 (별도 구현 필요)

        UpdateUI(currentIndex, turnHistory.Count);
    }

    // 다음 수로 진행하기
    public void OnNextButtonClicked()
    {
        if (currentIndex >= turnHistory.Count) return;

        var nextTurn = turnHistory[currentIndex];
        boardState[nextTurn.row, nextTurn.col] = nextTurn.currPlayer;
        currentIndex++;

        if (nextTurn.currPlayer == 1)
            _blockController.PlaceMarker(Block.MarkerType.blackMarker, nextTurn.row, nextTurn.col, currentIndex);
        else
            _blockController.PlaceMarker(Block.MarkerType.whiteMarker, nextTurn.row, nextTurn.col, currentIndex);


        UpdateUI(currentIndex, turnHistory.Count);
    }

    // currentIndex 기준으로 보드 재구성
    private void ApplyBoard()
    {
        // 먼저 전체 보드 초기화
        _blockController.ClearMarkers();

        foreach (var replay in turnHistory)
        {
            // 플레이어 돌 표시
            var markerType = replay.currPlayer == 1 ? Block.MarkerType.blackMarker : Block.MarkerType.whiteMarker;
            _blockController.PlaceMarker(markerType, replay.row, replay.col, replay.turnNumber);
        }
    }

    public void UpdateUI(int currentTurn, int totalTurn)
    {
        turnNumber.text = $"{currentTurn} / {totalTurn}";
    }
}
