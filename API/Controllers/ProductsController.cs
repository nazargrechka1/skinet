using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        return Ok(await repository.GetProductsAsync());
    }

    // single item get 
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repository.AddProduct(product);

        return await repository.SaveChangesAsync()
            ? CreatedAtAction("GetProduct", new { id = product.Id }, product)
            : BadRequest("Problem Creating Product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id) return BadRequest("id mismatch");
        if (!repository.ProductExists(id)) return NotFound();
        
        repository.UpdateProduct(product);
        return await repository.SaveChangesAsync()
            ? NoContent()
            : BadRequest("problem updating product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        
        repository.DeleteProduct(product);
        
        return await repository.SaveChangesAsync()
            ? NoContent()
            : BadRequest("problem deleting product");
    }
}