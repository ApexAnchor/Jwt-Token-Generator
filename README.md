**How to Add JWT Token Authentication for Web APIs**

**Introduction**

JSON Web Tokens (JWTs) are a popular way to secure web APIs. They are compact, self-contained tokens that can be used to securely transmit information between two parties. In this guide, we will show you how to add JWT token authentication to your ASP.NET Core Web API.

**Prerequisites**

Before you begin, you will need the following:

* A .NET Core web API project
* A JWT library (e.g., Microsoft.IdentityModel.Tokens)
* A secret key for signing and verifying JWTs

**Step 1: Install the JWT Library**

Open your .NET Core project in Visual Studio or your preferred IDE. Right-click on the project and select **Manage NuGet Packages**. Search for the **Microsoft.AspNetCore.Authentications.JwtBearer** package and install it.

**Step 2: Create a Secret Key**

You will need a secret key to sign and verify JWTs. 

**Step 3: Configure JWT Authentication**

Open the `Startup.cs` file in your project. In the `ConfigureServices` method, add the following code:

```csharp
 builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(o =>
                 {
                     o.RequireHttpsMetadata = false;
                     var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]);
                     var audience = builder.Configuration["Jwt:Audience"].ToString();
                     var issuer = builder.Configuration["Jwt:Issuer"].ToString();
                     o.SaveToken = true;
                     o.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidAudience = audience,
                         ValidIssuer = issuer,
                         IssuerSigningKey = new SymmetricSecurityKey(key)
                     };
                 });   
```

**Step 4: Generate a Token**

In your API controller, add a method to create a JWT token. For example:

```csharp
[HttpPost("reg")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (ModelState.IsValid)
    {
        User user = new() { Email = request.Email, Password = HashPassword(request.Password) };
        await applicationDbContext.Users.AddAsync(user);
        await applicationDbContext.SaveChangesAsync();
        return Ok("Registration successfull");
    }
    return BadRequest("Invalid data supplied");
}

[HttpPost("login")]
public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
{
    if (ModelState.IsValid)
    {
        var user = await applicationDbContext.Users.FindAsync(request.Email);
        if (user is null)        {
            return BadRequest("User not registered");
        }
        else if (user.Password != HashPassword(request.Password))        {
            return BadRequest("Incorrect Password");
        }
        string token = await tokenProvider.GenerateToken(user);
        return Ok(token);
    }
    return BadRequest("Invalid data supplied");
}
```

**Step 5: Protect API Endpoints**

To protect your API endpoints, add the `[Authorize]` attribute to the controller or action methods that you want to protect. For example:

```csharp
 [ApiController]
 [Route("[controller]")]
 [Authorize(Roles = "User")]
 public class WeatherForecastController : ControllerBase
 {
    // ...
 }
```

**Step 6: Test the Authentication**

Run your API project and send a request to the `/login` endpoint to obtain a JWT token. Then, send a request to a protected API endpoint, with the JWT token in the Authorization header. You should receive a successful response if the JWT token is valid.

**Conclusion**

In this guide, we showed you how to add JWT token authentication to your ASP.NET Core Web API. By following these steps, you can secure your API and protect it from unauthorized access.
