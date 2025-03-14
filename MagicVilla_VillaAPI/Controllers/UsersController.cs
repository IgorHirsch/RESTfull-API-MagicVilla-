using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;


        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            // ➤ 1️ Login-Versuch mit den eingegebenen Daten
            var loginResponse = await _userRepo.Login(model);

            // ➤ 2️ Falls Benutzer nicht existiert oder kein Token erstellt wurde
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest; // ⛔ 400 - Bad Request
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response); // ❌ Antwort: Login fehlgeschlagen
            }

            // ➤ 3️ Falls Login erfolgreich war, wird das Token zurückgegeben
            _response.StatusCode = HttpStatusCode.OK; // ✅ 200 - OK
            _response.IsSuccess = true;
            _response.Result = loginResponse; // 📌 Enthält Token und Benutzerinfo
            return Ok(_response); // ✅ Antwort: Login erfolgreich
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            // 1️ Überprüfung, ob der Benutzername bereits existiert
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique) // Falls Benutzername bereits existiert, Fehlermeldung zurückgeben
            {
                _response.StatusCode = HttpStatusCode.BadRequest; // ⛔ 400 - Bad Request
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            // 2️ Registrierung des Benutzers in der Datenbank
            var user = await _userRepo.Register(model);
            if (user == null) // Falls die Registrierung fehlschlägt, Fehlermeldung zurückgeben
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }

            // 3️ Falls erfolgreich, Erfolgsantwort zurückgeben
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response); // ✅ Benutzer erfolgreich registriert
        }
    }
}
