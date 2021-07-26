using System;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : NetworkBehaviour
{
  public static UIManager instance;
  public Hand playerHand;

  private void Awake()
  {
    instance = this;
  }

  private void Update()
  {
    // Debug.Log(IsMouseOverUI());
  }
}