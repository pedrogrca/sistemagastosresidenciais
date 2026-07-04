using System.Text.Json.Serialization;
using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.Middleware;
using GastosResidenciais.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Nome da política de CORS que autoriza o front-end React a consumir esta API.
const string PoliticaCorsFrontend = "PermitirFrontend";

// ---------------------------------------------------------------------------
// Registro dos serviços (injeção de dependência)
// ---------------------------------------------------------------------------

// Banco de dados: Entity Framework Core usando SQLite.
// A connection string vem do appsettings.json ("DefaultConnection").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Camada de serviços (regras de negócio). Registrada como Scoped porque
// depende do AppDbContext, que também vive por requisição (Scoped).
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// Handler global de exceções + suporte a respostas no formato ProblemDetails.
builder.Services.AddExceptionHandler<TratamentoGlobalDeExcecoes>();
builder.Services.AddProblemDetails();

// Controllers da API. Configuramos o JSON para (de)serializar enums como texto
// ("Despesa"/"Receita") em vez de números — deixa a API mais legível e evita erros.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// CORS: por padrão o navegador bloqueia chamadas de um site (React) para
// outra origem (a API). Esta política libera o endereço do servidor de
// desenvolvimento do Vite.
builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCorsFrontend, policy =>
        policy.WithOrigins(
                  "http://localhost:5173", // porta padrão do Vite (React)
                  "http://localhost:5174") // porta alternativa usada se a 5173 estiver ocupada
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Swagger/OpenAPI: documentação interativa da API (útil para testes manuais).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------------------------------------------------------------------------
// Criação/atualização automática do banco de dados
// ---------------------------------------------------------------------------
// Ao iniciar, aplicamos as migrações pendentes. Assim o banco (arquivo .db)
// é criado automaticamente na primeira execução, sem nenhum comando manual.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ---------------------------------------------------------------------------
// Pipeline de requisições HTTP
// ---------------------------------------------------------------------------
// O handler global de exceções deve ser um dos primeiros middlewares,
// para capturar erros de qualquer etapa seguinte do pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(PoliticaCorsFrontend);

app.UseAuthorization();

app.MapControllers();

app.Run();
