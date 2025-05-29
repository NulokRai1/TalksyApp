using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using TalksyApp.Helper;
using TalksyApp.Services;
using MyApp.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalksyApp API", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,    // Change from ApiKey to Http
		Scheme = "bearer",                 // Lowercase bearer
		BearerFormat = "JWT"
	});


	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] { }
		}
	});
});



// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = builder.Configuration["AppSettings:Issuer"],
			ValidateAudience = true,
			ValidAudience = builder.Configuration["AppSettings:Audience"],
			ValidateLifetime = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)
			),
			ValidateIssuerSigningKey = true
		};
	});

builder.Services.AddAuthorization();
//builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<AppDbContext>();
// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatService>();

// HTTP Context and Identity helper
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GetIdentity>();

// SQL Connection factory
builder.Services.AddSingleton(serviceProvider =>
{
	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var connectionString = configuration.GetConnectionString("DefaultConnection")
		?? throw new ApplicationException("The connection string is null");
	return new SqlConnectionFactory(connectionString);
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("http://localhost:5173") // Frontend URL
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.MapIdentityApi<User>();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication(); // Must come before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
