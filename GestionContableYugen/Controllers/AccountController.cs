using GestionContableYugen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // GET: /Account/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();  // Limpiar sesión
        return RedirectToAction("Login", "Account");

    }


    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // Incluye el rol asociado
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario && u.Contrasena == model.Contrasena);

            if (usuario != null)
            {
                Console.WriteLine($"🔹 Usuario autenticado: {usuario.NombreUsuario}, ID: {usuario.UsuarioID}, Rol: {usuario.Rol.NombreRol}");

                // Guardar en sesión
                HttpContext.Session.SetString("Usuario", usuario.NombreUsuario);
                HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID);
                HttpContext.Session.SetString("Rol", usuario.Rol.NombreRol);

                Console.WriteLine($" Usuario encontrado: {(usuario != null ? usuario.NombreUsuario : "Ninguno")}");
                Console.WriteLine($" Rol encontrado: {(usuario != null ? usuario.Rol.NombreRol : "Ninguno")}");


                //  AUTENTICACIÓN CON COOKIES
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.NombreRol)  // Guarda el nombre del rol, no el ID
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, //  Mantiene la sesión activa aunque la pestaña se cierre
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity), authProperties);

                Console.WriteLine(" Usuario autenticado correctamente.");
                Console.WriteLine($"Usuario en sesión después de Login: {HttpContext.Session.GetString("Usuario")}");
                Console.WriteLine($"Rol en sesión después de Login: {HttpContext.Session.GetString("Rol")}");

                //  Redirigir según el rol del usuario
                if (usuario.Rol.NombreRol == "Administrador")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (usuario.Rol.NombreRol == "Asistente")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (usuario.Rol.NombreRol == "Cajero")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (usuario.Rol.NombreRol == "Bodeguero")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                Console.WriteLine(" Error: Credenciales incorrectas.");
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            }
        }
        return View(model);
    }
}
