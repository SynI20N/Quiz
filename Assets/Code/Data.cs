using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Data : ScriptableObject
{
    private List<Sprite> _images;
    private List<Cell> _cells;
    private const int _maxImagesOnScreen = 9;
    private const int _maxButtonsOnScreen = 9;
    // cells part
    private GameObject _cellPrefab;
    private GameObject _starParticles;

    //text
    private Text _text;

    //UI part
    private List<Button> _buttons;

    public Data(List<Sprite> images, GameObject cellPrefab, GameObject starParticles, Text text)
    {
        _text = text;
        _images = images.GroupBy(x => x.name).Select(x => x.First()).ToList();
        _cellPrefab = cellPrefab;
        _starParticles = starParticles;

        _cells = new List<Cell>(_maxImagesOnScreen);
        _buttons = new List<Button>(_maxButtonsOnScreen);

        Button _button = text.gameObject.AddComponent<Button>();
        _buttons.Add(_button);
        _button.Init(() => { });
    }
    public void SpawnCells(List<Sprite> images, Vector3 startPos, int rows, int columns)
    {
        RemoveUsed(ref images);
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject _object = Instantiate(_cellPrefab, GenerateVector(startPos, i, columns), Quaternion.identity, Game.Instance.GetCanvas().transform);
            _cells.Add(_object.AddComponent<Cell>());
            _cells[i].Init(_starParticles, PickRandomSprite(ref images), _cells[i].EaseInBounce);
        }
        images.Clear();
        int _index = Random.Range(0, rows * columns);
        _cells[_index].SetAction(_cells[_index].BounceAndWin);
        string _str = _text.text;
        _str = _str.Trim();
        _str = _str.Remove(_str.LastIndexOf(' ')).TrimEnd();
        _str += " " + _cells[_index].GetImage().name;
        _text.text = _str;
        _images.Remove(_cells[_index].GetImage());
    }
    private void RemoveUsed(ref List<Sprite> sprites)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite _image = sprites[i];
            if (_images.Find(x => x.name == _image.name) == null)
            {
                sprites.Remove(sprites[i]);
            }
        }
    }
    public void CreateButton(GameObject prefab, Vector3 startPos, UnityAction action)
    {
        GameObject _object = Instantiate(
                prefab,
                startPos,
                Quaternion.identity);
        Button button = _object.AddComponent<Button>();
        _buttons.Add(button);
        button.Init(action);
    }
    private Vector3 GenerateVector(Vector3 startPos, int step, int columns)
    {
        Vector3 _vector = startPos;
        RectTransform _rect = _cellPrefab.GetComponent<RectTransform>();
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