using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace Logrythmik.Mvc.Proxies
{
    public class RolesProxy : IRolesProxy
    {
        #region Implementation of IRolesProxy

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        /// <summary>
        /// Gets the roles for user.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public string[] GetRolesForUser(Guid userGuid)
        {
            return Roles.GetRolesForUser(userGuid.ToString());
        }

        public string[] GetRolesForUser(Guid userGuid, List<string> scopeNames)
        {
            return Roles.GetRolesForUser( userGuid.ToString() );
        }

        public Dictionary<string, string> GetLogrythmikPublicClaimsForUser(Guid userGuid, List<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public void UpdateLogrythmikPublicDataClaim( string userGuid, int ipcPublicScopeId, string identityName, string claimNs, string claimValue )
        {
            throw new NotImplementedException();
        }

        public void UpdatePublicScopeDataClaim( string userGuid, string identityGuid, string dataClaimNamespace, string newDataClaimValue)
        {
            throw new NotImplementedException();
        }

        public void AddLogrythmikPublicDataClaim(string userGuid, int ipcPublicScopeId, string identityName, string claimNs, string claimValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified user is in this role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>
        /// 	<c>true</c> if [is user in role] [the specified user GUID]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUserInRole(Guid userGuid, string roleName)
        {
            var roles = GetRolesForUser(userGuid);
            if (roles == null)
                return false;
            return roles.Contains(roleName);
        }

        public void RemoveUserFromRole(Guid userGuid, string roleName, int scopeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks to see if a role exists.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public bool RoleExists(string roleName)
        {
            var roles = GetAllRoles();
            if (roles == null)
                return false;
            return roles.Contains(roleName);
        }

        public void AddUserToRole(Guid userGuid, string roleName, int scopeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the user from role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        public void RemoveUserFromRole(Guid userGuid, string roleName)
        {
            Roles.RemoveUsersFromRoles(new [] { userGuid.ToString() }, new [] { roleName });
        }

        /// <summary>
        /// Adds the user to role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        public void AddUserToRole(Guid userGuid, string roleName)
        {
            Roles.AddUsersToRoles(new [] {userGuid.ToString() }, new[] { roleName });
        }

        #endregion
    }
}