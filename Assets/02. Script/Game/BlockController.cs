using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;

    public delegate void OnBlockClicked(int row, int col);
    public OnBlockClicked OnBlockClickedDelegate;


    // 1. 모든 블록을 초기화
    public void InitBlocks() {
        for (int i = 0; i < blocks.Length; i++)
            blocks[i].InitMarker(i, (blockIndex) => {
                // UI 사운드 추가
                SoundManager.Instance.PlayUI(SoundType.UI_Set_Baduk);
                // 특정 Block이 클릭 된 상태에 대한 처리
                var row = blockIndex / Constants.BlockColumnCount;
                var col = blockIndex % Constants.BlockColumnCount;
                
                OnBlockClickedDelegate?.Invoke(row, col);
            });
    }

    // 2. 특정 Block에 마커 표시
    public void PlaceMarker(Block.MarkerType markerType, int row, int col, int turnNumber) {
        // row, col >> index 변환
        var blockIndex = row * Constants.BlockColumnCount + col;
        
        blocks[blockIndex].Setmarker(markerType);
        blocks[blockIndex].SetTurnNumber(turnNumber);
    }

    // 3. 특정 Block의 배경색을 설정
    public void SetBlockColor() {
        // TODO : 게임 로직이 완성되면 구현
    }
    
    // 4. 모든 마커 초기화
    public void ClearMarkers()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].Setmarker(Block.MarkerType.None);
            blocks[i].SetTurnNumber(0);
        }
    }
    
    // 5. 특정 마커 초기화
    public void RemoveMarker(int row, int col)
    {
        var blockIndex = row * Constants.BlockColumnCount + col;
        blocks[blockIndex].Setmarker(Block.MarkerType.None);
        blocks[blockIndex].SetTurnNumber(0);
    }
}
