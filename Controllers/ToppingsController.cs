using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaProject;
using PizzaProject.Models;

namespace PizzaProject.Controllers
{
    public class ToppingsController : Controller
    {
        private readonly DataContext _context;

        public ToppingsController(DataContext context)
        {
            _context = context;
        }

        // GET: Toppings
        public async Task<IActionResult> Index(string search)
        {
            if (!string.IsNullOrEmpty(TempData["ErrorMessage"]?.ToString()))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            IEnumerable<Topping> toppings = _context.Toppings.Include("Pizzas");
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                toppings = toppings.Where(topping =>
                    topping.Name.ToLower().Contains(search)
                );
            }

            ViewBag.Search = search;

            return View(toppings);
        }

        // GET: Toppings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var topping = await _context.Toppings.Include("Pizzas").FirstOrDefaultAsync(m => m.Id == id);
            return topping != null ? View(topping) : NotFound();
        }

        // GET: Toppings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Toppings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Topping topping)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topping);
        }

        // GET: Toppings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var topping = await _context.Toppings.FindAsync(id);
            return topping != null ? View(topping) : NotFound();
        }

        // POST: Toppings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Topping topping)
        {
            if (id != topping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToppingExists(topping.Id))
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
            return View(topping);
        }

        // GET: Toppings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var topping = await _context.Toppings.Include("Pizzas").FirstOrDefaultAsync(m => m.Id == id);

            if (topping != null)
            {
                if (topping.Pizzas.Count > 0)
                {
                    TempData["ErrorMessage"] = $"Topping {topping.Name} cannot be deleted, because it is assigned to one or more pizza.";
                    return RedirectToAction(nameof(Index));
                }

                return View(topping);
            }
            return NotFound();
        }

        // POST: Toppings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topping = await _context.Toppings.FindAsync(id);

            if (topping != null)
            {
                _context.Toppings.Remove(topping);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToppingExists(int id)
        {
            return _context.Toppings.Any(e => e.Id == id);
        }
    }
}
