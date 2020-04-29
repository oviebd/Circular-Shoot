﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemModel
{
    public string itemName;
    public GameEnum.PlayerrTType itemType;
    public Sprite itemImage;
    public bool isPurchased = false;
    public int price = 0;
    public int playerId = 0;
}
