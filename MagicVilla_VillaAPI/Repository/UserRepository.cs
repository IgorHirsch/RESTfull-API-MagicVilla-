using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }


        public bool IsUniqueUser(string username)
        {
            // 1️ Suche nach einem Benutzer in der Datenbank, der den gleichen Benutzernamen hat.
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);

            // 2️ Wenn kein Benutzer mit diesem Benutzernamen gefunden wurde (d.h. der Name ist einzigartig)
            if (user == null)
            {
                return true; // Benutzername ist einzigartig
            }

            // 3️⃣ Wenn ein Benutzer mit diesem Benutzernamen gefunden wurde
            return false; // Benutzername ist nicht einzigartig
        }


        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            // 🔍 Suche nach einem Benutzer in der Datenbank, dessen Benutzername und Passwort übereinstimmen
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() 
                                                                                    == loginRequestDTO.UserName.ToLower() && u.Password 
                                                                                    == loginRequestDTO.Password);

                // ❌ Falls kein Benutzer gefunden wird, geben wir eine leere Antwort zurück
                if (user == null)
                {
                    return new LoginResponseDTO()
                    {
                        Token = "", // Kein Token, weil Anmeldung fehlgeschlagen ist
                        User = null // Kein Benutzerobjekt
                    };
                }

            // ✅ Benutzer wurde gefunden → Jetzt wird ein JWT-Token generiert
            var tokenHandler = new JwtSecurityTokenHandler(); // Erzeugt ein JWT-Token-Handler-Objekt
            var key = Encoding.ASCII.GetBytes(secretKey);  // Konvertiert den geheimen Schlüssel in ein Byte-Array //secretKey = configuration.GetValue<string>("ApiSettings:Secret");

            // 🔐 Token-Konfiguration erstellen
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()), // Speichert die Benutzer-ID als Claim
                    new Claim(ClaimTypes.Role, user.Role) // Speichert die Benutzerrolle als Claim
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token ist 7 Tage gültig
                SigningCredentials = new(new SymmetricSecurityKey(key), // Verwende den geheimen Schlüssel
                                                                      SecurityAlgorithms.HmacSha256Signature) // Verwende HMAC SHA256 für die Signatur
            };

            // 🔑 JWT-Token basierend auf der Token-Beschreibung erstellen
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 🔄 Das erstellte Token in eine Zeichenkette umwandeln
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO() 
            {
                Token = tokenHandler.WriteToken(token), // Token in lesbare Form umwandeln
                User = user // Benutzerinformationen zurückgeben
            };

            // 📤 Rückgabe der Login-Antwort mit Token und Benutzerinformationen
            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            // 1️ Neuen Benutzer aus den übergebenen Registrierungsdaten erstellen
            LocalUser user = new()
            {
                UserName = registerationRequestDTO.UserName, // Benutzername setzen
                Password = registerationRequestDTO.Password,    // (⚠️ Problem: Passwort wird in Klartext gespeichert!)
                Name = registerationRequestDTO.Name,               // Name des Benutzers
                Role = registerationRequestDTO.Role                      // Benutzerrolle (z.B. "Admin", "User")
            };

            // 2️ Benutzer in die Datenbank hinzufügen
            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync(); // Änderungen speichern

            // 3️ Sicherheitsmaßnahme: Passwort aus der Antwort entfernen
            user.Password = "";

            // 4️ Benutzer zurückgeben (aber ohne Passwort)
            return user;
        }
    }
}
