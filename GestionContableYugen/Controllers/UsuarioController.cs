using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using GestionContableYugen.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using GestionContableYugen.ViewModels;
using Microsoft.AspNetCore.Authorization;


namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsuarioController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.Include(u => u.Rol).ToListAsync();
            return View(usuarios);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Roles, "RolID", "NombreRol");
            return View();
        }



        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel usuarioViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Roles, "RolID", "NombreRol");
                return View(usuarioViewModel);
            }

            // Mapeamos el ViewModel a la entidad Usuario usando AutoMapper
            var usuario = _mapper.Map<Usuario>(usuarioViewModel);
            usuario.RolID = usuarioViewModel.RolID;  // Aseguramos que solo asignamos el ID del Rol, no un objeto Rol nuevo
            usuario.FechaRegistro = DateTime.Now;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();  // Guardamos en la base de datos

            return RedirectToAction(nameof(Index));  // Redirigimos a la lista de usuarios
        }


        // GET: Usuario/Edit/{id}

        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioViewModel = new UsuarioViewModel
            {
                UsuarioID = usuario.UsuarioID,
                NombreUsuario = usuario.NombreUsuario,
                Contrasena = usuario.Contrasena,
                Estado = usuario.Estado,
                RolID = usuario.RolID
            };



            ViewBag.Roles = new SelectList(_context.Roles, "RolID", "NombreRol", usuarioViewModel.RolID);

            return View(usuarioViewModel);
        }


        // POST: Usuario/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel usuarioViewModel)
        {
            if (id != usuarioViewModel.UsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.NombreUsuario = usuarioViewModel.NombreUsuario;
                usuario.Estado = usuarioViewModel.Estado;
                usuario.RolID = usuarioViewModel.RolID;

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(_context.Roles, "RolID", "NombreRol", usuarioViewModel.RolID);
            return View(usuarioViewModel);
        }


        // GET: Usuario/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Estado = false; 
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Reactivar
        [HttpPost]
        public async Task<IActionResult> Reactivar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Estado = true; // Reactivar usuario
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}


