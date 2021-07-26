using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    [SerializeField] private int value;
    [SerializeField] private string nameCard;
    [SerializeField] private Material cardMat;
    [SerializeField] private Color colorValue;
    [SerializeField] private SymbolCard symbol;
    public enum Color
    {
        Red,
        Black
    }

    public enum SymbolCard
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    }



    public int Value => value;

    public string NameCard => nameCard;

    public Color ColorValue => colorValue;

    public SymbolCard Symbol => symbol;

    public Material CardMat => cardMat;
}