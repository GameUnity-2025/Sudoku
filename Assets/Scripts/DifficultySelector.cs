using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Attach this script to your LevelSelect UI manager and wire each difficulty button to the corresponding method.
public class DifficultySelector : MonoBehaviour
{
    // Suggested mapping: these numbers represent how many cells to remove (or another metric used by Board)
    // Adjust as needed to match the game's intended difficulty scale.
    public int easy = 20;
    public int medium = 35;
    public int hard = 50;
    public int veryHard = 60;
    public int master = 70;

    [Tooltip("Optional UI Text to show the currently selected difficulty (e.g. 'Easy')")]
    public Text difficultyLabel;
    [Tooltip("If true, this will auto-find buttons in the scene by their visible text and attach listeners.")]
    public bool autoWireButtons = true;
    [Header("Back confirmation")]
    [Tooltip("If true, show a confirmation dialog before returning to MainMenu")]
    public bool confirmOnBack = true;
    [Tooltip("Optional: assign a panel GameObject (inactive) that contains Yes/No buttons and a Text. If null, a simple runtime panel will be created.")]
    public GameObject confirmPanel;
    public UnityEngine.UI.Button confirmYesButton;
    public UnityEngine.UI.Button confirmNoButton;
    public UnityEngine.UI.Text confirmTextUI;
    public string confirmMessage = "Return to Main Menu? Your progress will be lost.";

    void Start()
    {
        // Initialize label if present
        UpdateLabelFromCurrent();

        if (autoWireButtons)
        {
            AutoWireButtonsByText();
        }
    }

    void AutoWireButtonsByText()
    {
        // Find all buttons in scene and attach listeners based on their visible text
        var buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        foreach (var b in buttons)
        {
            string label = null;
            var t = b.GetComponentInChildren<UnityEngine.UI.Text>();
            if (t != null) label = t.text;

            // try TMP if present
            if (label == null)
            {
                var tmp = b.GetComponentInChildren<TMPro.TMP_Text>();
                if (tmp != null) label = tmp.text;
            }

            if (string.IsNullOrEmpty(label)) continue;
            string s = label.Trim().ToLower();

            // remove any newline or extra characters
            if (s.Contains("easy"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(SelectEasy);
                b.onClick.AddListener(UpdateLabelFromCurrent);
            }
            else if (s.Contains("medium"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(SelectMedium);
                b.onClick.AddListener(UpdateLabelFromCurrent);
            }
            else if (s.Contains("very hard") || s.Contains("veryhard") || s.Contains("very-hard"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(SelectVeryHard);
                b.onClick.AddListener(UpdateLabelFromCurrent);
            }
            else if (s.Contains("hard") && !s.Contains("very"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(SelectHard);
                b.onClick.AddListener(UpdateLabelFromCurrent);
            }
            else if (s.Contains("master"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(SelectMaster);
                b.onClick.AddListener(UpdateLabelFromCurrent);
            }
            else if (s.Contains("back"))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(BackToMainMenu);
            }
        }
    }

    public void BackToMainMenu()
    {
        if (!confirmOnBack)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        ShowConfirm();
    }

    void ShowConfirm()
    {
        if (confirmPanel != null)
        {
            // use assigned panel
            if (confirmTextUI != null) confirmTextUI.text = confirmMessage;
            confirmPanel.SetActive(true);
            if (confirmYesButton != null)
            {
                confirmYesButton.onClick.RemoveAllListeners();
                confirmYesButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
            }
            if (confirmNoButton != null)
            {
                confirmNoButton.onClick.RemoveAllListeners();
                confirmNoButton.onClick.AddListener(() => confirmPanel.SetActive(false));
            }
            return;
        }

        // create a simple runtime confirm dialog if none assigned
        CreateRuntimeConfirm();
    }

    GameObject runtimeConfirm;
    void CreateRuntimeConfirm()
    {
        if (runtimeConfirm != null) { runtimeConfirm.SetActive(true); return; }

        // Create Canvas child panel
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        runtimeConfirm = new GameObject("ConfirmPanel");
        runtimeConfirm.transform.SetParent(canvas.transform, false);

        var image = runtimeConfirm.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0f,0f,0f,0.6f); // dark overlay
        var rt = runtimeConfirm.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        // inner box
        var box = new GameObject("Box");
        box.transform.SetParent(runtimeConfirm.transform, false);
        var boxImage = box.AddComponent<UnityEngine.UI.Image>();
        // darker box for contrast
        boxImage.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        var brt = box.GetComponent<RectTransform>();
        brt.sizeDelta = new Vector2(520,200);
        brt.anchorMin = new Vector2(0.5f, 0.5f);
        brt.anchorMax = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = Vector2.zero;
        brt.pivot = new Vector2(0.5f, 0.5f);
        // subtle outline for the box
        var boxOutline = box.AddComponent<UnityEngine.UI.Outline>();
        boxOutline.effectColor = new Color(0f,0f,0f,0.6f);
        boxOutline.effectDistance = new Vector2(2, -2);

        // text
        var textGO = new GameObject("ConfirmText");
        textGO.transform.SetParent(box.transform, false);
        var txt = textGO.AddComponent<UnityEngine.UI.Text>();
        txt.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = confirmMessage;
        txt.fontSize = 24;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        // add outline + shadow for readability
        var txtOutline = textGO.AddComponent<UnityEngine.UI.Outline>();
        txtOutline.effectColor = new Color(0f,0f,0f,0.8f);
        txtOutline.effectDistance = new Vector2(2, -2);
        var txtShadow = textGO.AddComponent<UnityEngine.UI.Shadow>();
        txtShadow.effectColor = new Color(0f,0f,0f,0.5f);
        txtShadow.effectDistance = new Vector2(1, -1);
        var trt = textGO.GetComponent<RectTransform>();
        trt.sizeDelta = new Vector2(480,100);
        trt.anchoredPosition = new Vector2(0,30);

        // Yes button
        var yes = new GameObject("YesButton");
        yes.transform.SetParent(box.transform, false);
        var yesBtn = yes.AddComponent<UnityEngine.UI.Button>();
        var yesImg = yes.AddComponent<UnityEngine.UI.Image>();
        // green affirmative button
        yesImg.color = new Color(0.09f, 0.6f, 0.18f, 1f);
        var ytxt = new GameObject("Text"); ytxt.transform.SetParent(yes.transform, false);
        var ytext = ytxt.AddComponent<UnityEngine.UI.Text>(); ytext.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf"); ytext.alignment = TextAnchor.MiddleCenter; ytext.text = "Yes";
        ytext.color = Color.white; ytext.fontSize = 20; ytext.fontStyle = FontStyle.Bold;
        var yrt = yes.GetComponent<RectTransform>(); yrt.sizeDelta = new Vector2(160,48); yrt.anchoredPosition = new Vector2(-110,-60);
        // button color transitions
        var yesColors = yesBtn.colors; yesColors.normalColor = yesImg.color; yesColors.highlightedColor = new Color(0.18f, 0.75f, 0.28f, 1f); yesColors.pressedColor = new Color(0.06f,0.4f,0.12f,1f); yesBtn.colors = yesColors;

        // No button
        var no = new GameObject("NoButton");
        no.transform.SetParent(box.transform, false);
        var noBtn = no.AddComponent<UnityEngine.UI.Button>();
        var noImg = no.AddComponent<UnityEngine.UI.Image>();
        // neutral cancel button
        noImg.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        var ntxt = new GameObject("Text"); ntxt.transform.SetParent(no.transform, false);
        var ntext = ntxt.AddComponent<UnityEngine.UI.Text>(); ntext.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf"); ntext.alignment = TextAnchor.MiddleCenter; ntext.text = "No";
        ntext.color = Color.white; ntext.fontSize = 20; ntext.fontStyle = FontStyle.Bold;
        var nrt = no.GetComponent<RectTransform>(); nrt.sizeDelta = new Vector2(160,48); nrt.anchoredPosition = new Vector2(110,-60);
        var noColors = noBtn.colors; noColors.normalColor = noImg.color; noColors.highlightedColor = new Color(0.7f,0.7f,0.7f,1f); noColors.pressedColor = new Color(0.45f,0.45f,0.45f,1f); noBtn.colors = noColors;

        yesBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        noBtn.onClick.AddListener(() => runtimeConfirm.SetActive(false));
    }


    void UpdateLabelFromCurrent()
    {
        if (difficultyLabel == null) return;

        int d = PlayerSettings.difficulty;
        string name = DifficultyNameForValue(d);
        difficultyLabel.text = "Difficulty: " + name;
    }

    string DifficultyNameForValue(int val)
    {
        if (val == easy) return "Easy";
        if (val == medium) return "Medium";
        if (val == hard) return "Hard";
        if (val == veryHard) return "Very Hard";
        if (val == master) return "Master";
        return val.ToString();
    }

    public void SelectEasy()
    {
        PlayerSettings.SetDifficulty(easy);
        UpdateLabelFromCurrent();
        Debug.Log("Difficulty set: Easy (" + easy + ")");
    }

    public void SelectMedium()
    {
        PlayerSettings.SetDifficulty(medium);
        UpdateLabelFromCurrent();
        Debug.Log("Difficulty set: Medium (" + medium + ")");
    }

    public void SelectHard()
    {
        PlayerSettings.SetDifficulty(hard);
        UpdateLabelFromCurrent();
        Debug.Log("Difficulty set: Hard (" + hard + ")");
    }

    public void SelectVeryHard()
    {
        PlayerSettings.SetDifficulty(veryHard);
        UpdateLabelFromCurrent();
        Debug.Log("Difficulty set: Very Hard (" + veryHard + ")");
    }

    public void SelectMaster()
    {
        PlayerSettings.SetDifficulty(master);
        UpdateLabelFromCurrent();
        Debug.Log("Difficulty set: Master (" + master + ")");
    }
}
