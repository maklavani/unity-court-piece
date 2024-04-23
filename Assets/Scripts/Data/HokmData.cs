using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CardData
{
    public int number;
    public Cards type;
    public Sprite image;
}

[Serializable]
public class MoveData
{
	public Cards type;
    public int number;
    public int turn;
}

[Serializable]
public class TableGameData
{
    public List<MoveData> moves;
    public int first;
    public int win;
}

[Serializable]
public class Card
{
    public int number;
    public Cards type;
}

public class Animation
{
    public bool status;
    public GameObject go;
    public Vector3 targetPosition;
    public float rotate;
	public Vector2 scale;
	public float speed;
	public bool sound;
}