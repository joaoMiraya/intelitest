using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using api.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/contacts", async (ContactsModel contact, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var phoneNumber = contact.Number;
    var id = contact.Id;
    
    var hash = new HashEntry[] {
    new HashEntry("Id", id),    
    new HashEntry("Number", phoneNumber)
    };

    await db.HashSetAsync("contacts", hash);
    return Results.Ok("Contato adicionado com sucesso!");
})
.WithName("AddContact")
.WithOpenApi();

app.Run();
