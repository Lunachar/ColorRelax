using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusHistory : MonoBehaviour
{
    [SerializeField] private int maxEntries = 15;
    [SerializeField] private float entrySpacing = 30f;
    [SerializeField] private TMP_Text mainScoreText;
    [SerializeField] private TMP_Text entryPrefab;
    [SerializeField] private Transform entriesContainer;
    
    private Queue<TMP_Text> activeEntries = new Queue<TMP_Text>();
    private Vector3 nextEntryPosition;

    private void Start()
    {
        nextEntryPosition = mainScoreText.transform.position - 
                            new Vector3(0, mainScoreText.rectTransform.rect.height + entrySpacing, 0);
    }

    public void AddBonusEntry(int bonus)
    {
        // create or reuse an entry
        TMP_Text entry = (activeEntries.Count >= maxEntries) ? 
            activeEntries.Dequeue() : Instantiate(entryPrefab, entriesContainer);
        
        // set the text
        entry.text = $"+{bonus}";
        entry.transform.position = nextEntryPosition;
        entry.gameObject.SetActive(true);
        
        // animate
        entry.transform.localScale = Vector3.zero;
        entry.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        
        // move other entries
        foreach (var e in activeEntries)
        {
            e.transform.DOMoveY(e.transform.position.y - entrySpacing, 0.3f);
        }
        
        activeEntries.Enqueue(entry);
    }
}