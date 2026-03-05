using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameSEM : MonoBehaviour
{
    private SubstanceType substanceType;
    private int currentSubstanceNum;
    private int substanceNumDB;
    private string correctGraphic = "Quars";

    private Image currentSubstanceImage;
    private Image substanceDBImage;

    private GameObject sampleMicroscope;

    [SerializeField] private ReportInfo reportScreen; // pantalla del ordenador

    private Dictionary<string, Sprite> substanceSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> hologramSubstances = new Dictionary<string, GameObject>();

    #region BUTTONS
    private Button startScan;
    private Button nextPhotoDB;
    private Button prevPhotoDB;
    private Button checkPhotoDB;
    private Button sendReportToDB;
    private Button generateReport;
    #endregion

    #region Hologram
    [SerializeField] private GameObject substanceHologram;
    private Button zoomOut;
    private Button zoomIn;
    private int currentZoom = 1;
    #endregion

    private void Start()
    {
        var resourcesSprites = Resources.LoadAll<Sprite>("ReportSprites");
        for (int i = 0; i < resourcesSprites.Length; i++)
        {
            substanceSprites.Add(resourcesSprites[i].name, resourcesSprites[i]);
        }

        var resourcesHologram = Resources.LoadAll<GameObject>("HologramSubstances");
        for (int i = 0; i < resourcesHologram.Length; i++)
        {
            hologramSubstances.Add(resourcesHologram[i].name, resourcesHologram[i]);
            GameObject hologramSubstance = Instantiate(resourcesHologram[i], substanceHologram.transform);
            hologramSubstance.SetActive(false);
        }

        #region Buttons' OnClick

        startScan = transform.GetChild(0).GetComponent<Button>();
        startScan.onClick.AddListener(() =>
        {
            StartMinigame();

            startScan.gameObject.SetActive(false);
            ToggleCheckSubstance(true);
        });

        nextPhotoDB = transform.GetChild(1).GetComponent<Button>();
        nextPhotoDB.onClick.AddListener(() =>
        {
            NextSubstancePhotoDB();
        });

        prevPhotoDB = transform.GetChild(2).GetComponent<Button>();
        prevPhotoDB.onClick.AddListener(() =>
        {
            PreviousSubstancePhotoDB();
        });

        checkPhotoDB = transform.GetChild(3).GetComponent<Button>();
        checkPhotoDB.onClick.AddListener(() =>
        {
            CheckSubstanceType();
        });

        generateReport = transform.GetChild(4).GetComponent<Button>();
        generateReport.onClick.AddListener(() =>
        {
            GenerateReport();
            generateReport.gameObject.SetActive(false);
            sendReportToDB.gameObject.SetActive(true);
        });

        sendReportToDB = transform.GetChild(5).GetComponent<Button>();
        sendReportToDB.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });

        currentSubstanceImage = transform.GetChild(6).GetComponent<Image>();
        substanceDBImage = transform.GetChild(7).GetComponent<Image>();

        zoomOut = transform.GetChild(8).GetComponent<Button>();
        zoomOut.onClick.AddListener(() =>
        {
            if (currentZoom == 1) { return; }
            currentZoom--;
            ToggleHologramSubstance();
        });

        zoomIn = transform.GetChild(9).GetComponent<Button>();
        zoomIn.onClick.AddListener(() =>
        {
            if (currentZoom == 3) { return; }
            currentZoom++;
            ToggleHologramSubstance();
        });

        #endregion
    }

    private void StartMinigame()
    {
        currentSubstanceNum = Random.Range(1, 4);

        currentSubstanceImage.sprite = substanceSprites[substanceType.ToString() + currentSubstanceNum];

        substanceNumDB = (currentSubstanceNum + 1) % 3 + 1;

        substanceDBImage.sprite = substanceSprites[substanceType.ToString() + substanceNumDB];
    }

    private void CheckSubstanceType()
    {
        if (currentSubstanceNum == substanceNumDB)
        {
            ToggleCheckSubstance(false);
            generateReport.gameObject.SetActive(true);
            ToggleHologram(true);
            zoomIn.gameObject.SetActive(true);
            zoomOut.gameObject.SetActive(true);
        }
        else
        {
            // Error
        }
    }

    private void GenerateReport()
    {
        if (reportScreen == null)
        {
            Debug.LogError("Report screen not assigned.");
            return;
        }

        // Activar el Canvas
        reportScreen.transform.GetChild(0).gameObject.SetActive(true);

        reportScreen.SetSubstanceAndGraphic(
            substanceSprites[substanceType.ToString() + currentSubstanceNum],
            substanceSprites[correctGraphic]
        );

        if (sampleMicroscope != null)
        {
            Destroy(sampleMicroscope);
        }
    }
    private void NextSubstancePhotoDB()
    {
        if (substanceNumDB == 3) { return; }
        substanceNumDB++;
        substanceDBImage.sprite = substanceSprites[substanceType.ToString() + substanceNumDB];
    }

    private void PreviousSubstancePhotoDB()
    {
        if (substanceNumDB == 1) { return; }
        substanceNumDB--;
        substanceDBImage.sprite = substanceSprites[substanceType.ToString() + substanceNumDB];
    }

    public void SetSubstanceType(SubstanceType s)
    {
        substanceType = s;
    }

    public void SetSampleMicroscope(GameObject sample)
    {
        sampleMicroscope = sample;
    }

    private void ToggleCheckSubstance(bool state)
    {
        currentSubstanceImage.gameObject.SetActive(state);
        substanceDBImage.gameObject.SetActive(state);
        nextPhotoDB.gameObject.SetActive(state);
        prevPhotoDB.gameObject.SetActive(state);
        checkPhotoDB.gameObject.SetActive(state);
    }

    private void ToggleHologram(bool state)
    {
        substanceHologram.SetActive(state);
        ToggleHologramSubstance();
    }

    private void ToggleHologramSubstance()
    {
        for (int i = 0; i < 3; i++)
        {
            substanceHologram.transform.GetChild(i).gameObject.SetActive(i + 1 == currentZoom);
        }
    }
}