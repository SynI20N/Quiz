using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Objects;
using UnityEngine.UI;
using System.Linq;

public class Data : ScriptableObject
{
    private List<Sprite> images;
    private List<Cell> cells;
    private const int maxImagesOnScreen = 9;
    private const int maxButtonsOnScreen = 9;
    // cells part
    private GameObject cellPrefab;
    private GameObject starParticles;

    //text
    private Text text;

    //UI part
    private List<Objects.Button> buttons;

    public Data(List<Sprite> images, GameObject cellPrefab, GameObject starParticles, Text text)
    {
        this.text = text;
        this.images = images.GroupBy(x => x.name).Select(x => x.First()).ToList();
        this.cellPrefab = cellPrefab;
        this.starParticles = starParticles;

        cells = new List<Cell>(maxImagesOnScreen);
        buttons = new List<Objects.Button>(maxButtonsOnScreen);

        Objects.Button _button = text.gameObject.AddComponent<Objects.Button>();
        buttons.Add(_button);
        _button.Init("Idle");
    }
    public void SpawnCells(List<Sprite> images, Vector3 startPos, int rows, int columns)
    {
        Clean(ref images);
        for(int i = 0; i < rows * columns; i++)
        {
            GameObject _object = Instantiate(
                cellPrefab,
                GenerateVector(startPos,i,columns),
                Quaternion.identity, 
                Game.Instance.GetCanvas().transform);
            cells.Add(_object.AddComponent<Cell>());
            cells[i].Init(starParticles, PickRandomSprite(ref images), "Shake");
        }
        images.Clear();
        int _index = Random.Range(0, rows * columns);
        cells[_index].SetAction("Win");
        string _str = text.text;
        _str = _str.Trim();
        _str = _str.Remove(_str.LastIndexOf(' ')).TrimEnd();
        _str += " " + cells[_index].GetImage().name;
        text.text = _str;
        this.images.Remove(cells[_index].GetImage());
    }
    private void Clean(ref List<Sprite> sprites)
    {
        for(int i = 0; i < sprites.Count; i++)
        {
            Sprite _image = sprites[i];
            if(images.Find(x => x.name == _image.name) == null)
            {
                sprites.Remove(sprites[i]);
            }
        }
    }
    public void CreateRestart(GameObject prefab)
    {
        GameObject _object = Instantiate(
                prefab,
                Vector3.zero,
                Quaternion.identity);
        Objects.Button button = _object.AddComponent<Objects.Button>();
        buttons.Add(button);
        button.Init("Restart");
    }
    public void CreatePanel(GameObject prefab)
    {
        GameObject _object = Instantiate(
                prefab,
                Vector3.zero,
                Quaternion.identity);
        Objects.Button button = _object.AddComponent<Objects.Button>();
        buttons.Add(button);
        button.Init("Idle");
    }
    private Vector3 GenerateVector(Vector3 startPos, int step, int columns)
    {
        Vector3 _vector = startPos;
        RectTransform _rect = cellPrefab.GetComponent<RectTransform>();
        _vector.Set(
            startPos.x + step % columns * _rect.rect.width,
            startPos.y - Mathf.Round(step / columns) * _rect.rect.height,
            startPos.z);
        return _vector;
    }
    private Sprite PickRandomSprite(ref List<Sprite> sprites)
    {
        int _index = Random.Range(0, sprites.Count);
        Sprite _sprite = sprites[_index];
        sprites.Remove(_sprite);
        return _sprite;
    }
    public void BounceEffect()
    {
        foreach(var c in cells)
        {
            c.BounceIn();
        }
    }
    public void ReleaseResources()
    {
        foreach(var c in cells)
        {
            c.Destroy();
        }
        cells.Clear();
    }
}