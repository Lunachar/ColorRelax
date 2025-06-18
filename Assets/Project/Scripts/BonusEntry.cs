using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusEntry : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        Invoke(nameof(FadeAndDestroy), lifetime);
    }

    private void FadeAndDestroy()
    {
        if (text == null)
        {
            Destroy(gameObject);
            return;
        }

        text.DOFade(0, fadeOutDuration);
        transform.DOScale(0.7f, fadeOutDuration)
            .SetEase(Ease.InBack);
        transform.DOMoveY(transform.position.y + 20f, fadeOutDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}