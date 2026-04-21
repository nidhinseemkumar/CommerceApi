using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommerceApi.Data;
using CommerceApi.Models;
using CommerceApi.Services;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await productService.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(int id)
    {
        try
        {
            return Ok(productService.GetById(id));
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var result = await productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Product updated)
    {
        try
        {
            return Ok(await productService.UpdateAsync(id, updated));
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await productService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
}