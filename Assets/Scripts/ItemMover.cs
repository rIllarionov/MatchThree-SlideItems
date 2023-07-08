using System;
using DG.Tweening;
using UnityEngine;


public class ItemMover : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private IndexProvider _indexProvider;
    
    private GameObject _firstElement;
    private GameObject _firstParent;

    private GameObject _secondElement;
    private GameObject _secondParent;

    private bool _hasFirstElement;
    private bool _hasSecondElement;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _hasFirstElement == false) //захват первого элемента
        {
            _hasFirstElement = SelectElement(out _firstElement, out _firstParent);
        }

        if (Input.GetMouseButtonUp(0) && _hasFirstElement) //захват второго элемента
        {
            _hasSecondElement = SelectElement(out _secondElement, out _secondParent);
            
            if (!_hasSecondElement) //если не оказалось элемента под мышью то сбрасываем так же и первый элемент
            {
                _hasFirstElement = false;
            }
        }

        if (_hasFirstElement && _hasSecondElement) //если оба элемента есть
                                                   //и выполняется условие проверки, то меняем их местами
        {
            if (CanMove())
            {
                ChangeElements();
            }
            else
            {
                _hasFirstElement = false;
                _hasSecondElement = false;
            }
        }
    }

    private void ChangeElements()
    {
        _hasFirstElement = false;
        _hasSecondElement = false;
        
        var firstElementStartPosition = _firstElement.transform.position;
        var secondElementStartPosition = _secondElement.transform.position;

        _firstElement.transform
            .DOMove(secondElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);

        _secondElement.transform
            .DOMove(firstElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);
        
        ReversParents();
    }

    private void ReversParents()
    {
        _firstElement.transform.SetParent(_secondParent.transform);
        _secondElement.transform.SetParent(_firstParent.transform);
    }

    private bool SelectElement(out GameObject element, out GameObject parent)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.one);

        if (hit.collider != null)
        {
            element = hit.collider.gameObject;
            parent = element.transform.parent.gameObject;
            return true;
        }
        else
        {
            element = null;
            parent = null;
            return false;
        }
    }

    private bool CanMove() //проверяем что объекты рядом и что двигать планируем не диагонально
    {
        
        // если индекс x отлиается на единицу а у не отличается или если у отличается на единицу а х не меняется
        var firstArrayElement = _indexProvider.GetIndex(_firstElement.transform.position);
        var secondArrayElement = _indexProvider.GetIndex(_secondElement.transform.position);

        if (firstArrayElement.x == secondArrayElement.x && Math.Abs(firstArrayElement.y - secondArrayElement.y) == 1)
        {
            return true;
        }
        
        else if (firstArrayElement.y == secondArrayElement.y && Math.Abs(firstArrayElement.x - secondArrayElement.x) == 1)
        {
            return true;
        }

        return false;
    }
}