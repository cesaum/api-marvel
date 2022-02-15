using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using APIMarvel.MarvelApi;

namespace APIMarvel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarvelController
    {
        public MarvelController()
        {
        }

        [HttpGet]
        public async Task<CharacterDataWrapper> GetCharacter() {
            return await new Marvel().GetCharacters();
        }
        
    }
}
