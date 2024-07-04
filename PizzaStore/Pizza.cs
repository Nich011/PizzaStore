using Microsoft.EntityFrameworkCore; // Permite se comunicar com um banco de dados como SQLite

namespace PizzaStore.Models // Conjunto de classes relacionadas ao banco de dados
{
    public class Pizza // Classe que estabelece os tipos de valor contidos dentro de um item no banco de dados
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    class PizzaDb : DbContext // Classe de contexto. Representa a sessão estabelecida com o banco de dados. // Revisar!!!!!!!!!
    {
        public PizzaDb(DbContextOptions options) : base(options) { }
        public DbSet<Pizza> Pizzas { get; set; } = null!; // Realiza a query para lançar novas instâncias de Pizza
    }
}