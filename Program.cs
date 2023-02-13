using ES.API.Entities;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

///ElasticSearch
var uri = builder.Configuration["ElasticSearchConfiguation:Uri"];
var defaultIndex = builder.Configuration["ElasticSearchConfiguation:Index"];

var settings = new ConnectionSettings(new Uri(uri)).PrettyJson().DefaultIndex(defaultIndex);

var client = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(client);

await client.Indices.CreateAsync(defaultIndex, i => i.Map<ProductEntity>(x => x.AutoMap()));

////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
