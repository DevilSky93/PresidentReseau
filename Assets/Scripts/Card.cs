using System;
using System.Collections.Generic;
using DG.Tweening;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour
{
    public CardData card;
    public MeshRenderer theRenderer;
    public Hand isInHandOf;

    private int value;
    private string nameCard;
    private CardData.Color colorValue;
    private CardData.SymbolCard symbol;

    private bool _isInHand = true;
    private bool _isOver, _isSelected;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private float zCoord;

    public void InitCard(Hand h)
    {
        isInHandOf = h;
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
            transform.DOLocalMove(new Vector3(0, .2f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
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
        _offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        var mousePos = Input.mousePosition;
        
        transform.position = new Vector3(GetMouseWorldPos().x + _offset.x, GetMouseWorldPos().y + _offset.y, transform.position.z);
    }

    private void OnMouseExit()
    {
        if (_isInHand && !_isSelected)
        {
            transform.DOLocalMove(new Vector3(0f, 0f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                _isOver = false;
            });
        }
    }

    private void OnMouseUp()
    {
        // _isSelected = false;
        var mousePos = Input.mousePosition;
        if (mousePos.y > Screen.currentResolution.height / 2f)
        {
            // if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            //     out var networkedClient))
            // {
            //     PlayCardServerRpc();
            // }
            Debug.Log("Play card !");
            _isInHand = false;
            _isSelected = false;
        }
        else
        {
            Debug.Log("Not playing card !");

            transform.DOLocalMove(Vector2.zero, .2f).SetEase(Ease.OutCubic);
            
            if (isInHandOf.selectedCards.Find(x=>x.nameCard == nameCard) != null)
            {
                isInHandOf.selectedCards.Remove(this);
                _isSelected = false;
                transform.DOLocalMove(new Vector3(0f, 0f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
                {
                    _isOver = false;
                });
                return;
            }
            
            if (isInHandOf.selectedCards.Count > 0)
            {
                if (isInHandOf.selectedCards[0].value == value)
                {
                    isInHandOf.selectedCards.Add(this);
                }
                else
                {
                    _isSelected = false;
                    transform.DOLocalMove(new Vector3(0f, 0f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
                    {
                        _isOver = false;
                    });
                }
            }
            else if(isInHandOf.selectedCards.Find(x=> x.nameCard == nameCard) == null)
            {
                isInHandOf.selectedCards.Add(this);
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.x = mousePoint.x/1.5f;
        mousePoint.y = mousePoint.y*1.5f;
        mousePoint.z = zCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    [ServerRpc]
    private void PlayCardServerRpc()
    {
        // GameManager.instance.PlayCard(isInHandOf);
    }
}