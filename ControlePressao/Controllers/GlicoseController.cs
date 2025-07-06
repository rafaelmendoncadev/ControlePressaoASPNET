using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlePressao.Data;
using ControlePressao.Models;

namespace ControlePressao.Controllers
{
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
            var glicoses = await _context.Glicose
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

            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(m => m.Id == id);
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

            var glicose = await _context.Glicose.FindAsync(id);
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
                    _context.Update(glicose);
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

            var glicose = await _context.Glicose
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var glicose = await _context.Glicose.FindAsync(id);
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
            return _context.Glicose.Any(e => e.Id == id);
        }
    }
}
