using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Logrythmik.Mvc.Proxies
{
    public class FormsAuthenticationProxy : IFormsAuthenticationProxy
    {
        #region Public Methods
        
        /// <summary>
        /// Signs the in.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        public virtual void SignIn(Guid userGuid, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userGuid.ToString(), createPersistentCookie);
        }

        /// <summary>
        /// Signs the out.
        /// </summary>
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public RedirectResult RedirectFromLoginPage(Guid userGuid, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userGuid.ToString(), createPersistentCookie);

            var returnUrl = FormsAuthentication.GetRedirectUrl(userGuid.ToString(), createPersistentCookie);

            return new RedirectResult(returnUrl);
        }
        

        #endregion
    }
}
