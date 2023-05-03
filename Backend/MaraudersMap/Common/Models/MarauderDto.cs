namespace MaraudersMap.Common.Models
{
    public class MarauderDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int LastUpdate { get; set; }
        public bool IsActivated { get; set; }
        public string? Coordinates { get; set; }
    }
}
