using UnityEngine.UI;
using UnityEngine;

public class LevelsSwitchManager : MonoBehaviour
{
    [SerializeField] private int countOfLevels;

    [Header("Контейнеры")]
    [SerializeField] private Transform visualGridContainer;
    [SerializeField] private Transform workingGridContainer;
    [SerializeField] private Transform maskGridContainer;

    [Header ("Префабы")]
    [SerializeField] private GameObject visualGridPrefab;
    [SerializeField] private GameObject workingGridPrefab;
    [SerializeField] private GameObject maskGridPrefab;
    [SerializeField] private GameObject visualCellPrefab;
    [SerializeField] private GameObject workingCellPrefab;

    [Header("Кнопки")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Позиции")]
    [SerializeField] private float targetPosition;
    [SerializeField] private float nowPosition;
    [SerializeField] private float step;

    private int index;

    private void Start()
    {
        int num = countOfLevels / 10;

        for (int i=0; i < num; i++) {
            Instantiate(maskGridPrefab, maskGridContainer);

            var visualGrid = Instantiate(visualGridPrefab, visualGridContainer);
            var workingGrid = Instantiate(workingGridPrefab, workingGridContainer);
            
            for (int j=0; j < 10; j++) {
                var visualCell = Instantiate(visualCellPrefab, visualGrid.transform).GetComponent<CellVisual>();
                var workingCell = Instantiate(workingCellPrefab, workingGrid.transform).GetComponent<CellButton>();

                workingCell.button.targetGraphic = visualCell.number;
                workingCell.blur = visualCell.blur;

                visualCell.lockImage.gameObject.SetActive(false);
                visualCell.number.gameObject.SetActive(true);
                visualCell.number.text = (i * 10 + j + 1).ToString();
                visualCell.blur.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        index = 0;
        nowPosition = targetPosition = visualGridContainer.localPosition.x;

        UpdateButtons();
    }

    private void Update()
    {
        if (Mathf.Abs(targetPosition - nowPosition) >= step) {
            if (nowPosition < targetPosition) {
                visualGridContainer.localPosition = new Vector2(visualGridContainer.localPosition.x + step, visualGridContainer.localPosition.y);
                workingGridContainer.localPosition = new Vector2(workingGridContainer.localPosition.x + step, workingGridContainer.localPosition.y);
                maskGridContainer.localPosition = new Vector2(maskGridContainer.localPosition.x + step, maskGridContainer.localPosition.y);
            } else {
                visualGridContainer.localPosition = new Vector2(visualGridContainer.localPosition.x - step, visualGridContainer.localPosition.y);
                workingGridContainer.localPosition = new Vector2(workingGridContainer.localPosition.x - step, workingGridContainer.localPosition.y);
                maskGridContainer.localPosition = new Vector2(maskGridContainer.localPosition.x - step, maskGridContainer.localPosition.y);
            }
            nowPosition = visualGridContainer.localPosition.x;
        }

        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && index > 0)
            PreviousMenu();
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && index < countOfLevels / 10 - 1)
            NextMenu();
    }

    public void NextMenu()
    {
        index += 1;
        targetPosition -= 923.76f;
        UpdateButtons();
    }
    public void PreviousMenu()
    {
        index -= 1;
        targetPosition += 923.76f;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        rightButton.interactable = index < countOfLevels / 10 - 1;
        leftButton.interactable = index > 0;
    }
}
