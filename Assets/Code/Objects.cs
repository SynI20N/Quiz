using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Objects
{
    public class Cell : MonoBehaviour, IBouncing
    {
        private Sequence sequence;
        private GameObject prefab;
        private GameObject starParticles;
        private UnityEvent onClick;
        private Sprite image;
        private Transform imageTransform;
        public void Init(GameObject starParticles, Sprite image, UnityAction action)
        {
            this.starParticles = starParticles;
            this.image = image;

            prefab = gameObject;
            prefab.name = this.image.name;
            GameObject _object = new GameObject();
            _object.transform.SetParent(prefab.transform);
            _object.AddComponent<Image>().sprite = this.image;
            _object.name = "Image";
            _object.transform.position = prefab.transform.position;
            imageTransform = _object.transform;

            onClick = new UnityEvent();
            sequence = DOTween.Sequence();

            SetAction(action);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
        public void BounceAndWin()
        {
            SetAction(() => { });
            BounceOut();
            starParticles.transform.position = prefab.transform.position;
            starParticles.GetComponent<ParticleSystem>().Play();
            Game.Instance.StartCoroutine("GoNextLevel");
        }
        public void Shake()
        {
            if(imageTransform.rotation == Quaternion.identity)
                EaseInBounce();
        }
        public Sprite GetImage()
        {
            return image;
        }
        public void BounceIn()
        {
            prefab.transform.localScale = new Vector3(0, 0, 0);
            sequence.Append(prefab.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InBounce));
            sequence.Append(prefab.transform.DOScale(1f, 0.2f).SetEase(Ease.InBounce));
        }
        public void BounceOut()
        {
            sequence.Append(imageTransform.DOScale(1.1f, 0.5f).SetEase(Ease.OutBounce));
            sequence.Append(imageTransform.DOScale(0.1f, 0.2f).SetEase(Ease.OutBounce));
        }
        public void EaseInBounce()
        {
            sequence.Append(imageTransform.DOShakeRotation(0.5f));
        }
        public void SetAction(UnityAction action)
        {
            onClick.RemoveAllListeners();
            onClick.AddListener(action);
        }
        public void Destroy()
        {
            starParticles = null;
            onClick = null;
            image = null;
            sequence.Kill();
            Destroy(prefab);
        }
    }
    public class Button : MonoBehaviour, IFading
    {
        private Sequence sequence;
        private GameObject prefab;
        private UnityEvent onClick;
        private Material material;

        public void Init(UnityAction action)
        {
            prefab = gameObject;
            prefab.transform.SetParent(Game.Instance.GetCanvas().transform);

            onClick = new UnityEvent();
            onClick.AddListener(action);

            GetMaterial();
            sequence = DOTween.Sequence();

            FadeIn();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
        private void GetMaterial()
        {
            Image _image;
            if (prefab.TryGetComponent(out _image))
                material = _image.material;
            Text _text;
            if (prefab.TryGetComponent(out _text))
                material = _text.material;
        }
        public void FadeIn()
        {
            Color _color = material.GetColor("_Color");
            _color.a = 0f;
            material.SetColor("_Color", _color);
            sequence.Append(material.DOFade(1f, 1f));
        }
        public void FadeOut()
        {
            Color _color = material.GetColor("_Color");
            _color.a = 1f;
            material.SetColor("_Color", _color);
            sequence.Append(material.DOFade(0f, 1f));
        }
    }
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
