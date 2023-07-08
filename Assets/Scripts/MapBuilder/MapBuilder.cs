using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private RectTransform _map;
    [SerializeField] private IndexProvider _indexProvider;

    [SerializeField] private int _countItemsRow = 3;
    [SerializeField] private int _countItemsColumn = 5;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private ScriptableItemsHolder _itemsHolder;

    private TileInitializer[,] _tileMap;
    private ScriptableItemSettings[] _itemsSettings;
    
    private void Awake()
    {
        _itemsSettings = _itemsHolder.Items;
        _indexProvider.Initialize(_countItemsColumn,_countItemsRow);
        InitializeMap();
        SetItems();
    }

    private void InitializeMap()
    {
        _tileMap = new TileInitializer [_countItemsRow, _countItemsColumn];

        for (int i = 0; i < _countItemsRow; i++)
        {
            for (int j = 0; j < _countItemsColumn; j++)
            {
                _tileMap[i, j] = Instantiate(_tilePrefab, _map, false).GetComponent<TileInitializer>();
                _tileMap[i, j].transform.localPosition = _indexProvider.GetCoordinates(j,i);
            }
        }
    }

    private void SetItems()
    {
        for (int i = 0; i < _countItemsRow; i++)
        {
            for (int j = 0; j < _countItemsColumn; j++)
            {
                //запускаем поиск рандомного элемента и когда находим инсталим его в текущую клетку
                _tileMap[i, j].Initialize(GetRandomElement(true,i,j));
            }
        }
    }

    private ScriptableItemSettings GetRandomElement (bool isItemGood, int rowIndex, int columnIndex)
    {
        //предпологаем что элемент нам сразу подходит В процессе меняем флаг на фолс если элемент не подходит
        //генерируем рандомный элемент
        var randomElement = _itemsSettings[Random.Range(0, _itemsSettings.Length)];

        //если слева есть более двух элементов то проверяем эти два элемента
        if (columnIndex > 1 && isItemGood)
        {
            var currentType = Convert.ToInt32(randomElement.Type);
            var firstElement = Convert.ToInt32(_tileMap[rowIndex, columnIndex - 1].Type);
            var secondElement = Convert.ToInt32(_tileMap[rowIndex, columnIndex - 2].Type);

            if (firstElement==secondElement)//если предыдущие два элемента равны, то сравниваем с текущим
            {
                if (currentType==firstElement)
                {
                    isItemGood = false; //если они все три равны то помечаем элемент как неподходящий
                }
            }
        }

        //далее сравниваем по столбцу (если по строкам не нашли совпадений)
        if (rowIndex > 1 && isItemGood)
        {
            var currentType = Convert.ToInt32(randomElement.Type);
            var firstElement = Convert.ToInt32(_tileMap[rowIndex - 1, columnIndex].Type);
            var secondElement = Convert.ToInt32(_tileMap[rowIndex - 2, columnIndex].Type);

            if (firstElement==secondElement)
            {
                if (currentType==firstElement)
                {
                    isItemGood = false;
                }
            }
        }

        //если по результатам проверки рандомный элемент не совпал с предыдущими двумя элементами
        //по горизнтали и по вертикали, то он нам подходит
        if (isItemGood)
        {
            return randomElement;
        }

        //иначе запускаем новую итерацию метода
        else
        {
            return GetRandomElement(true, rowIndex, columnIndex);
        }
        
    }
}