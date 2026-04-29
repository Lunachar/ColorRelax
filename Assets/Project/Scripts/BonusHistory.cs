using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusHistory : MonoBehaviour
{
    [SerializeField] private int maxEntries = 10;
    [SerializeField] private float entrySpacing = 46f;
    [SerializeField] private float displayDelay = 0.08f;
    [SerializeField] private TMP_Text entryPrefab;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private Vector2 firstEntryAnchoredPosition = Vector2.zero;
    [SerializeField] private Vector2 entrySize = new Vector2(420f, 42f);
    [SerializeField] private float entryFontSize = 28f;

    private readonly Queue<TMP_Text> activeEntries = new Queue<TMP_Text>();
    private readonly Queue<BonusHistoryEntry> pendingBonuses = new Queue<BonusHistoryEntry>();

    private bool isDisplaying = false;

    public void AddBonusEntry(int bonus)
    {
        AddBonusEntry(bonus, string.Empty);
    }

    public void AddBonusEntry(int bonus, string label)
    {
        pendingBonuses.Enqueue(new BonusHistoryEntry(bonus, label));
        if (!isDisplaying)
        {
            StartCoroutine(ProcessBonusQueue());
        }
    }

    private IEnumerator ProcessBonusQueue()
    {
        isDisplaying = true;

        while (pendingBonuses.Count > 0)
        {
            BonusHistoryEntry bonus = pendingBonuses.Dequeue();
            ShowBonusEntry(bonus);
            yield return new WaitForSeconds(displayDelay);
        }

        isDisplaying = false;
    }

    private void ShowBonusEntry(BonusHistoryEntry bonus)
    {
        while (activeEntries.Count >= maxEntries)
        {
            TMP_Text oldEntry = activeEntries.Dequeue();
            oldEntry.transform.DOKill();
            Destroy(oldEntry.gameObject);
        }

        TMP_Text entry = CreateEntry();

        entry.text = string.IsNullOrWhiteSpace(bonus.Label)
            ? $"+{bonus.Amount}"
            : $"+{bonus.Amount}  {bonus.Label}";
        entry.gameObject.SetActive(true);

        if (entry.transform is RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = firstEntryAnchoredPosition;
            rectTransform.sizeDelta = entrySize;
        }
        else
        {
            entry.transform.localPosition = Vector3.zero;
        }

        entry.transform.DOKill();
        entry.transform.localScale = Vector3.zero;
        entry.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        foreach (TMP_Text activeEntry in activeEntries)
        {
            activeEntry.transform.DOLocalMoveY(activeEntry.transform.localPosition.y - entrySpacing, 0.3f);
        }

        activeEntries.Enqueue(entry);
        UpdateEntriesAppearance();
    }

    private void UpdateEntriesAppearance()
    {
        int total = activeEntries.Count;
        int i = 0;

        foreach (TMP_Text entry in activeEntries)
        {
            float alpha = 1f;
            if (total > 1)
            {
                float t = (float)i / (total - 1);
                alpha = Mathf.Lerp(0.08f, 1f, t);
            }

            Color color = entry.color;
            entry.color = new Color(color.r, color.g, color.b, alpha);
            i++;
        }
    }

    private TMP_Text CreateEntry()
    {
        Transform parent = entriesContainer != null ? entriesContainer : transform;
        GameObject entryObject = new GameObject("BonusEntry", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        entryObject.transform.SetParent(parent, false);

        TextMeshProUGUI entry = entryObject.GetComponent<TextMeshProUGUI>();
        entry.fontSize = entryFontSize;
        entry.fontStyle = FontStyles.Bold;
        entry.alignment = TextAlignmentOptions.Left;
        entry.enableWordWrapping = false;
        entry.overflowMode = TextOverflowModes.Overflow;
        entry.color = Color.white;
        entry.raycastTarget = false;

        if (entryPrefab != null)
        {
            entry.font = entryPrefab.font;
            entry.fontSharedMaterial = entryPrefab.fontSharedMaterial;
            entry.color = entryPrefab.color;
        }

        RectTransform rect = entry.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = entrySize;

        return entry;
    }

    private readonly struct BonusHistoryEntry
    {
        public BonusHistoryEntry(int amount, string label)
        {
            Amount = amount;
            Label = label;
        }

        public int Amount { get; }
        public string Label { get; }
    }
}
