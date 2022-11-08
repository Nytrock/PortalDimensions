using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelsSwitchManager : MonoBehaviour
{
    [Header("Настройки мира")]
    [SerializeField] private World showingWorld;
    private int countMenus;
    [SerializeField] private TextMeshProUGUI header;


    [Header("Контейнеры")]
    [SerializeField] private Transform visualGridContainer;
    [SerializeField] private Transform workingGridContainer;
    [SerializeField] private Transform maskGridContainer;
    [SerializeField] private Transform instantGridContainer;

    [Header ("Префабы")]
    [SerializeField] private GameObject visualGridPrefab;
    [SerializeField] private GameObject workingGridPrefab;
    [SerializeField] private GameObject maskGridPrefab;
    [SerializeField] private GameObject visualCellPrefab;
    [SerializeField] private GameObject workingCellPrefab;
    [SerializeField] private GameObject instantButtonPrefab;

    [Header("Кнопки")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    private List<Image> instantButtons = new List<Image>();

    [Header("Спрайты кнопок")]
    [SerializeField] private Sprite disableInstant;
    [SerializeField] private Sprite activeInstant;

    [Header("Позиции")]
    [SerializeField] private float step;
    [SerializeField] private int speed;
    private float targetPosition;
    private float nowPosition;

    private int index;

    private void Awake()
    {
        countMenus = showingWorld.countLevels / 10;
        header.text = LocalizationManager.GetTranslate(showingWorld.header);
        int countCompleted = showingWorld.completedLevels;

        for (int i=0; i < countMenus; i++) {
            Instantiate(maskGridPrefab, maskGridContainer);

            var visualGrid = Instantiate(visualGridPrefab, visualGridContainer);
            var workingGrid = Instantiate(workingGridPrefab, workingGridContainer);
            
            for (int j=0; j < 10; j++) {
                int number = i * 10 + j;
                var visualCell = Instantiate(visualCellPrefab, visualGrid.transform).GetComponent<CellVisual>();
                var workingCell = Instantiate(workingCellPrefab, workingGrid.transform).GetComponent<CellButton>();

                workingCell.button.targetGraphic = visualCell.number;
                workingCell.button.onClick.AddListener(delegate { LoadLevel(number); });
                workingCell.blur = visualCell.blur;
                workingCell.GetComponent<CellButton>().enabled = number < showingWorld.completedLevels;

                visualCell.lockImage.gameObject.SetActive(number >= showingWorld.completedLevels);
                visualCell.number.gameObject.SetActive(number < showingWorld.completedLevels);
                visualCell.number.text = (number + 1).ToString();
                visualCell.blur.color = new Color(1f, 1f, 1f, 0f);
            }

            var instantButton = Instantiate(instantButtonPrefab, instantGridContainer);
            instantButtons.Add(instantButton.GetComponent<Image>());
            int iCopy = i;
            instantButton.GetComponent<Button>().onClick.AddListener(delegate { InstantTransition(iCopy); });
        }

        index = 0;
        nowPosition = targetPosition = visualGridContainer.localPosition.x;

        UpdateButtons();
        SetInstant(index, index);
        if (!rightButton.interactable)
            GetComponent<Animator>().Play("ButtonDisabled", 1, 0f);
        if (!leftButton.interactable)
            GetComponent<Animator>().Play("ButtonDisabled", 0, 0f);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < speed; i++) {
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
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && index > 0)
            PreviousMenu();
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && index < countMenus - 1)
            NextMenu();
    }

    public void NextMenu()
    {
        index += 1;
        targetPosition -= 923.76f;
        UpdateButtons();
        SetInstant(index - 1, index);
    }
    public void PreviousMenu()
    {
        index -= 1;
        targetPosition += 923.76f;
        UpdateButtons();
        SetInstant(index + 1, index);
    }

    private void UpdateButtons()
    {
        rightButton.interactable = index < countMenus - 1;
        leftButton.interactable = index > 0;
    }

    private void SetInstant(int previuos, int next)
    {
        instantButtons[previuos].sprite = disableInstant;
        instantButtons[previuos].GetComponent<Button>().interactable = true;
        instantButtons[next].sprite = activeInstant;
        instantButtons[next].GetComponent<Button>().interactable = false;
    }

    public void InstantTransition(int newIndex)
    {
        float newPosition = (index - newIndex) * 923.76f;
        nowPosition += newPosition;
        targetPosition += newPosition;
        visualGridContainer.localPosition = new Vector2(nowPosition, visualGridContainer.localPosition.y);
        workingGridContainer.localPosition = new Vector2(nowPosition, workingGridContainer.localPosition.y);
        maskGridContainer.localPosition = new Vector2(nowPosition, maskGridContainer.localPosition.y);
        SetInstant(index, newIndex);
        index = newIndex;
        UpdateButtons();
    }

    public void LoadLevel(int id)
    {
        LevelInfoHolder.deathsCount = 0;
        LevelInfoHolder.restartsCount = 0;
        LevelInfoHolder.levelId = id;
        LevelInfoHolder.worldId = showingWorld.id;
        SceneManager.LoadScene(4);
    }
}
