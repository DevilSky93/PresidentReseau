using System;
using DG.Tweening;
using MLAPI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour
{
    public CardData card;
    public MeshRenderer theRenderer;

    private int value;
    private string nameCard;
    private CardData.Color colorValue;
    private CardData.SymbolCard symbol;

    private bool _isInHand = true;
    private bool _isOver, _isSelected;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private float zCoord;

    public void InitCard()
    {
        theRenderer.material = card.CardMat;
        value = card.Value;
        nameCard = card.NameCard;
        colorValue = card.ColorValue;
        symbol = card.Symbol;
    }

    private void OnMouseEnter()
    {
        if (_isInHand && !_isOver)
        {
            transform.DOLocalMove(new Vector2(0, .2f), .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                _isOver = true;
            });
        }
    }
    
    private void OnMouseDown()
    {
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        _isSelected = true;
        // _offset = transform.position -
        //          Camera.main.ScreenToWorldPoint(
        //              new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

        _offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        var mousePos = Input.mousePosition;

        transform.position = GetMouseWorldPos() + _offset;
    }

    private void OnMouseExit()
    {
        if (_isInHand && !_isSelected)
        {
            transform.DOLocalMove(Vector2.zero, .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                _isOver = false;
            });
        }
    }

    private void OnMouseUp()
    {
        _isSelected = false;
        var mousePos = Input.mousePosition;
        if (mousePos.y > Screen.currentResolution.height / 2f)
        {
            Debug.Log("Play card !");
            _isInHand = false;
        }
        else
        {
            Debug.Log("Not playing card !");
            transform.DOLocalMove(Vector2.zero, .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                _isOver = false;
            });
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.x = mousePoint.x/2f;
        mousePoint.y = mousePoint.y;
        mousePoint.z = zCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}