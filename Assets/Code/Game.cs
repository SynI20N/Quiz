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
    [SerializeField] private GameObject _starParticles;
    [SerializeField] private GameObject _transition;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Text _text;
    [SerializeField] private Canvas _canvas;

    private List<Sprite> _sprites;
    private Data _data;
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
        _sprites = Resources.LoadAll("Images", typeof(Sprite)).Cast<Sprite>().ToList();
        _data = new Data(_sprites, _cellPrefab, _starParticles, _text);
        _animator = GetComponent<Animator>();
        _data.SpawnCells(GetLevelSprites(), _startPoint.position, (int)_currentLevel, 3);
        _data.BounceEffect();
    }
    private List<Sprite> GetLevelSprites()
    {
        return Resources.LoadAll("Images/Level" + (int)_currentLevel, typeof(Sprite)).Cast<Sprite>().ToList();
    }
    public Canvas GetCanvas()
    {
        return _canvas;
    }
    public void Restart()
    {
        _transition.transform.SetAsLastSibling();
        _animator.SetTrigger("Start");

        StartCoroutine("DelayedLoadScene");
    }
    private IEnumerator DelayedLoadScene()
    {
        yield return new WaitForSeconds(1f);
        _data.ReleaseResources();
        SceneManager.LoadScene("Game");
    }
    public IEnumerator GoNextLevel()
    {
        yield return new WaitForSeconds(0.7f);
        _currentLevel = _currentLevel.Next();
        if (_currentLevel < Level.Restart)
        {
            _data.ReleaseResources();
            _data.SpawnCells(GetLevelSprites(), _startPoint.position, (int)_currentLevel, 3);
        }
        else
        {
            _data.CreateButton(_panelPrefab, Vector3.zero, () => { });
            _data.CreateButton(_restartPrefab, Vector3.zero, Restart);
        }
    }
}
enum Level
{
    One = 1,
    Two = 2,
    Three = 3,
    Restart = 4
}
