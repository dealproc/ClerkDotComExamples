using Clerk.BackendAPI;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace Site.Views.Shared.Components;

public class UserButtonViewComponent : ViewComponent
{
    private readonly ClerkBackendApi _clerkBackendApi;

    public UserButtonViewComponent(ClerkBackendApi clerkBackendApi)
    {
        _clerkBackendApi = clerkBackendApi;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? logoutUrl)
    {
        var userId = UserClaimsPrincipal
            .Claims
            .FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value ?? throw new InvalidOperationException("");

        var userTask = _clerkBackendApi.Users.GetAsync(userId);
        var organizationTask = _clerkBackendApi.Organizations.ListAsync(new Clerk.BackendAPI.Models.Operations.ListOrganizationsRequest
        {
            UserId = [userId]
        });

        await Task.WhenAll(userTask, organizationTask);

        var org = organizationTask.Result;

        //org.Organizations.Data[0]

        return View(new Model(
            userTask.Result.User?.FirstName ?? "", 
            userTask.Result.User?.LastName ?? "",
            userTask.Result.User?.ImageUrl ?? "", 
            logoutUrl ?? "",
            organizationTask.Result.Organizations?.Data.Select(d => d.Name) ?? []));
    }

    public record Model(
        string FirstName,
        string LastName,
        string Avatar, 
        string LogoutUrl, 
        IEnumerable<string> Organizations)
    {
        public string Username => $"{FirstName} {LastName}";
    }
}
