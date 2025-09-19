using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    [SerializeField] private Sprite blackMarkerSprite;
    [SerializeField] private Sprite whiteMarkerSprite;
    [SerializeField] private Sprite forbiddenMarkerSprite;
    [SerializeField] private Sprite recommendSprite;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Image markerSpriteRenderer;

    public delegate void OnBlockClicked(int index);
    private OnBlockClicked _onBlockClicked;


    // 마커 타입
    public enum MarkerType
    {
        None,
        blackMarker,
        whiteMarker,
        forbiddenMarker,
        recommendMarker
    }

    // Block Index
    private int _blockIndex;

    // Block의 색 변경을 위한 Block의 Sprite Renderer
    private SpriteRenderer _spriteRenderer;
    private Color _defaultBlockColor;


    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultBlockColor = _spriteRenderer.color;
    }


    // 1. 초기화
    public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked) {
        _blockIndex = blockIndex;
        Setmarker(MarkerType.None);
        SetBlockColor(_defaultBlockColor);
        _onBlockClicked = onBlockClicked;
    }

    // 2. 마커 설정
    public void Setmarker(MarkerType type) {
        switch (type) {
            case MarkerType.None:
                markerSpriteRenderer.sprite = null;
                markerSpriteRenderer.color = new Color(1, 1, 1, 0);
                break;
            case MarkerType.blackMarker:
                markerSpriteRenderer.sprite = blackMarkerSprite;
                markerSpriteRenderer.color = new Color(1, 1, 1, 1);
                break;
            case MarkerType.whiteMarker:
                markerSpriteRenderer.sprite = whiteMarkerSprite;
                markerSpriteRenderer.color = new Color(1, 1, 1, 1);
                break;
            case MarkerType.forbiddenMarker:
                markerSpriteRenderer.sprite = forbiddenMarkerSprite;
                markerSpriteRenderer.color = new Color(1, 1, 1, 1);
                break;
            case MarkerType.recommendMarker:
                markerSpriteRenderer.sprite = recommendSprite;
                markerSpriteRenderer.color = new Color(1, 1, 1, 1);
                break;
        }
    }

    // 3. 컬러 설정
    public void SetBlockColor(Color color) {
        _spriteRenderer.color = color;
    }

    // 4. 블럭 터치
    public void OnMouseClick() {
        _onBlockClicked?.Invoke(_blockIndex);
    }
    
    // 5. 기보시스템 번호 새기기
    public void SetTurnNumber(int turn)
    {
        if (turn > 0)
        {
            turnNumberText.text = turn.ToString();
            turnNumberText.gameObject.SetActive(true);
        }
        else
        {
            turnNumberText.gameObject.SetActive(false);
        }
    }
}