using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#pragma warning disable 649

[RequireComponent(typeof(Animator))]
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _restartPrefab;
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private GameObject _starsPrefab;
    [SerializeField] private GameObject _transitionPrefab;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Text _text;

    private Container _container;
    private Level _currentLevel = Level.One;
    private Animator _animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        DOTween.Init();
    }
    private void Start()
    {
        _container = new Container(GetAllSprites(), _text);
        _animator = GetComponent<Animator>();
        _container.CreateCells(_cellPrefab,new Table(_startPoint.position, (int)_currentLevel, 3), GetLevelSprites());
        _container.BounceEffect();
    }
    private List<Sprite> GetLevelSprites()
    {
        return Resources.LoadAll("Images/Level" + (int)_currentLevel, typeof(Sprite)).Cast<Sprite>().ToList();
    }
    private List<Sprite> GetAllSprites()
    {
        return Resources.LoadAll("Images", typeof(Sprite)).Cast<Sprite>().ToList();
    }
    public Canvas GetCanvas()
    {
        return FindObjectOfType<Canvas>();
    }
    public void Restart()
    {
        _transitionPrefab.transform.SetAsLastSibling();
        _animator.SetTrigger("Start");

        StartCoroutine("DelayedLoadScene");
    }
    private IEnumerator DelayedLoadScene()
    {
        yield return new WaitForSeconds(1f);
        _container.ReleaseResources();
        SceneManager.LoadScene("Game");
    }
    public IEnumerator GoNextLevel(Vector3 starsPos)
    {
        _starsPrefab.transform.position = starsPos;
        _starsPrefab.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.7f);
        _currentLevel = _currentLevel.Next();
        if (_currentLevel < Level.Restart)
        {
            _container.ReleaseResources();
            _container.CreateCells(_cellPrefab, new Table(_startPoint.position, (int)_currentLevel, 3), GetLevelSprites());
        }
        else
        {
            _container.CreateButton(_panelPrefab, Vector3.zero, () => { });
            _container.CreateButton(_restartPrefab, Vector3.zero, Restart);
        }
    }
}
