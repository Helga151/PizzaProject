using Microsoft.EntityFrameworkCore;

namespace PizzaProject.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Topping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Pizza> Pizzas { get; set; } = new List<Pizza>();
    }
}
