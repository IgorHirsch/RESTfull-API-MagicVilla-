using MagicVilla_VillaAPI.Controllers.Models;
using MagicVilla_VillaAPI.Controllers.Models.Dto;
using MagicVilla_VillaAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/villaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<VillaDTO> GetVillas()
        {
            return VillaStore.villaList;
        }

        [HttpGet("id")]
        public VillaDTO GetVilla(int id)
        {
            return VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        }
    }
}
