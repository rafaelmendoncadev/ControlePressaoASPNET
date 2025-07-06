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
    public class PressaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PressaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pressao
        public async Task<IActionResult> Index()
        {
            var pressoes = await _context.Pressao
                .OrderByDescending(p => p.DataHora)
                .ToListAsync();
            return View(pressoes);
        }

        // GET: Pressao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressao = await _context.Pressao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pressao == null)
            {
                return NotFound();
            }

            return View(pressao);
        }

        // GET: Pressao/Create
        public IActionResult Create()
        {
            // Definir valor padrão para a data/hora atual
            var model = new Pressao
            {
                DataHora = DateTime.Now
            };
            return View(model);
        }

        // POST: Pressao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,Sistolica,Diastolica,FrequenciaCardiaca,Observacoes")] Pressao pressao)
        {
            // Validação customizada
            if (pressao.Sistolica <= pressao.Diastolica)
            {
                ModelState.AddModelError("Sistolica", "A pressão sistólica deve ser maior que a diastólica");
            }

            if (pressao.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(pressao);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de pressão salva com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar os dados: " + ex.Message);
                }
            }
            return View(pressao);
        }

        // GET: Pressao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressao = await _context.Pressao.FindAsync(id);
            if (pressao == null)
            {
                return NotFound();
            }
            return View(pressao);
        }

        // POST: Pressao/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,Sistolica,Diastolica,FrequenciaCardiaca,Observacoes")] Pressao pressao)
        {
            if (id != pressao.Id)
            {
                return NotFound();
            }

            // Validação customizada
            if (pressao.Sistolica <= pressao.Diastolica)
            {
                ModelState.AddModelError("Sistolica", "A pressão sistólica deve ser maior que a diastólica");
            }

            if (pressao.DataHora > DateTime.Now)
            {
                ModelState.AddModelError("DataHora", "A data e hora não podem ser futuras");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pressao);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medição de pressão atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PressaoExists(pressao.Id))
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
                    return View(pressao);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pressao);
        }

        // GET: Pressao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressao = await _context.Pressao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pressao == null)
            {
                return NotFound();
            }

            return View(pressao);
        }

        // POST: Pressao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pressao = await _context.Pressao.FindAsync(id);
            if (pressao != null)
            {
                _context.Pressao.Remove(pressao);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medição de pressão excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PressaoExists(int id)
        {
            return _context.Pressao.Any(e => e.Id == id);
        }
    }
}
