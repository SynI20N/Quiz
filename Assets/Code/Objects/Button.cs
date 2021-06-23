using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button : MonoBehaviour, IFadeable
{
    private Sequence _sequence;
    private GameObject _prefab;
    private UnityEvent _onClick;
    private Material _material;

    public void Init(UnityAction action)
    {
        _prefab = gameObject;
        _prefab.transform.SetParent(Game.Instance.GetCanvas().transform);

        _onClick = new UnityEvent();
        _onClick.AddListener(action);

        GetMaterial();
        _sequence = DOTween.Sequence();

        FadeIn();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick.Invoke();
    }
    public void FadeIn()
    {
        SetColorAlpha(0f);
        _sequence.Append(_material.DOFade(1f, 1f));
    }
    public void FadeOut()
    {
        SetColorAlpha(1f);
        _sequence.Append(_material.DOFade(0f, 1f));
    }
    private void SetColorAlpha(float alpha)
    {
        Color _color = _material.GetColor("_Color");
        _color.a = alpha;
        _material.SetColor("_Color", _color);
    }
    private void GetMaterial()
    {
        Image _image;
        if (_prefab.TryGetComponent(out _image))
            _material = _image.material;
        Text _text;
        if (_prefab.TryGetComponent(out _text))
            _material = _text.material;
    }
}
