using Microsoft.EntityFrameworkCore;

namespace PizzaProject.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Size Size { get; set; }
        public Dough Dough { get; set; }
        public List<Topping> Toppings { get; set; } = new List<Topping>();
    }
}
