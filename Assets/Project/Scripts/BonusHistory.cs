/* System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusHistory : MonoBehaviour
{
    [SerializeField] private int maxEntries = 15;
    [SerializeField] private float entrySpacing = 30f;
    [SerializeField] private float displayDelay = 0.3f;
    [SerializeField] private TMP_Text mainScoreText;
    [SerializeField] private TMP_Text entryPrefab;
    [SerializeField] private Transform entriesContainer;

    private Queue<TMP_Text> activeEntries = new Queue<TMP_Text>();
    private Queue<int> pendingBonuses = new Queue<int>();

    private Vector3 nextEntryPosition;
    private bool isDisplaying = false;

  /*  private void Start()
    {
        nextEntryPosition = mainScoreText.transform.position -
                            new Vector3(0, mainScoreText.rectTransform.rect.height + entrySpacing, 0);
    }
*/
    public void AddBonusEntry(int bonus)
    {
        pendingBonuses.Enqueue(bonus);
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
            int bonus = pendingBonuses.Dequeue();
            ShowBonusEntry(bonus);
            yield return new WaitForSeconds(displayDelay);
        }

        isDisplaying = false;
    }

    private void ShowBonusEntry(int bonus)
    {
        TMP_Text entry = (activeEntries.Count >= maxEntries)
            ? activeEntries.Dequeue()
            : Instantiate(entryPrefab, entriesContainer);

        entry.text = $"+{bonus}";
        entry.transform.position = nextEntryPosition;
        entry.gameObject.SetActive(true);

        entry.transform.DOKill();
        entry.transform.localScale = Vector3.zero;
        entry.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        foreach (var e in activeEntries)
        {
            e.transform.DOLocalMoveY(e.transform.localPosition.y - entrySpacing, 0.3f);
        }

        activeEntries.Enqueue(entry);
        UpdateEntriesAppearance();
    }

    private void UpdateEntriesAppearance()
    {
        int total = activeEntries.Count;
        int i = 0;

        foreach (var e in activeEntries)
        {
            float alpha = 1f;
            if (total > 1)
            {
                float t = (float)i / (total - 1);
                alpha = Mathf.Lerp(0.3f, 1f, t);
            }
            Color c = e.color;
            e.color = new Color(c.r, c.g, c.b, alpha);
            i++;
        }
    }

}