using Clerk.BackendAPI;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

builder.Services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
    .Configure(options =>
    {
        options.Authority = "*******************************";
        options.ClientId = "***********************";
        options.ClientSecret = "*****************************";
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, (connect) =>
    {
        connect.ResponseType = "code";
        connect.UsePkce = true;

        connect.Scope.Clear();
        connect.Scope.Add("openid");
        connect.Scope.Add("profile");
        //o.Scope.Add("roles");
        connect.Scope.Add("email");

        connect.GetClaimsFromUserInfoEndpoint = true;
        connect.SaveTokens = true;

        connect.TokenValidationParameters = new TokenValidationParameters()
        {
            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role,
            ValidateIssuer = true
        };

        connect.MapInboundClaims = false;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (cookie) =>
    {
        //cookie.Cookie.SameSite = SameSiteMode.Lax;
        //cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        cookie.AccessDeniedPath = "/error";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authorized", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
    });
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton((services) => new ClerkBackendApi("********************************"));

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
