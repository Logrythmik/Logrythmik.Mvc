
using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.Proxies
{
    public interface IFormsAuthenticationProxy
    {
        void SignIn(Guid userGuid, bool createPersistentCookie);
        void SignOut();
        RedirectResult RedirectFromLoginPage(Guid userGuid, bool createPersistentCookie);
    }
}
