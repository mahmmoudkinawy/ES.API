using ES.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.ComponentModel.DataAnnotations;

namespace ES.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IElasticClient _elasticClient;

    public ProductsController(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient ??
            throw new ArgumentNullException(nameof(elasticClient));
    }

    [HttpGet(Name = "GetProduct")]
    public async Task<IActionResult> GetProduct([FromQuery] string keyword)
    {
        var result = await _elasticClient.SearchAsync<ProductEntity>(s =>
        {
            s.Query
                (
                    q => q.QueryString
                        (
                            d => d.Query($"*{keyword}*")
                        )
                ).Size(1000);

            return s;
        });

        return Ok(result.Documents.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] ProductForCreationDto productForCreationDto)
    {
        var product = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Description = productForCreationDto.Description,
            Name = productForCreationDto.Name,
            Price = productForCreationDto.Price,
            Quantity = productForCreationDto.Quantity
        };

        var result = await _elasticClient.IndexDocumentAsync(product);

        if (result.IsValid)
        {
            return Ok(product);
        }

        return BadRequest("Something went wrong");
    }

}

public class ProductForCreationDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
