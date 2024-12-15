namespace AttStat.Models
{
    class Order
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Order(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
