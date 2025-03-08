using MagicVilla_VillaAPI.Controllers.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
        {
            new VillaDTO { Id = 1, Name = "Villa 1" },
            new VillaDTO { Id = 2, Name = "Villa 2" },
            new VillaDTO { Id = 3, Name = "Villa 3" },
            new VillaDTO { Id = 4, Name = "Villa 4" },
            new VillaDTO { Id = 5, Name = "Villa 5" }
        };

    }
}
