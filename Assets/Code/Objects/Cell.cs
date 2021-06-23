using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IBounceable
{
    private Sequence _sequence;
    private GameObject _box;
    private GameObject _image;
    private UnityEvent _onClick;
    public void Init(Sprite image, UnityAction action)
    {
        _box = gameObject;
        _box.name = image.name;

        GameObject _object = new GameObject(nameof(Image));
        _object.transform.SetParent(_box.transform);
        _object.AddComponent<Image>().sprite = image;
        _object.transform.position = _box.transform.position;
        _image = _object;

        _onClick = new UnityEvent();
        _sequence = DOTween.Sequence();

        SetAction(action);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick.Invoke();
    }
    public void BounceAndWin()
    {
        SetAction(() => { });
        BounceOut();
        Game.Instance.StartCoroutine("GoNextLevel", _box.transform.position);
    }
    public Sprite GetImage()
    {
        return _image.GetComponent<Image>().sprite;
    }
    public void BounceIn()
    {
        _box.transform.localScale = new Vector3(0, 0, 0);
        _sequence.Append(_box.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InBounce));
        _sequence.Append(_box.transform.DOScale(1f, 0.2f).SetEase(Ease.InBounce));
    }
    public void BounceOut()
    {
        _sequence.Append(_image.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutBounce));
        _sequence.Append(_image.transform.DOScale(0.1f, 0.2f).SetEase(Ease.OutBounce));
    }
    public void EaseInBounce()
    {
        if (_image.transform.rotation == Quaternion.identity)
            _sequence.Append(_image.transform.DOShakeRotation(0.5f));
    }
    public void SetAction(UnityAction action)
    {
        _onClick.RemoveAllListeners();
        _onClick.AddListener(action);
    }
    public void Destroy()
    {
        _onClick = null;
        _image = null;
        _sequence.Kill();
        Destroy(_box);
    }
}