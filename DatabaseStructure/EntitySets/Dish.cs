using System;

namespace DatabaseStructure.EntitySets
{
    [Serializable]
    public class Dish
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}