using Microsoft.AspNetCore.Mvc;
using Stockwise.Api.Dtos;
using Stockwise.Api.Services;

namespace Stockwise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController  : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
    {
        IEnumerable<CategoryResponseDto> categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
    {
        CategoryResponseDto dto = await _categoryService.GetByIdAsync(id);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        var dto = await _categoryService.CreateAsync(categoryCreateDto);
        return CreatedAtAction(nameof(GetCategory), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, CategoryUpdateDto categoryUpdateDto)
    {
        await _categoryService.UpdateAsync(id, categoryUpdateDto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}