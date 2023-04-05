using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService) //dependency injection: to instantiate a class(DataContext class) by injecting it into the contructor of the class( AccountController class) so we can use DataContext there which is inherited from DbContext
        {
            _tokenService = tokenService;
            _context = context;
            
        }

        [HttpPost("register")] // POST: api/accounts/register

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.Username))

                return BadRequest("Username is taken");

             using var hmac = new HMACSHA512();

             var user = new AppUser
             {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
             };

             _context.Users.Add(user);
             await _context.SaveChangesAsync();

              return new UserDto
             {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
             };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login (LoginDto loginDto)
        {
             var user = await _context.Users.SingleOrDefaultAsync(x =>
             x.UserName == loginDto.Username);

             if (user == null) return Unauthorized("invalid username");

             using var hmac = new HMACSHA512(user.PasswordSalt); // hmac.Key

             var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

             for (int i = 0; i < computedHash.Length; i++)
             {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
             }
             
             return new UserDto
             {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
             };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}