using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommerceApi.Data;
using CommerceApi.Models;
using CommerceApi.Services;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await categoryService.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(int id)
    {
        try
        {
            return Ok(categoryService.GetById(id));
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        var result = await categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Category updated)
    {
        try
        {
            return Ok(await categoryService.UpdateAsync(id, updated));
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
            await categoryService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
}