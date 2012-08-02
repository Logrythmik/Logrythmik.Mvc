using System;
using System.Collections.Generic;
using System.Web.Security;

namespace Logrythmik.Mvc.Proxies
{
    public interface IMembershipProxy
    {
        MembershipUser CreateUser(string userName, string password, string email, out MembershipCreateStatus status);

        MembershipUser GetUser(Guid userGuid);
        MembershipUser GetUser(string userName);

        void UpdateUser(MembershipUser user);
        //void UpdateUserCredential(string userGuid, string username, string email);
        void DeleteUser(Guid userGuid);

        bool ValidateUser(Guid userGuid, string password);
        bool ValidateUser( string userName, string password, List<string> scopeNames );

        bool ChangePassword(Guid userGuid, string oldPassword, string newPassword);
        string ResetPassword(Guid userGuid);
        
        bool UnlockUser(Guid userGuid);

        MembershipUserCollection FindUsersByEmail(string email, int pageIndex, int pageSize, out int totalRecords);
        MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);
        MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords);
        string GetUserNameByEmail(string email);
        int MinPasswordLength { get; }
        
        string GeneratePassword();
        bool ValidatePasswordStrength(string password);
    }
}
