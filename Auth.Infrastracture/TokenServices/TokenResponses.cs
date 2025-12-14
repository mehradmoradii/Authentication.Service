using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.TokenServices
{
    public sealed record TokenPair(string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry);

    public sealed record RefreshTokenRecord(Guid UserId, string? Ip, DateTime ExpiresAt);

}
