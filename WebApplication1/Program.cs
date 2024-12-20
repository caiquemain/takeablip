using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Registrar IHttpClientFactory
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Ajuste de idioma Swagger
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

// Configuração do XML para documentação
var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    options.SupportNonNullableReferenceTypes();
    options.DescribeAllParametersInCamelCase();
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    options.SupportNonNullableReferenceTypes();
});

// Configuração explícita da porta exigida pelo Render
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Porta padrão do Render
});

var app = builder.Build();

// Redirecionar a raiz para o GitHub
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "https://github.com/caiquemain/takeablip", 301));

// Configurar middleware de pipeline HTTP
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GitHub Repo Lister API v1");
        options.RoutePrefix = "documentation"; 
    });
}

// Adicionar HTTPS apenas se não for produção
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
