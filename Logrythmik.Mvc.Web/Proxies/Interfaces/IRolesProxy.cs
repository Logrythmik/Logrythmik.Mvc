using System;
using System.Collections.Generic;

namespace Logrythmik.Mvc.Proxies
{
    public interface IRolesProxy
    {
        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns></returns>
        string[] GetAllRoles();

        /// <summary>
        /// Gets the roles for user.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        string[] GetRolesForUser(Guid userGuid);

        string[] GetRolesForUser( Guid userGuid, List<string> scopeNames );

        Dictionary<string, string> GetLogrythmikPublicClaimsForUser( Guid userGuid, List<string> scopeNames );

        void UpdateLogrythmikPublicDataClaim( string userGuid, int ipcPublicScopeId, string identityName, string claimNs, string claimValue );

        /// <summary>
        /// Determines whether the specified user is in this role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>
        /// 	<c>true</c> if [is user in role] [the specified user GUID]; otherwise, <c>false</c>.
        /// </returns>
        bool IsUserInRole(Guid userGuid, string roleName);

        /// <summary>
        /// Adds the user to role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        void AddUserToRole(Guid userGuid, string roleName);

        /// <summary>
        /// Adds the user to role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="scopeId">The scope id of the event.</param>
        void AddUserToRole( Guid userGuid, string roleName, int scopeId );

        /// <summary>
        /// Removes the user from role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        void RemoveUserFromRole(Guid userGuid, string roleName);

        /// <summary>
        /// Removes the user from role.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="scopeId">The scope id of the event.</param>
        void RemoveUserFromRole( Guid userGuid, string roleName, int scopeId );

        /// <summary>
        /// Checks to see if a role exists.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        bool RoleExists(string roleName);
    }
}