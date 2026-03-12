using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

public class MinigameSEM : MonoBehaviour
{
    private enum SEMState
    {
        Idle,
        ReadyToStart,
        ReadyToSearch,
        Searching,
        SearchFinished,
        ViewingReport,
        Completed
    }

    [System.Serializable]
    private class SubstanceSEMData
    {
        public SubstanceType substanceType;

        [Header("SEM Initial Image")]
        public Sprite initialSemSprite;

        [Header("Search")]
        public List<Sprite> searchSequence = new List<Sprite>();
        public float searchFrameDuration = 0.08f;
        public int searchLoops = 3;
        public Sprite resultSprite;

        [Header("Report Pages")]
        public List<Sprite> reportPages = new List<Sprite>();

        [Header("Hologram")]
        public GameObject hologramRoot;
    }

    [Header("Detected Sample")]
    [SerializeField] private SubstanceType substanceType = SubstanceType.NULL;

    [Header("Substance Data")]
    [SerializeField] private List<SubstanceSEMData> substancesData = new List<SubstanceSEMData>();

    [Header("Screen 1 - Main Search Screen")]
    [SerializeField] private GameObject screen1Root;
    [SerializeField] private Image mainDisplayImageSEM;
    [SerializeField] private Image mainDisplayImageDatabase;

    [Header("Screen 2 - Report Screen")]
    [SerializeField] private GameObject screen2Root;
    [SerializeField] private GameObject reportRoot;
    [SerializeField] private Image reportPageImage;

    [Header("Buttons")]
    [SerializeField] private Button startScanButton;
    [SerializeField] private Button searchResultsButton;
    [SerializeField] private Button view3DButton;
    [SerializeField] private Button viewReportButton;
    [SerializeField] private Button reportNextButton;
    [SerializeField] private Button reportPrevButton;
    [SerializeField] private Button sendReportButton;
    [SerializeField] private Button popupRestartButton;

    [Header("Hologram Zoom Buttons")]
    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;

    [Header("Button Label References (assign the visible text of each button)")]
    [SerializeField] private TMP_Text startScanTMP;
    [SerializeField] private TMP_Text searchResultsTMP;
    [SerializeField] private TMP_Text view3DTMP;
    [SerializeField] private TMP_Text viewReportTMP;
    [SerializeField] private TMP_Text reportNextTMP;
    [SerializeField] private TMP_Text reportPrevTMP;
    [SerializeField] private TMP_Text sendReportTMP;
    [SerializeField] private TMP_Text popupRestartTMP;
    [SerializeField] private TMP_Text zoomInTMP;
    [SerializeField] private TMP_Text zoomOutTMP;

    [Header("Button Labels")]
    [SerializeField] private string startScanButtonLabel = "Start Scan";
    [SerializeField] private string searchResultsButtonLabel = "Search Results";
    [SerializeField] private string view3DButtonLabel = "View 3D";
    [SerializeField] private string viewReportButtonLabel = "View Report";
    [SerializeField] private string reportNextButtonLabel = "Next";
    [SerializeField] private string reportPrevButtonLabel = "Prev";
    [SerializeField] private string sendReportButtonLabel = "Send Report";
    [SerializeField] private string popupRestartButtonLabel = "Restart";
    [SerializeField] private string zoomInButtonLabel = "+";
    [SerializeField] private string zoomOutButtonLabel = "-";

    [Header("Force Labels")]
    [SerializeField] private bool forceButtonLabelsEveryFrame = true;

    [Header("Popup")]
    [SerializeField] private GameObject completionPopup;

    [Header("VR Restart")]
    [SerializeField] private bool restartWithBothGripButtons = true;

    [Header("Optional Cleanup")]
    [SerializeField] private GameObject sampleMicroscope;

    [Header("State")]
    [SerializeField] private bool isMinigameRunning = false;

    [Header("Hologram Zoom Settings")]
    [SerializeField] private int defaultZoomIndex = 0;

    private SEMState currentState = SEMState.Idle;
    private SubstanceSEMData currentData;

    private int currentSearchIndex = 0;
    private int currentSearchLoop = 0;
    private float searchTimer = 0f;

    private int currentReportPageIndex = 0;
    private int currentHologramZoomIndex = 0;

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    private void Awake()
    {
        CacheXRDevices();
        ApplyButtonLabels();

        if (startScanButton != null)
        {
            startScanButton.onClick.RemoveAllListeners();
            startScanButton.onClick.AddListener(OnStartScanPressed);
        }

        if (searchResultsButton != null)
        {
            searchResultsButton.onClick.RemoveAllListeners();
            searchResultsButton.onClick.AddListener(OnSearchResultsPressed);
        }

        if (view3DButton != null)
        {
            view3DButton.onClick.RemoveAllListeners();
            view3DButton.onClick.AddListener(OnView3DPressed);
        }

        if (viewReportButton != null)
        {
            viewReportButton.onClick.RemoveAllListeners();
            viewReportButton.onClick.AddListener(OnViewReportPressed);
        }

        if (reportNextButton != null)
        {
            reportNextButton.onClick.RemoveAllListeners();
            reportNextButton.onClick.AddListener(OnReportNextPressed);
        }

        if (reportPrevButton != null)
        {
            reportPrevButton.onClick.RemoveAllListeners();
            reportPrevButton.onClick.AddListener(OnReportPrevPressed);
        }

        if (sendReportButton != null)
        {
            sendReportButton.onClick.RemoveAllListeners();
            sendReportButton.onClick.AddListener(OnSendReportPressed);
        }

        if (popupRestartButton != null)
        {
            popupRestartButton.onClick.RemoveAllListeners();
            popupRestartButton.onClick.AddListener(OnRestartPressed);
        }

        if (zoomInButton != null)
        {
            zoomInButton.onClick.RemoveAllListeners();
            zoomInButton.onClick.AddListener(OnZoomInPressed);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.onClick.RemoveAllListeners();
            zoomOutButton.onClick.AddListener(OnZoomOutPressed);
        }

        ResetFullUI();
    }

    private void Start()
    {
        ApplyButtonLabels();
    }

    private void OnEnable()
    {
        ApplyButtonLabels();
    }

    private void Update()
    {
        if (forceButtonLabelsEveryFrame)
        {
            ApplyButtonLabels();
        }

        if (currentState == SEMState.Completed && completionPopup != null && completionPopup.activeInHierarchy)
        {
            CheckVRRestartInput();
        }

        if (currentState != SEMState.Searching)
        {
            return;
        }

        if (currentData == null || currentData.searchSequence == null || currentData.searchSequence.Count == 0)
        {
            FinishSearchImmediately();
            return;
        }

        searchTimer += Time.deltaTime;

        if (searchTimer < currentData.searchFrameDuration)
        {
            return;
        }

        searchTimer = 0f;

        if (mainDisplayImageDatabase != null)
        {
            if (!mainDisplayImageDatabase.gameObject.activeSelf)
            {
                mainDisplayImageDatabase.gameObject.SetActive(true);
            }

            mainDisplayImageDatabase.sprite = currentData.searchSequence[currentSearchIndex];
        }

        currentSearchIndex++;

        if (currentSearchIndex >= currentData.searchSequence.Count)
        {
            currentSearchIndex = 0;
            currentSearchLoop++;
        }

        if (currentSearchLoop >= Mathf.Max(1, currentData.searchLoops))
        {
            FinishSearchImmediately();
        }
    }

    public void SetSubstanceType(SubstanceType s)
    {
        substanceType = s;
        currentData = FindDataForSubstance(substanceType);
        PrepareMinigameForDetectedSample();
    }

    public void SetSampleMicroscope(GameObject sample)
    {
        sampleMicroscope = sample;
    }

    public bool IsMinigameRunning()
    {
        return isMinigameRunning;
    }

    public void SetMinigameRunning(bool value)
    {
        isMinigameRunning = value;
    }

    private SubstanceSEMData FindDataForSubstance(SubstanceType targetType)
    {
        for (int i = 0; i < substancesData.Count; i++)
        {
            if (substancesData[i] != null && substancesData[i].substanceType == targetType)
            {
                return substancesData[i];
            }
        }

        return null;
    }

    private void PrepareMinigameForDetectedSample()
    {
        if (substanceType == SubstanceType.NULL)
        {
            return;
        }

        currentData = FindDataForSubstance(substanceType);

        if (currentData == null)
        {
            Debug.LogWarning("No SEM data configured for substance: " + substanceType);
            return;
        }

        ResetFullUI();

        if (screen1Root != null)
        {
            screen1Root.SetActive(true);
        }

        if (screen2Root != null)
        {
            screen2Root.SetActive(true);
        }

        if (mainDisplayImageSEM != null)
        {
            mainDisplayImageSEM.gameObject.SetActive(true);
            mainDisplayImageSEM.sprite = currentData.initialSemSprite;
        }

        if (mainDisplayImageDatabase != null)
        {
            mainDisplayImageDatabase.gameObject.SetActive(false);
            mainDisplayImageDatabase.sprite = null;
        }

        currentHologramZoomIndex = Mathf.Max(0, defaultZoomIndex);
        DisableAllHolograms();

        if (startScanButton != null)
        {
            startScanButton.gameObject.SetActive(true);
        }

        currentState = SEMState.ReadyToStart;
        isMinigameRunning = true;
    }

    private void OnStartScanPressed()
    {
        if (currentState != SEMState.ReadyToStart)
        {
            return;
        }

        if (startScanButton != null)
        {
            startScanButton.gameObject.SetActive(false);
        }

        if (searchResultsButton != null)
        {
            searchResultsButton.gameObject.SetActive(true);
        }

        currentState = SEMState.ReadyToSearch;
    }

    private void OnSearchResultsPressed()
    {
        if (currentState != SEMState.ReadyToSearch)
        {
            return;
        }

        if (currentData == null)
        {
            Debug.LogWarning("Current SEM data is null.");
            return;
        }

        if (searchResultsButton != null)
        {
            searchResultsButton.gameObject.SetActive(false);
        }

        if (view3DButton != null)
        {
            view3DButton.gameObject.SetActive(false);
        }

        if (viewReportButton != null)
        {
            viewReportButton.gameObject.SetActive(false);
        }

        if (zoomInButton != null)
        {
            zoomInButton.gameObject.SetActive(false);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.gameObject.SetActive(false);
        }

        if (mainDisplayImageDatabase != null)
        {
            mainDisplayImageDatabase.gameObject.SetActive(true);
        }

        searchTimer = 0f;
        currentSearchIndex = 0;
        currentSearchLoop = 0;
        currentState = SEMState.Searching;
    }

    private void FinishSearchImmediately()
    {
        if (currentData != null && mainDisplayImageDatabase != null)
        {
            mainDisplayImageDatabase.gameObject.SetActive(true);
            mainDisplayImageDatabase.sprite = currentData.resultSprite;
        }

        if (view3DButton != null)
        {
            view3DButton.gameObject.SetActive(true);
        }

        if (viewReportButton != null)
        {
            viewReportButton.gameObject.SetActive(true);
        }

        currentState = SEMState.SearchFinished;
    }

    private void OnView3DPressed()
    {
        if (currentState != SEMState.SearchFinished && currentState != SEMState.ViewingReport)
        {
            return;
        }

        currentHologramZoomIndex = Mathf.Max(0, defaultZoomIndex);

        ShowCurrentHologram();

        if (zoomInButton != null)
        {
            zoomInButton.gameObject.SetActive(true);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.gameObject.SetActive(true);
        }

        RefreshZoomButtons();
    }

    private void OnZoomInPressed()
    {
        if (currentData == null || currentData.hologramRoot == null)
        {
            return;
        }

        int childCount = currentData.hologramRoot.transform.childCount;
        if (childCount <= 0)
        {
            return;
        }

        if (currentHologramZoomIndex >= childCount - 1)
        {
            return;
        }

        currentHologramZoomIndex++;
        ShowCurrentHologram();
        RefreshZoomButtons();
    }

    private void OnZoomOutPressed()
    {
        if (currentData == null || currentData.hologramRoot == null)
        {
            return;
        }

        if (currentHologramZoomIndex <= 0)
        {
            return;
        }

        currentHologramZoomIndex--;
        ShowCurrentHologram();
        RefreshZoomButtons();
    }

    private void ShowCurrentHologram()
    {
        DisableAllHolograms();

        if (currentData == null || currentData.hologramRoot == null)
        {
            return;
        }

        currentData.hologramRoot.SetActive(true);

        int childCount = currentData.hologramRoot.transform.childCount;
        if (childCount <= 0)
        {
            return;
        }

        if (currentHologramZoomIndex < 0)
        {
            currentHologramZoomIndex = 0;
        }

        if (currentHologramZoomIndex >= childCount)
        {
            currentHologramZoomIndex = childCount - 1;
        }

        for (int i = 0; i < childCount; i++)
        {
            currentData.hologramRoot.transform.GetChild(i).gameObject.SetActive(i == currentHologramZoomIndex);
        }
    }

    private void RefreshZoomButtons()
    {
        if (currentData == null || currentData.hologramRoot == null)
        {
            if (zoomInButton != null)
            {
                zoomInButton.gameObject.SetActive(false);
            }

            if (zoomOutButton != null)
            {
                zoomOutButton.gameObject.SetActive(false);
            }

            return;
        }

        int childCount = currentData.hologramRoot.transform.childCount;

        if (childCount <= 1)
        {
            if (zoomInButton != null)
            {
                zoomInButton.gameObject.SetActive(false);
            }

            if (zoomOutButton != null)
            {
                zoomOutButton.gameObject.SetActive(false);
            }

            return;
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.gameObject.SetActive(currentHologramZoomIndex > 0);
        }

        if (zoomInButton != null)
        {
            zoomInButton.gameObject.SetActive(currentHologramZoomIndex < childCount - 1);
        }
    }

    private void OnViewReportPressed()
    {
        if (currentState != SEMState.SearchFinished && currentState != SEMState.ViewingReport)
        {
            return;
        }

        if (currentData == null || currentData.reportPages == null || currentData.reportPages.Count == 0)
        {
            Debug.LogWarning("No report pages configured for substance: " + substanceType);
            return;
        }

        currentReportPageIndex = 0;

        if (reportRoot != null)
        {
            reportRoot.SetActive(true);
        }

        if (reportPageImage != null)
        {
            reportPageImage.gameObject.SetActive(true);
        }

        RefreshReportPage();

        currentState = SEMState.ViewingReport;
    }

    private void OnReportNextPressed()
    {
        if (currentState != SEMState.ViewingReport)
        {
            return;
        }

        if (currentData == null || currentData.reportPages == null)
        {
            return;
        }

        if (currentReportPageIndex >= currentData.reportPages.Count - 1)
        {
            return;
        }

        currentReportPageIndex++;
        RefreshReportPage();
    }

    private void OnReportPrevPressed()
    {
        if (currentState != SEMState.ViewingReport)
        {
            return;
        }

        if (currentReportPageIndex <= 0)
        {
            return;
        }

        currentReportPageIndex--;
        RefreshReportPage();
    }

    private void RefreshReportPage()
    {
        if (currentData == null || currentData.reportPages == null || currentData.reportPages.Count == 0)
        {
            return;
        }

        if (reportPageImage != null)
        {
            reportPageImage.gameObject.SetActive(true);
            reportPageImage.sprite = currentData.reportPages[currentReportPageIndex];
        }

        if (reportPrevButton != null)
        {
            reportPrevButton.gameObject.SetActive(currentReportPageIndex > 0);
        }

        bool isLastPage = currentReportPageIndex >= currentData.reportPages.Count - 1;

        if (reportNextButton != null)
        {
            reportNextButton.gameObject.SetActive(!isLastPage);
        }

        if (sendReportButton != null)
        {
            sendReportButton.gameObject.SetActive(isLastPage);
        }
    }

    private void OnSendReportPressed()
    {
        if (currentState != SEMState.ViewingReport)
        {
            return;
        }

        if (completionPopup != null)
        {
            completionPopup.SetActive(true);
        }

        if (popupRestartButton != null)
        {
            popupRestartButton.gameObject.SetActive(false);
        }

        if (sampleMicroscope != null)
        {
            Destroy(sampleMicroscope);
            sampleMicroscope = null;
        }

        currentState = SEMState.Completed;
    }

    private void OnRestartPressed()
    {
        SetMinigameRunning(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckVRRestartInput()
    {
        if (!restartWithBothGripButtons)
        {
            return;
        }

        if (!leftHandDevice.isValid || !rightHandDevice.isValid)
        {
            CacheXRDevices();
        }

        bool leftGripPressed = false;
        bool rightGripPressed = false;

        if (leftHandDevice.isValid)
        {
            leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out leftGripPressed);
        }

        if (rightHandDevice.isValid)
        {
            rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out rightGripPressed);
        }

        if (leftGripPressed && rightGripPressed)
        {
            OnRestartPressed();
        }
    }

    private void CacheXRDevices()
    {
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void ResetFullUI()
    {
        if (screen1Root != null)
        {
            screen1Root.SetActive(true);
        }

        if (screen2Root != null)
        {
            screen2Root.SetActive(true);
        }

        if (reportRoot != null)
        {
            reportRoot.SetActive(false);
        }

        if (completionPopup != null)
        {
            completionPopup.SetActive(false);
        }

        if (mainDisplayImageSEM != null)
        {
            mainDisplayImageSEM.gameObject.SetActive(false);
            mainDisplayImageSEM.sprite = null;
        }

        if (mainDisplayImageDatabase != null)
        {
            mainDisplayImageDatabase.gameObject.SetActive(false);
            mainDisplayImageDatabase.sprite = null;
        }

        if (reportPageImage != null)
        {
            reportPageImage.gameObject.SetActive(false);
            reportPageImage.sprite = null;
        }

        if (startScanButton != null)
        {
            startScanButton.gameObject.SetActive(false);
        }

        if (searchResultsButton != null)
        {
            searchResultsButton.gameObject.SetActive(false);
        }

        if (view3DButton != null)
        {
            view3DButton.gameObject.SetActive(false);
        }

        if (viewReportButton != null)
        {
            viewReportButton.gameObject.SetActive(false);
        }

        if (reportNextButton != null)
        {
            reportNextButton.gameObject.SetActive(false);
        }

        if (reportPrevButton != null)
        {
            reportPrevButton.gameObject.SetActive(false);
        }

        if (sendReportButton != null)
        {
            sendReportButton.gameObject.SetActive(false);
        }

        if (popupRestartButton != null)
        {
            popupRestartButton.gameObject.SetActive(false);
        }

        if (zoomInButton != null)
        {
            zoomInButton.gameObject.SetActive(false);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.gameObject.SetActive(false);
        }

        DisableAllHolograms();

        currentState = SEMState.Idle;
    }

    private void DisableAllHolograms()
    {
        for (int i = 0; i < substancesData.Count; i++)
        {
            if (substancesData[i] != null && substancesData[i].hologramRoot != null)
            {
                substancesData[i].hologramRoot.SetActive(false);
            }
        }
    }

    private void ApplyButtonLabels()
    {
        ApplyTMPLabel(startScanTMP, startScanButtonLabel);
        ApplyTMPLabel(searchResultsTMP, searchResultsButtonLabel);
        ApplyTMPLabel(view3DTMP, view3DButtonLabel);
        ApplyTMPLabel(viewReportTMP, viewReportButtonLabel);
        ApplyTMPLabel(reportNextTMP, reportNextButtonLabel);
        ApplyTMPLabel(reportPrevTMP, reportPrevButtonLabel);
        ApplyTMPLabel(sendReportTMP, sendReportButtonLabel);
        ApplyTMPLabel(popupRestartTMP, popupRestartButtonLabel);
        ApplyTMPLabel(zoomInTMP, zoomInButtonLabel);
        ApplyTMPLabel(zoomOutTMP, zoomOutButtonLabel);
    }

    private void ApplyTMPLabel(TMP_Text targetText, string label)
    {
        if (targetText == null)
        {
            return;
        }

        targetText.text = label;
    }
}