using Microsoft.OpenApi.Models;
//using PizzaStore.DB;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db"; // string de conexão do banco de dados estabelecida em 

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
builder.Services.AddSqlite<PizzaDb>(connectionString); // Registra o uso de SQLite como o provedor de Banco de dados com o qual a aplicação se conecta.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Estabelece o Uso de Swagger no projeto. Rodando a aplicação e acessando a URL com caminho /swagger permite acessar.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

app.MapGet("/", () => "Hello World!"); // Teste simples

app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id)); // Retorna o registro com o ID especificado

app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync()); // Retorna a lista completa de registros

app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) => // db = conexão do banco de dados ; pizza = corpo da requisição seguindo modelo Pizza
{

    await db.Pizzas.AddAsync(pizza); // adiciona o novo registro
    await db.SaveChangesAsync(); // salva as mudanças no banco de dados. 
    return Results.Created($"/pizza/{pizza.Id}", pizza); // retorno da requisição

}); // Lança uma nova instância de objeto "pizza" como um registro no banco de dados.

app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) => // db = conexão do banco de dados ; updatepizza = corpo da requisição seguindo modelo Pizza ; id
{

    var pizza = await db.Pizzas.FindAsync(id); // estabelece o valor da variável pizza como o registro de ID correspondente.
    if (pizza is null) return Results.NotFound(); // caso não encontre um registro com a ID correspondente, retorna um status code 404 (não encontrado)
    pizza.Name = updatepizza.Name; // substitui o valor contido no campo de nome pelo novo valor fornecido no corpo da requisição
    pizza.Description = updatepizza.Description; // substitui o valor contido no campo de descrição pelo novo valor
    await db.SaveChangesAsync(); // salva as mudanças
    return Results.NoContent(); // Retorna um status code 204 indicando que a requisição foi um sucesso e não há novas informações a serem retornadas

}); // atualiza os valores de um registro identificado a partir de uma ID.

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) => // db = conexão do banco de dados ; id
{

    var pizza = await db.Pizzas.FindAsync(id); // armazena os valores contidos no registro do banco de dados de ID correspondente na variavel pizza
    if (pizza is null) return Results.NotFound(); // caso não haja um registro correspondente, é retornado um status code 404 (Não encontrado)
    db.Pizzas.Remove(pizza); // Realiza a remoção do registro do banco de dados
    await db.SaveChangesAsync(); // Salva as mudanças no banco de dados
    return Results.Ok(); // retorna um status code 200 (OK)

}); // Deleta um registro no banco de dados através de um ID

app.Run(); // executa a aplicação (Esperando requisições em localhost:5278)
