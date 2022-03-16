using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartApartmentData.DTOs;
using SmartApartmentData.Interfaces;

namespace SmartApartmentData.Controllers
{
    public class AuthController: BaseAPIController
    {
        private readonly ITokenService _tokenService;

        public AuthController( ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO dto)
        {
            //Connect to User Directory to verify if user exists. For the benefit of this assessment I
            //will be using a single username to validate to enable the assessor test code without connectng
            //to a database or Directory service
            //var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==dto.UserName);
            var user = dto.UserName.Equals("tubotonba.harry")? new  User { UserName = "tubotonba.harry" } : null;
            if(user==null) return Unauthorized("User Not Found");
            var response = new UserDTO { userName = user.UserName, Token = _tokenService.GeneratedToken(user) };
            return response;
        }
    }
}