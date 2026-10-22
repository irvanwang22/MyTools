using UnityEngine;
using DG.Tweening;

public class PunchEffect : MonoBehaviour
{
    [Header("Scale")]
    public float defaultPunchScale = 1.2f;
    public float duration = 0.35f;

    [Header("Rotation")]
    public float randomRotation = 15f;

    [Header("Ease")]
    public Ease easeOut = Ease.OutBack;
    public Ease easeReturn = Ease.OutElastic;

    private Vector3 originalScale;
    private Vector3 originalRotation;

    private Tween currentTween;

    void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localEulerAngles;
    }

    public void PlayEffect()
    {
        PlayEffect(defaultPunchScale, randomRotation);
    }

    public void PlayEffect(float punchScale)
    {
        PlayEffect(punchScale, randomRotation);
    }

    public void PlayEffect(float punchScale, float rotationRange)
    {
        currentTween?.Kill();

        transform.localScale = originalScale;
        transform.localEulerAngles = originalRotation;

        float randomZ = Random.Range(-rotationRange, rotationRange);

        Sequence seq = DOTween.Sequence();

        seq.Append(
            transform.DOScale(
                originalScale * punchScale,
                duration * 0.2f
            ).SetEase(easeOut)
        );

        seq.Join(
            transform.DORotate(
                new Vector3(0, 0, randomZ),
                duration * 0.2f
            ).SetEase(easeOut)
        );

        seq.Append(
            transform.DOScale(
                originalScale,
                duration * 0.8f
            ).SetEase(easeReturn)
        );

        seq.Join(
            transform.DOLocalRotateQuaternion(
                Quaternion.Euler(originalRotation),
                duration * 0.8f
            ).SetEase(easeReturn)
        );

        currentTween = seq;
    }
}