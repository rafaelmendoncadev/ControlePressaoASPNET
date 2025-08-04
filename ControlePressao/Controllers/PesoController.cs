using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ControlePressao.Data;
using ControlePressao.Models;
using System.Security.Claims;

namespace ControlePressao.Controllers
{
    [Authorize]
    public class PesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Peso
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var pesos = await _context.Peso
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataHora)
                .ToListAsync();
            return View(pesos);
        }

        // GET: Peso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var peso = await _context.Peso
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (peso == null)
            {
                return NotFound();
            }

            return View(peso);
        }

        // GET: Peso/Create
        public IActionResult Create()
        {
            var model = new Peso
            {
                DataHora = DateTime.Now
            };
            return View(model);
        }

        // POST: Peso/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,Altura,PesoKg,Observacoes")] Peso peso)
        {
            if (peso.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    peso.UserId = GetCurrentUserId();
                    _context.Add(peso);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de peso salva com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar os dados: " + ex.Message);
                }
            }
            return View(peso);
        }

        // GET: Peso/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var peso = await _context.Peso
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (peso == null)
            {
                return NotFound();
            }
            return View(peso);
        }

        // POST: Peso/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,Altura,PesoKg,Observacoes")] Peso peso)
        {
            if (id != peso.Id)
            {
                return NotFound();
            }

            if (peso.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var existingPeso = await _context.Peso
                        .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                    
                    if (existingPeso == null)
                    {
                        return NotFound();
                    }

                    existingPeso.DataHora = peso.DataHora;
                    existingPeso.Altura = peso.Altura;
                    existingPeso.PesoKg = peso.PesoKg;
                    existingPeso.Observacoes = peso.Observacoes;

                    _context.Update(existingPeso);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de peso atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PesoExists(peso.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar os dados: " + ex.Message);
                    return View(peso);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(peso);
        }

        // GET: Peso/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var peso = await _context.Peso
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (peso == null)
            {
                return NotFound();
            }

            return View(peso);
        }

        // POST: Peso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var peso = await _context.Peso
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (peso != null)
            {
                _context.Peso.Remove(peso);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medição de peso excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PesoExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Peso.Any(e => e.Id == id && e.UserId == userId);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Valor padrão para admin
        }
    }
}