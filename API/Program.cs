using CasosDeUso;
using Entidades.Util;
using Infra;
using InterfaceAdapters;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region [DI]
CasosDeUsoBootstrapper.Register(builder.Services);
InfraBootstrapper.Register(builder.Services, builder.Configuration);
InterfaceAdaptersBootstrapper.Register(builder.Services);

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAllOrigins");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "McKing");
    c.RoutePrefix = string.Empty;
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});

app.MapControllers();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is RegraNegocioException ex)
        {
            var errorDetails = new
            {
                Title = "Regra de Neg�cio",
                ex.Message,
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorDetails));
        }
    });
});

app.Run();

