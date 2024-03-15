using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaProject;
using PizzaProject.Models;

namespace PizzaProject.Controllers
{
    public class PizzasController : Controller
    {
        private readonly DataContext _context;

        public PizzasController(DataContext context)
        {
            _context = context;
        }

        // GET: Pizzas
        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            IEnumerable<Pizza> pizzas = _context.Pizzas.Include("Toppings");
            if(!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                pizzas = pizzas.Where(pizza =>
                    pizza.Name.ToLower().StartsWith(search)
                );
            }

            ViewBag.Search = search;

            return View(pizzas);
        }

        // GET: Pizzas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pizza = await _context.Pizzas.Include("Toppings").FirstOrDefaultAsync(m => m.Id == id);
            return pizza != null ? View(pizza) : NotFound();
        }

        // GET: Pizzas/Create
        public IActionResult Create()
        {
            ViewBag.ToppingsName = _context.Toppings.Select(t => t.Name).ToList();
            return View();
        }

        // POST: Pizzas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Size,Dough")] Pizza pizza, [Bind("Toppings")] string[] Toppings)
        {
            if (ModelState.IsValid)
            {
                var listOfToppings = Toppings
                    .Select(name => name.Trim())
                    .Join(_context.Toppings,
                        name => name,
                        t => t.Name,
                        (name, t) => t)
                    .ToList();

                pizza.Toppings = listOfToppings;

                _context.Add(pizza);
                _context.SaveChanges();

                //update pizza attribute in each assigned topping; after SaveChanges() to get pizza ID 
                pizza = _context.Pizzas.FirstOrDefault(m => m.Id == pizza.Id);
                //pizza = _context.Pizzas.Include("Toppings").FirstOrDefault(m => m.Id == pizza.Id);

                //foreach(Topping topping in listOfToppings)
                //{
                //    topping.Pizzas.Add(pizza);
                //    _context.Toppings.Update(topping);
                //}

                //_context.SaveChanges();

                //_context.Pizzas.Update(pizza);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pizza);
        }

        // GET: Pizzas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            ViewBag.ToppingsName = _context.Toppings.Select(t => t.Name).ToList();

            var pizza = await _context.Pizzas.Include("Toppings").FirstOrDefaultAsync(m => m.Id == id);
            return pizza != null ? View(pizza) : NotFound();
        }

        // POST: Pizzas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Size,Dough")] Pizza pizza)
        {
            if (id != pizza.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pizza);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PizzaExists(pizza.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pizza);
        }

        // GET: Pizzas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pizza = await _context.Pizzas.Include("Toppings").FirstOrDefaultAsync(m => m.Id == id);
            return pizza != null ? View(pizza) : NotFound();
        }

        // POST: Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza != null)
            {
                _context.Pizzas.Remove(pizza);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PizzaExists(int id)
        {
            return _context.Pizzas.Any(e => e.Id == id);
        }
    }
}
