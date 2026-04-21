using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommerceApi.Data;
using CommerceApi.Models;
using CommerceApi.Services;

using CommerceApi.DTOs;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(
    IProductService productService, 
    IFileService fileService,
    ICategoryService categoryService) : ControllerBase
{
    [HttpGet("export/{format}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Export(string format)
    {
        var products = await productService.GetAllAsync();
        var data = products.Select(p => new ProductExportDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CategoryName = p.Category?.Name
        });

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = fileService.ExportToCsv(data);
            return File(bytes, "text/csv", "products.csv");
        }
        else if (format.Equals("excel", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = fileService.ExportToExcel(data, "Products");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }

        return BadRequest("Invalid format. Use 'csv' or 'excel'.");
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Please upload a file.");

        var extension = Path.GetExtension(file.FileName).ToLower();
        List<ProductExportDto> data;

        using (var stream = file.OpenReadStream())
        {
            if (extension == ".csv") data = fileService.ImportFromCsv<ProductExportDto>(stream);
            else if (extension == ".xlsx") data = fileService.ImportFromExcel<ProductExportDto>(stream);
            else return BadRequest("Unsupported file format.");
        }

        var categories = await categoryService.GetAllAsync();
        int updated = 0;
        int added = 0;

        foreach (var item in data)
        {
            var category = categories.FirstOrDefault(c => c.Name.Equals(item.CategoryName, StringComparison.OrdinalIgnoreCase));
            
            var product = new Product
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Stock = item.Stock,
                CategoryId = category?.Id
            };

            if (item.Id > 0)
            {
                try {
                    await productService.UpdateAsync(item.Id, product);
                    updated++;
                } catch {
                    // If ID provided but not found, maybe create it or skip?
                    // Implementation says "update if exists".
                    await productService.CreateAsync(product);
                    added++;
                }
            }
            else
            {
                await productService.CreateAsync(product);
                added++;
            }
        }

        return Ok(new { Message = $"Import completed. Added: {added}, Updated: {updated}" });
    }
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