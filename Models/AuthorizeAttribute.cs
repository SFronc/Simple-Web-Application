using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Models;

public class AuthorizeAttribute : TypeFilterAttribute{
    public AuthorizeAttribute() : base(typeof(AuthorizeFilter)){}

    private class AuthorizeFilter : IAuthorizationFilter{
        public void OnAuthorization(AuthorizationFilterContext context){
            if(context.HttpContext.Session.GetString("IsLoggedIn") != "true"){
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }

}