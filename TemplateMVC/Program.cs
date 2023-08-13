using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TemplateMVC.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Formatando as propriedades JSON para vir no formato de CamelCase 
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

// Habilitando METADATA
builder.Services.AddEndpointsApiExplorer();
// Gerando Swagger para documentação da API
builder.Services.AddSwaggerGen();

// Resolvendo a dependência do Context (Entity Framework Core)
// E passando a ConnectionString
builder.Services.AddDbContext<TemplateMVCContext>
    (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Resolvendo CORS
// Habilita qualquer header
// Habilita qualquer método
// Habilita qualquer origem
app.UseCors(x =>
{
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
});

// Resolvendo (registrando) dependências do Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Habilitando Autenticação
app.UseAuthentication();
// Habilitando Autorização
app.UseAuthorization();

// Habilitando HTTPS
app.UseHttpsRedirection();

// Mapeando Controllers para serem encontrados sem precisar especificá-los
app.MapControllers();

app.Run();