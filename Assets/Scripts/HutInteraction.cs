using UnityEngine;
using UnityEngine.UI;

public class HutInteraction : MonoBehaviour
{
    [SerializeField] private GameObject hutInteractionPanel;

    [SerializeField] private GameObject[] visualCubes;

    [SerializeField] private Button takeButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Text hutCubeCountText;
    [SerializeField] private Text playerCubeCountText;

    [SerializeField] private int cubeCount;

    [SerializeField] private float interactionDistance;

    private PlayerController player;

    private static HutInteraction currentHut;

    private static bool isAnyPanelOpen = false;

    private void Start()
    {
        hutInteractionPanel.SetActive(false);

        InitializeButtons();

        UpdateVisualCubes();

        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= interactionDistance && !isAnyPanelOpen)
        {
            DetectCabinClick();
        }
    }

    private void InitializeButtons()
    {
        takeButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        takeButton.onClick.AddListener(TakeCube);
        dropButton.onClick.AddListener(DropCube);
        quitButton.onClick.AddListener(ClosePanel);
    }

    private void DetectCabinClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (!isAnyPanelOpen)
                    {
                        OpenPanel();
                    }
                }
            }
        }
    }

    private void OpenPanel()
    {
        hutInteractionPanel.SetActive(true);

        currentHut = this;

        isAnyPanelOpen = true;

        UpdatePanel();

        player.DisableMovement();
    }

    private void TakeCube()
    {
        if (currentHut != null && currentHut.cubeCount > 0)
        {
            currentHut.cubeCount--;
            player.AddCube();

            currentHut.UpdatePanel();
            currentHut.UpdateVisualCubes();
        }
    }

    private void DropCube()
    {
        if (currentHut != null && player.GetCubeCount() > 0)
        {
            currentHut.cubeCount++;
            player.RemoveCube();

            currentHut.UpdatePanel();
            currentHut.UpdateVisualCubes();
        }
    }

    private void ClosePanel()
    {
        hutInteractionPanel.SetActive(false);

        currentHut = null;

        isAnyPanelOpen = false;

        player.EnableMovement();
    }

    private void UpdatePanel()
    {
        takeButton.gameObject.SetActive(cubeCount > 0);
        dropButton.gameObject.SetActive(player.GetCubeCount() > 0);

        hutCubeCountText.text = "Cubes dans la cabane : " + cubeCount;
        playerCubeCountText.text = "Cubes dans l'inventaire : " + player.GetCubeCount();
    }

    private void UpdateVisualCubes()
    {
        for (int i = 0; i < visualCubes.Length; i++)
        {
            if (i < cubeCount)
            {
                visualCubes[i].SetActive(true);
            }
            else
            {
                visualCubes[i].SetActive(false);
            }
        }
    }
}