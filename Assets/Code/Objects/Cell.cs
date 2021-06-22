using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IBounceable
{
    private Sequence _sequence;
    private GameObject _prefab;
    private GameObject _starParticles;
    private UnityEvent _onClick;
    private Sprite _image;
    private Transform _imageTransform;
    public void Init(GameObject starParticles, Sprite image, UnityAction action)
    {
        _starParticles = starParticles;
        _image = image;

        _prefab = gameObject;
        _prefab.name = _image.name;
        GameObject _object = new GameObject();
        _object.transform.SetParent(_prefab.transform);
        _object.AddComponent<Image>().sprite = _image;
        _object.name = "Image";
        _object.transform.position = _prefab.transform.position;
        _imageTransform = _object.transform;

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
        _starParticles.transform.position = _prefab.transform.position;
        _starParticles.GetComponent<ParticleSystem>().Play();
        Game.Instance.StartCoroutine("GoNextLevel");
    }
    public Sprite GetImage()
    {
        return _image;
    }
    public void BounceIn()
    {
        _prefab.transform.localScale = new Vector3(0, 0, 0);
        _sequence.Append(_prefab.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InBounce));
        _sequence.Append(_prefab.transform.DOScale(1f, 0.2f).SetEase(Ease.InBounce));
    }
    public void BounceOut()
    {
        _sequence.Append(_imageTransform.DOScale(1.1f, 0.5f).SetEase(Ease.OutBounce));
        _sequence.Append(_imageTransform.DOScale(0.1f, 0.2f).SetEase(Ease.OutBounce));
    }
    public void EaseInBounce()
    {
        if (_imageTransform.rotation == Quaternion.identity)
            _sequence.Append(_imageTransform.DOShakeRotation(0.5f));
    }
    public void SetAction(UnityAction action)
    {
        _onClick.RemoveAllListeners();
        _onClick.AddListener(action);
    }
    public void Destroy()
    {
        _starParticles = null;
        _onClick = null;
        _image = null;
        _sequence.Kill();
        Destroy(_prefab);
    }
}