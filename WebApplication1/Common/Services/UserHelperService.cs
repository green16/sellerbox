﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models.Database;

namespace WebApplication1.Common.Services
{
    public class UserHelperService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly DatabaseContext _context;

        public UserHelperService(UserManager<Users> userManager, SignInManager<Users> signInManager, DatabaseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public bool HasLoggedUser(ClaimsPrincipal user) => _signInManager.IsSignedIn(user);
        public string GetUserName(ClaimsPrincipal user) => _userManager.GetUserName(user);
        public string GetUserId(ClaimsPrincipal user) => _userManager.GetUserId(user);
        public async Task<string> GeVktUserAccessToken(ClaimsPrincipal user)
        {
            Users currentUser = await _userManager.GetUserAsync(user);
            return await _userManager.GetAuthenticationTokenAsync(currentUser, "Vkontakte", "access_token");
        }
        public async Task<DateTime?> GetVkUserAccessTokenExpiresAt(ClaimsPrincipal user)
        {
            Users currentUser = await _userManager.GetUserAsync(user);
            string value = await _userManager.GetAuthenticationTokenAsync(currentUser, "Vkontakte", "expires_at");
            if (!DateTime.TryParse(value, out DateTime result))
                return null;
            return result;
        }
        public async Task<int> GetUserIdVk(ClaimsPrincipal user)
        {
            Users currentUser = await _userManager.GetUserAsync(user);
            return currentUser.IdVk;
        }
        public KeyValuePair<int, string> GetSelectedGroup(ClaimsPrincipal user)
        {
            string idUser = _userManager.GetUserId(user);
            int idSelectedGroup = _context.Users.First(x => x.Id == idUser).IdCurrentGroup;

            return _context.Groups
                .Where(x => x.IdVk == idSelectedGroup)
                .Select(x => new KeyValuePair<int, string>(x.IdVk, x.Name))
                .FirstOrDefault();
        }
        public bool HasSelectedGroup(ClaimsPrincipal user)
        {
            string idUser = _userManager.GetUserId(user);
            return _context.Users.First(x => x.Id == idUser).IdCurrentGroup != 0;
        }
        public bool HasConnectedGroups(ClaimsPrincipal user)
        {
            string idUser = _userManager.GetUserId(user);
            return _context.GroupAdmins
                .Where(x => x.IdUser == idUser)
                .Include(x => x.Group)
                .Any(x => !string.IsNullOrEmpty(x.Group.AccessToken));
        }
        public IDictionary<int, string> GetConnectedGroups(ClaimsPrincipal user)
        {
            string idUser = _userManager.GetUserId(user);
            return _context.GroupAdmins.Where(x => x.IdUser == idUser)
                .Include(x => x.Group)
                .ToDictionary(x => x.Group.IdVk, x => x.Group.Name);
        }
    }
}
