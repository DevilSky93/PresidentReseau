using System;
using System.Collections.Generic;
using DG.Tweening;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour
{
    public CardData card;
    public MeshRenderer theRenderer;
    public Hand isInHandOf;
    public BoxCollider collider;
    
    public NetworkVariable<int> value = new NetworkVariable<int>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariable<string> nameCard = new NetworkVariable<string>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariable<CardData.Color> colorValue = new NetworkVariable<CardData.Color>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariable<CardData.SymbolCard> symbol = new NetworkVariable<CardData.SymbolCard>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    private bool _isInHand = true;
    private bool _isOver, _isSelected;

    private Vector3 _offset;
    private float _zCoord;
    
    public NetworkVariableVector3 position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    private void Update()
    {
        if (_isInHand)
        {
            transform.localPosition = position.Value;
        }
    }

    public void InitCard(Hand h)
    {
        isInHandOf = h;
        theRenderer.material = card.CardMat;
        value.Value = card.Value;
        nameCard.Value = card.NameCard;
        colorValue.Value = card.ColorValue;
        symbol.Value = card.Symbol;
    }

    private void OnMouseEnter()
    {
        if (_isInHand && !_isOver)
        {
            transform.DOLocalMove(new Vector3(0, .2f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                _isOver = true;
            }).OnUpdate(() =>
            {
                position.Value = transform.localPosition;
            });
        }
    }
    
    private void OnMouseDown()
    {
        _zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        _isSelected = !_isSelected;
        _offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        var mousePos = Input.mousePosition;
        
        transform.position = new Vector3(GetMouseWorldPos().x + _offset.x, GetMouseWorldPos().y + _offset.y, transform.position.z);
    }

    private void OnMouseExit()
    {
        // if (_isInHand && !_isSelected)
        // {
        //     transform.DOLocalMove(new Vector3(0f, 0f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
        //     {
        //         _isOver = false;
        //     }).OnUpdate(() =>
        //     {
        //         position.Value = transform.localPosition;
        //     });
        // }
    }

    private void OnMouseUp()
    {
        var mousePos = Input.mousePosition;
        if (mousePos.y > Screen.height / 2f)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<Player>().hand;
                player.PlayCardServerRpc();
            }
        }
        else
        {
            Debug.Log("Not playing card !");
            // If other card selected and same value
            if (isInHandOf.selectedCards.Find(x=>x.nameCard == nameCard) != null)
            {
                isInHandOf.selectedCards.Remove(this);
                _isSelected = false;
                transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.OutCubic).OnStart(() =>
                {
                    _isOver = false;
                }).OnUpdate(() =>
                {
                    position.Value = transform.localPosition;
                });
                return;
            }
            
            if (isInHandOf.selectedCards.Count > 0)
            {
                if (isInHandOf.selectedCards[0].value.Value == value.Value)
                {
                    isInHandOf.selectedCards.Add(this);
                }
                else
                {
                    _isSelected = false;
                    transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.OutCubic).OnStart(() =>
                    {
                        _isOver = false;
                    }).OnUpdate(() =>
                    {
                        position.Value = transform.localPosition;
                    });
                }
            }
            else if(isInHandOf.selectedCards.Find(x=> x.nameCard.Value == nameCard.Value) == null)
            {
                isInHandOf.selectedCards.Add(this);
                if (Vector2.Distance(Vector2.zero, new Vector2(transform.localPosition.x, transform.localPosition.y)) > .5f)
                {
                    transform.DOLocalMove(Vector2.zero, .2f).SetEase(Ease.OutCubic).OnUpdate(() =>
                    {
                        position.Value = transform.localPosition;
                    });                    
                }
                else
                {
                    transform.DOLocalMove(new Vector3(0, .2f, transform.localPosition.z), .2f).SetEase(Ease.OutCubic).OnStart(() =>
                    {
                        _isOver = true;
                    }).OnUpdate(() =>
                    {
                        position.Value = transform.localPosition;
                    });
                }
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.x = mousePoint.x/1.4f;
        mousePoint.y = mousePoint.y*1.5f;
        mousePoint.z = _zCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void CardPlay()
    {
        _isInHand = false;
        _isSelected = false;
    }
    
    public void CardCancelled()
    {
        _isSelected = false;
    }
}