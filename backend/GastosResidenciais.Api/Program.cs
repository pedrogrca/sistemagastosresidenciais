using System.Text;
using System.Text.Json.Serialization;
using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.Middleware;
using GastosResidenciais.Api.Models;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
builder.Services.AddScoped<ITotaisService, TotaisService>();

// Serviços de autenticação.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// PasswordHasher é sem estado, então pode ser Singleton. Faz o hash e a
// verificação segura das senhas.
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

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

// ---------------------------------------------------------------------------
// Autenticação e autorização (JWT)
// ---------------------------------------------------------------------------
// Configura a validação do token JWT em cada requisição: assinatura, emissor,
// audiência e validade. Os valores vêm da seção "Jwt" do appsettings.json.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emissor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Chave"]!))
        };
    });

builder.Services.AddAuthorization();

// Swagger/OpenAPI: documentação interativa da API (útil para testes manuais).
// Também habilitamos o botão "Authorize" para enviar o token JWT nos testes.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var esquemaJwt = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole o token JWT recebido no login (sem o prefixo 'Bearer')."
    };
    options.AddSecurityDefinition("Bearer", esquemaJwt);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

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

// A autenticação (valida o token) deve vir antes da autorização (verifica o acesso).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
