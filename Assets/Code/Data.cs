using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Container : ScriptableObject
{
    private const int _maxCellsOnScreen = 9;
    private const int _maxButtonsOnScreen = 9;

    private List<Sprite> _sprites;
    private List<Sprite> _levelSprites;
    private List<Cell> _cells;
    private List<Button> _buttons;

    private Canvas _canvas;
    private Text _text;

    public Container(List<Sprite> images, Text text)
    {
        _sprites = images.GroupBy(x => x.name).Select(x => x.First()).ToList();
        _text = text;
        _canvas = FindObjectOfType<Canvas>();
        _cells = new List<Cell>(_maxCellsOnScreen);
        _buttons = new List<Button>(_maxButtonsOnScreen);
    }
    public void CreateCells(GameObject prefab, Table table, List<Sprite> levelSprites)
    {
        _levelSprites = levelSprites;
        RemoveUsed(ref _levelSprites);
        for (int i = 0; i < table.Rows * table.Columns; i++)
        {
            GameObject _object = Instantiate(prefab, GenerateVector(table, i, prefab), Quaternion.identity, _canvas.transform);
            _cells.Add(_object.AddComponent<Cell>());
            _cells[i].Init(PickRandomSprite(ref _levelSprites), _cells[i].EaseInBounce);
        }
        int _index = Random.Range(0, table.Rows * table.Columns);
        _cells[_index].SetAction(_cells[_index].BounceAndWin);
        UpdateText(_index);
    }
    private void UpdateText(int cellIndex)
    {
        string _str = _text.text;
        _str = _str.Trim();
        _str = _str.Remove(_str.LastIndexOf(' ')).TrimEnd();
        _str += " " + _cells[cellIndex].GetImage().name;
        _text.text = _str;
        _sprites.Remove(_cells[cellIndex].GetImage());
    }
    public void CreateButton(GameObject prefab, Vector3 startPos, UnityAction action)
    {
        GameObject _object = Instantiate(
                prefab,
                startPos,
                Quaternion.identity,
                _canvas.transform);
        Button button = _object.AddComponent<Button>();
        _buttons.Add(button);
        button.Init(action);
    }
    private void RemoveUsed(ref List<Sprite> sprites)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite _image = sprites[i];
            if (_sprites.Find(x => x.name == _image.name) == null)
            {
                sprites.Remove(sprites[i]);
            }
        }
    }
    private Vector3 GenerateVector(Table table, int step, GameObject cellPrefab)
    {
        Vector3 _vector = table.StartPoint;
        RectTransform _rect = cellPrefab.GetComponent<RectTransform>();
        _vector.Set(
            table.StartPoint.x + step % table.Columns * _rect.rect.width,
            table.StartPoint.y - Mathf.Round(step / table.Columns) * _rect.rect.height,
            table.StartPoint.z);
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
        foreach (var c in _cells)
        {
            c.BounceIn();
        }
    }
    public void ReleaseResources()
    {
        foreach (var c in _cells)
        {
            c.Destroy();
        }
        _cells.Clear();
    }
}