using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.AuthHelper
{
    public interface ICheckUrlPermission
    {
        Task<bool> HasAccessAsync(Guid userId, string url, string method = "GET");
        Task<List<Permissions>> GetUserPermissions(Guid userId);
    }
}
