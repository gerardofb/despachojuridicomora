using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WDXWebApiDespachoJuridico.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMessageService _messageService;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IMessageService messageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _messageService = messageService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Registrar(string email, string password, string confirmation)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { Message = "el correo y la contraseña son obligatorios" });
            }
            if(confirmation != password)
            {
                return BadRequest(new { Message = "la contraseña y su confirmación no coinciden" });
            }
            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };
            IdentityResult userCreation = null;
            try
            {
                userCreation = await _userManager.CreateAsync(newUser, password);
            }
            catch(SqlException ex)
            {
                return StatusCode(500, new { Message = "Error en la comunicación interna del servidor con la base de datos" });
            }
            if (!userCreation.Succeeded)
            {
                string Errores = String.Join(',',userCreation.Errors.Select(a=> a.Code));
                return StatusCode(500, new { Message = "Error en la creación del usuario", Errors = Errores });
            }
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var tokenUrl = Url.Action("VerifyEmail", "Account", new
            {
                Id = newUser.Id,
                Token = confirmationToken
            },Request.Scheme);
            string mensaje = $@"Gracias por registrarse en el sitio web del Despacho Jurídico Gómez Mora.
                            Por favor confirme su correo accediendo al siguiente <a href='{tokenUrl}'>vínculo</a>.";
            string mensajeHtml = $@"<p>Gracias por registrarse en el sitio web del Despacho Jurídico Gómez Mora.</p>
                            <p>Por favor confirme su correo accediendo al siguiente <a href='{tokenUrl}'>vínculo</a>.</p>";
            bool _exito = await _messageService.Send(newUser.Email, "Verificación de correo Despacho Gómez Mora", mensaje, mensajeHtml);
            if (_exito)
            {
                return Ok(new { Message = "El registro se realizó con éxito" });
            }
            return Ok(new { Message = "Hubo algunos problemas al realizar el registro" });
        }

        [HttpGet]
        [Route("verificarusuario")]
        public async Task<IActionResult> VerifyEmail(string Id, string Token)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                throw new InvalidOperationException();

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, Token);
            if (!emailConfirmationResult.Succeeded)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al verificar el correo electrónico, intente de nuevo más tarde" });
            }

            return Ok();
        }

        [Authorize(Policy ="Permiso")]
        [HttpDelete]
        [Route("borrar")]
        public async Task<IActionResult> DeleteUser(string UserName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(UserName);
                var logins = await _userManager.GetLoginsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                using (var transaction = new ModeloWDXDespacho().Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    }

                    if (roles.Count() > 0)
                    {
                        foreach (var item in roles.ToList())
                        {
                            // item should be the name of the role
                            var result = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }
                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }
                return Ok(new { Message = "Usuario borrado con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message= "Ocurrió un error al intentar borrar el usuario " + ex.Message.ToString()});
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password, bool persistente)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { Message = "El usuario y la contraseña son obligatorios" });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return BadRequest(new { Message = "Usuario o contraseña inválidos" });
            }
            if(user.EmailConfirmed == false)
            {
                return BadRequest(new { Message = "El usuario no ha completado su registro, por favor revise su correo de confirmación de registro" });
            }
            var passwordSignIn = await _signInManager.PasswordSignInAsync(user, password, persistente, false);
            if (!passwordSignIn.Succeeded)
            {
                return BadRequest(new { Message = "Usuario o contraseña inválidos" });
            }
            // crear cookie
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var identity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.
AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties();
            props.IsPersistent = persistente;
            HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            props).Wait();
            
            // fin de creación de cookie
            return Ok(new { Message = "Se autenticó el usuario con éxito" });
        }
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new
            {
                Message = "Ha cerrado sesión exitosamente"
            });
        }
    }
}
