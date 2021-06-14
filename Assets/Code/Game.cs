using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Objects;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject restartPrefab;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private GameObject starParticles;
    [SerializeField] private GameObject transition;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Text text;
    [SerializeField] private Canvas canvas;

    private List<Sprite> sprites;
    private Data data;
    private Level currentLevel;
    private Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        DOTween.Init();
        DOTween.useSafeMode = false;
    }
    private void Start()
    {
        sprites = Resources.LoadAll("Images", typeof(Sprite)).Cast<Sprite>().ToList();
        data = new Data(sprites, cellPrefab, starParticles, text);
        currentLevel = Level.One;
        animator = GetComponent<Animator>();
        data.SpawnCells(GetLevelSprites(), startPoint.position, (int)currentLevel, 3);
        data.BounceEffect();
    }
    private List<Sprite> GetLevelSprites()
    {
        return Resources.LoadAll("Images/Level" + (int)currentLevel, typeof(Sprite)).Cast<Sprite>().ToList();
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public IEnumerator Restart()
    {
        transition.transform.SetAsLastSibling();
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1f);
        data.ReleaseResources();
        SceneManager.LoadScene("Game");
    }
    public IEnumerator GoNextLevel()
    {
        yield return new WaitForSeconds(0.7f);
        currentLevel = currentLevel.Next();
        if (currentLevel < Level.Restart)
        {
            data.ReleaseResources();
            data.SpawnCells(GetLevelSprites(), startPoint.position, (int)currentLevel, 3);
        }
        else
        {
            data.CreatePanel(panelPrefab);
            data.CreateRestart(restartPrefab);
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
