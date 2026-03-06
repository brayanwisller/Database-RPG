using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RpgApi.Data;
using RpgApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Environment.EnvironmentName = "Development";

builder.WebHost.UseUrls("http://localhost:5099");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=rpg.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


var webTask = app.RunAsync();
Console.WriteLine("API online em http://localhost:5099 (Swagger em /swagger)");
Console.WriteLine("=============================================");
Console.WriteLine("         Gerenciador de Itens de RPG         ");
Console.WriteLine("=============================================");
Console.WriteLine("Console e API executando juntos!");


while (true)
{
    Console.WriteLine();
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Cadastrar novo item");
    Console.WriteLine("2 - Listar todos os itens");
    Console.WriteLine("3 - Atualizar item (por Id)");
    Console.WriteLine("4 - Remover item (por Id)");
    Console.WriteLine("0 - Sair");
    Console.Write("\nOpção Selecionada: ");

    var options = Console.ReadLine();

    if (options == "0") break;

    switch (options)
    {
        case "1": await CreateItemAsync(); break;
        case "2": await ListItemsAsync(); break;
        case "3": await UpdateItemAsync(); break;
        case "4": await DeleteItemAsync(); break;
        default: Console.WriteLine("Opção inválida."); break;
    }
}


await app.StopAsync();
await webTask;


async Task CreateItemAsync()
{
    Console.Write("\nNome do item: ");
    var name = (Console.ReadLine() ?? "").Trim();

    Console.Write("Raridade (Comum, Incomum, Raro, Epico, Lendario): ");
    var rarity = (Console.ReadLine() ?? "").Trim();

    Console.Write("Preço: ");
    if (!decimal.TryParse(Console.ReadLine(), out var preco))
    {
        Console.WriteLine("Preço inválido.");
        return;
    }

    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(rarity))
    {
        Console.WriteLine("Nome e Raridade são obrigatórios.");
        return;
    }

    using var db = new AppDbContext();
    if (await db.Items.AnyAsync(i => i.Name.ToUpper() == name.ToUpper()))
    {
        Console.WriteLine("Já existe um item com esse nome.");
        return;
    }

    var item = new Item { Name = name, Rarity = rarity, Preco = preco};
    db.Items.Add(item);
    await db.SaveChangesAsync();
    Console.WriteLine($"Item '{item.Name}' cadastrado com sucesso! Id: {item.Id}");
}

async Task ListItemsAsync()
{
    using var db = new AppDbContext();
    var items = await db.Items.OrderBy(i => i.Id).ToListAsync();

    if (items.Count == 0){Console.WriteLine("Nenhum item encontrado."); return;}

    Console.WriteLine("\n----------------------------------------------------------------------------------");
    Console.WriteLine("Id | Nome                 | Raridade             | Preço      | Data Criação (UTC)");
    Console.WriteLine("----------------------------------------------------------------------------------");
    foreach (var i in items){
        Console.WriteLine($"{i.Id,2} | {i.Name,-20} | {i.Rarity,-20} | {i.Preco,10:C2} | {i.CriadoEm:yyyy-MM-dd HH:mm:ss}");
    }
    Console.WriteLine("----------------------------------------------------------------------------------");
}

async Task UpdateItemAsync()
{
    Console.Write("\nInforme o Id do item a atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var item = await db.Items.FindAsync(id);
    if (item is null) { Console.WriteLine("Item não encontrado."); return; }

    Console.WriteLine($"Atualizando Id {item.Id}. Deixe em branco para manter o valor atual.");
    
    Console.Write($"Nome atual [{item.Name}]: ");
    var newName = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newName)) item.Name = newName;
    
    Console.Write($"Raridade atual [{item.Rarity}]: ");
    var newRarity = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newRarity)) item.Rarity = newRarity;

    Console.Write($"Preço atual [{item.Preco:C2}]: ");
    var newPrecoStr = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newPrecoStr) && decimal.TryParse(newPrecoStr, out var newPreco))
    {
        item.Preco = newPreco;
    }
    
    if (await db.Items.AnyAsync(i => i.Name.ToUpper() == item.Name.ToUpper() && i.Id != id))
    {
        Console.WriteLine("Já existe outro item com o novo nome informado.");
        return;
    }
    
    await db.SaveChangesAsync();
    Console.WriteLine("Item atualizado com sucesso.");
}

async Task DeleteItemAsync()
{
    Console.Write("\nInforme o Id do item a remover: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var item = await db.Items.FindAsync(id);
    if (item is null) { Console.WriteLine("Item não encontrado."); return; }

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    Console.WriteLine($"Item '{item.Name}' removido com sucesso.");
}