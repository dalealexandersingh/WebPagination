using System.ComponentModel.DataAnnotations;

namespace WebPagination.Database
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string Name { get; set; }
    }
}
