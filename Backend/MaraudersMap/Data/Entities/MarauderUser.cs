using System.ComponentModel.DataAnnotations.Schema;

namespace MaraudersMap.Data.Entities
{
    public class MarauderUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }

        public MarauderUser()
        {
            Name = "";
            Coordinates = "";
        }
    }
}
