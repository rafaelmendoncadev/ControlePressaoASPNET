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
    public class GlicoseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GlicoseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Glicose
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var glicoses = await _context.Glicose
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.DataHora)
                .ToListAsync();
            return View(glicoses);
        }

        // GET: Glicose/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (glicose == null)
            {
                return NotFound();
            }

            return View(glicose);
        }

        // GET: Glicose/Create
        public IActionResult Create()
        {
            var model = new Glicose
            {
                DataHora = DateTime.Now
            };
            return View(model);
        }

        // POST: Glicose/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,Valor,Periodo,Observacoes")] Glicose glicose)
        {
            if (glicose.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    glicose.UserId = GetCurrentUserId();
                    _context.Add(glicose);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de glicose salva com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar os dados: " + ex.Message);
                }
            }
            return View(glicose);
        }

        // GET: Glicose/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (glicose == null)
            {
                return NotFound();
            }
            return View(glicose);
        }

        // POST: Glicose/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,Valor,Periodo,Observacoes")] Glicose glicose)
        {
            if (id != glicose.Id)
            {
                return NotFound();
            }

            if (glicose.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var existingGlicose = await _context.Glicose
                        .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
                    
                    if (existingGlicose == null)
                    {
                        return NotFound();
                    }

                    existingGlicose.DataHora = glicose.DataHora;
                    existingGlicose.Valor = glicose.Valor;
                    existingGlicose.Periodo = glicose.Periodo;
                    existingGlicose.Observacoes = glicose.Observacoes;

                    _context.Update(existingGlicose);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de glicose atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlicoseExists(glicose.Id))
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
                    return View(glicose);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(glicose);
        }

        // GET: Glicose/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (glicose == null)
            {
                return NotFound();
            }

            return View(glicose);
        }

        // POST: Glicose/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (glicose != null)
            {
                _context.Glicose.Remove(glicose);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medição de glicose excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GlicoseExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Glicose.Any(e => e.Id == id && e.UserId == userId);
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
