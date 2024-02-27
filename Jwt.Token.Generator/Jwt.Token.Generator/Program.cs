using Jwt.Token.Generator.Data;
using Jwt.Token.Generator.Options;
using Jwt.Token.Generator.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite("DataSource = appdb");
    });
    builder.Services.AddTransient<IJwtTokenProvider, JwtTokenProvider>();

    builder.Services.ConfigureOptions<JwtOptionsSetup>();      
    
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

}


var app = builder.Build();

{
    if (app.Environment.IsDevelopment())
    { 
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

