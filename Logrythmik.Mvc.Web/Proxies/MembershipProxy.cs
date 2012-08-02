
using System;
using System.Collections.Generic;
using System.Web.Security;

using Microsoft.Practices.Unity;

namespace Logrythmik.Mvc.Proxies
{
    public class MembershipProxy : IMembershipProxy
    {
        private readonly MembershipProvider _Provider;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipProxy"/> class.
        /// </summary>
        [InjectionConstructor]
        public MembershipProxy()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipProxy"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public MembershipProxy(MembershipProvider provider)
        {
            _Provider = provider ?? Membership.Provider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>The length of the min password.</value>
        public int MinPasswordLength
        {
            get { return _Provider.MinRequiredPasswordLength; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public MembershipUser CreateUser(string userName, string password, string email, out MembershipCreateStatus status)
        {
            return _Provider.CreateUser(userName, password, email, null, null, true, null, out status);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public MembershipUser GetUser(Guid userGuid)
        {
            return _Provider.GetUser(userGuid, true);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        public MembershipUser GetUser(string userName)
        {
            return _Provider.GetUser(userName, true);
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidateUser(Guid userGuid, string password)
        {
            return _Provider.ValidateUser(userGuid.ToString(), password);
        }

        public bool ValidateUser( string userName, string password, List<string> scopeNames )
        {
            return _Provider.ValidateUser(userName, password);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public bool ChangePassword(Guid userGuid, string oldPassword, string newPassword)
        {
            var currentUser = _Provider.GetUser(userGuid, false);
            if (currentUser != null && currentUser.ProviderUserKey != null)
                return _Provider.ChangePassword(currentUser.ProviderUserKey.ToString(), oldPassword, newPassword);
            return false;
        }


        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userGuid">Name of the user.</param>
        public void DeleteUser(Guid userGuid)
        {
            _Provider.DeleteUser(userGuid.ToString(), true);
        }

        /// <summary>
        /// Finds the users by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public MembershipUserCollection FindUsersByEmail(string email, int pageIndex, int pageSize, out int totalRecords)
        {
            return _Provider.FindUsersByEmail(email, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Finds the name of the users by.
        /// </summary>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return _Provider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return _Provider.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets the user name by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public string GetUserNameByEmail(string email)
        {
            return _Provider.GetUserNameByEmail(email);
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public string ResetPassword(Guid userGuid)
        {
            return _Provider.ResetPassword(userGuid.ToString(), null);
        }

        /// <summary>
        /// Unlocks the user.
        /// </summary>
        /// <param name="userGuid">The username.</param>
        /// <returns></returns>
        public bool UnlockUser(Guid userGuid)
        {
            return _Provider.UnlockUser(userGuid.ToString());
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateUser(MembershipUser user)
        {
            _Provider.UpdateUser(user);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            return _Provider.GetPassword(null, null);
        }

        /// <summary>
        /// Validates the password strength.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidatePasswordStrength(string password)
        {
            return _Provider.ChangePasswordQuestionAndAnswer(null, password, null, null);
        }

        #endregion
    }
}
