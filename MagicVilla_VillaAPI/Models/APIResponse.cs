using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; } // Der HTTP-Statuscode der Antwort (z. B. 200, 400, 500)
        public bool IsSuccess { get; set; } = true; // Gibt an, ob die Anfrage erfolgreich war (true) oder fehlgeschlagen ist (false)
        public List<string> ErrorMessages { get; set; } // Eine Liste von Fehlermeldungen, falls die Anfrage fehlschlägt
        public object Result { get; set; } // Das eigentliche Ergebnis der API-Antwort (z. B. ein Benutzerobjekt oder eine Liste)
    }
}
