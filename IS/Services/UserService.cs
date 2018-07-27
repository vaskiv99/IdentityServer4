using System;
using System.Threading.Tasks;
using IS.Data;
using IS.Models;
using IS.Models.UserDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IS.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> UpdateUserAsync(Guid id, UpdateUserDto updateUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.ToString()).ConfigureAwait(false);
            if (updateUser.FirstName != default(string))
                user.FirstName = updateUser.FirstName;
            if (updateUser.LastName != default(string))
                user.LastName = updateUser.FirstName;
            if (updateUser.UserName != default(string))
                user.UserName = updateUser.UserName;

            return user;
        }
        public async Task<bool> SaveAsync()
        {
            var result = await _context.SaveChangesAsync().ConfigureAwait(false);
            return result >= 0;
        }
    }
}
