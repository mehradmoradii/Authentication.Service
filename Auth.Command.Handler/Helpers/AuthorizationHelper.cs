//using Auth.Domain.AppUsers.Aggregate;
//using Auth.Infrastracture.Repository;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Auth.Command.Handler.Authorization
//{
//    public class AuthorizationHelper
//    {
        
//            private readonly UserManager<AppUser> _userRepository;
            

//            public AuthorizationHelper(UserManager<AppUser> userRepository )
//            {
//                _userRepository = userRepository;
//            }

//            public async Task<bool> UserHasPermissionAsync(Guid userId, string url, string method)
//            {
//                var user = await _userRepository.FindByIdAsync(userId.ToString());
//                if (user == null) return false;

//                // Get all AppUrls accessible by user's groups
//                //var permittedUrls = await _projectRepository.GetUrlsForGroupsAsync(user.Groups.Select(g => g.GroupId));
//                var permittedUrls = user.Groups

//                // Check if the requested URL & method is in permitted URLs
//                return permittedUrls.Any(u => u.Url == url && u.Method == method);
//            }
//        }

//    }
//}
