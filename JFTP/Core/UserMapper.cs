using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;

namespace JFTP.Core
{
    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new UserIdentity()
            {
                UserName = "bob",
            };
        }

        public static Guid? ValidateUser(string username, string password)
        {
            return Guid.NewGuid();
        }
    }
}