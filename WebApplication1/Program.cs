var builder = WebApplication.CreateBuilder(args);

// Adicione servi�os ao cont�iner
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Registrar IHttpClientFactory
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(xmlPath);
});

// Configura��o expl�cita da porta exigida pelo Render
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Porta padr�o do Render1
});

var app = builder.Build();

// Configurar middleware de pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GitHub Repo Lister API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection(); // Redirecionamento para HTTPS no ambiente de produ��o
}

app.UseAuthorization();
app.MapControllers();

app.Run();
