﻿using System;
using System.Collections.Generic;
using DatabaseStructure;

namespace Backend.Messages
{
    [Serializable]
    public class ChosenDishes : Message
    {
        public Dictionary<Dish, uint> DishesAndQuantity;

        public override MessageType MessageType => MessageType.ChosenDishes;
    }
}