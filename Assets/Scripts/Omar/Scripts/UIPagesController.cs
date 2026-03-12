using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPagesController : MonoBehaviour
{
    [Serializable]
    public class ButtonTransition
    {
        public Button button;
        [Min(0)] public int targetPageIndex;

        [Header("Optional extra action")]
        public bool openDrawerOnClick = false;
        public DrawerMover drawerToOpen;
    }

    [Serializable]
    public class Page
    {
        [Header("Contenido de la fase (tu PantallaON_XCat)")]
        public GameObject contentRoot;

        [Header("UI que se activa en esta fase (On/Chose/Skip, etc.)")]
        public List<GameObject> enabledUI = new List<GameObject>();

        [Header("Acciones de botones en esta fase")]
        public List<ButtonTransition> transitions = new List<ButtonTransition>();
    }

    [Header("Todos los elementos UI que este script puede controlar (se apagan/encienden por fase)")]
    [SerializeField] private List<GameObject> managedUI = new List<GameObject>();

    [Header("Pages / Phases")]
    [SerializeField] private List<Page> pages = new List<Page>();

    [Header("Start Page")]
    [SerializeField, Min(0)] private int startPageIndex = 0;

    private int currentPageIndex = -1;

    private void Awake()
    {
        // Apaga todos los contenidos al arrancar
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i]?.contentRoot != null)
                pages[i].contentRoot.SetActive(false);
        }

        // Apaga toda la UI gestionada al arrancar
        SetManagedUIActive(false);
    }

    private void Start()
    {
        GoToPage(startPageIndex, force: true);
    }

    public void GoToPage(int pageIndex)
    {
        GoToPage(pageIndex, force: false);
    }

    private void GoToPage(int pageIndex, bool force)
    {
        if (pages == null || pages.Count == 0)
        {
            Debug.LogWarning("[UIPagesController] No pages configured.");
            return;
        }

        if (pageIndex < 0 || pageIndex >= pages.Count)
        {
            Debug.LogWarning($"[UIPagesController] Invalid page index {pageIndex}. Pages count: {pages.Count}");
            return;
        }

        if (!force && pageIndex == currentPageIndex)
            return;

        // Desactiva contenido anterior
        if (currentPageIndex >= 0 && currentPageIndex < pages.Count)
        {
            var prev = pages[currentPageIndex]?.contentRoot;
            if (prev != null) prev.SetActive(false);
        }

        // Activa contenido nuevo
        var nextContent = pages[pageIndex]?.contentRoot;
        if (nextContent != null) nextContent.SetActive(true);

        SetManagedUIActive(false);

        var page = pages[pageIndex];
        if (page != null && page.enabledUI != null)
        {
            foreach (var go in page.enabledUI)
            {
                if (go != null) go.SetActive(true);
            }
        }

        // Reconfigura listeners SOLO para esta fase
        ClearManagedButtonsListeners();
        SetupTransitionsForPage(pageIndex);

        currentPageIndex = pageIndex;
    }

    private void SetManagedUIActive(bool active)
    {
        if (managedUI == null) return;

        foreach (var go in managedUI)
        {
            if (go != null) go.SetActive(active);
        }
    }

    private void ClearManagedButtonsListeners()
    {
        var buttons = new HashSet<Button>();

        for (int i = 0; i < pages.Count; i++)
        {
            var trs = pages[i]?.transitions;
            if (trs == null) continue;

            foreach (var t in trs)
            {
                if (t?.button != null)
                    buttons.Add(t.button);
            }
        }

        foreach (var b in buttons)
            b.onClick.RemoveAllListeners();
    }

    private void SetupTransitionsForPage(int pageIndex)
    {
        var page = pages[pageIndex];
        if (page?.transitions == null) return;

        foreach (var t in page.transitions)
        {
            if (t?.button == null) continue;

            int capturedIndex = t.targetPageIndex;
            bool capturedOpenDrawer = t.openDrawerOnClick;
            DrawerMover capturedDrawer = t.drawerToOpen;

            t.button.onClick.AddListener(() =>
            {
                if (capturedOpenDrawer && capturedDrawer != null)
                    capturedDrawer.Open();

                GoToPage(capturedIndex);
            });
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (startPageIndex < 0) startPageIndex = 0;
        if (pages != null && pages.Count > 0 && startPageIndex >= pages.Count)
            startPageIndex = pages.Count - 1;
    }
#endif
}