using Auth.Domain.AppProjects.Entity;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.AuthHelper;
using Auth.Infrastracture.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.QueryHandlers.AppProjects
{
    public class CheckUrlPermissionHandler : ICheckUrlPermission
    {
        private readonly UserManager<AppUser> _userManager;

        public CheckUrlPermissionHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Permissions>> GetUserPermissions(Guid userId)
        {
            var access = _userManager.FindByIdAsync(userId.ToString()).Result.Groups
               .Select(u => u.Group).SelectMany(u=>u.Urls).ToList();
            var perms = new List<Permissions>();
            foreach (var url in access)
            {
                perms.Add(new Permissions { Url = url.Url.Url, Method = url.Url.Method });
            }
            return perms;
               
        }

        public async Task<bool> HasAccessAsync(Guid userId, string url, string method = "GET")
        {
            // Normalize URL (lowercase, remove trailing slashes)
            url = url.TrimEnd('/').ToLower();

            var access = _userManager.FindByIdAsync(userId.ToString()).Result.Groups
                .Select(u=>u.Group)
                .SelectMany(g => g.Urls)
                .Any(p => p.Url.Url.ToLower() == url && p.Url.Method == method);

            return access;
        }
    }
}

