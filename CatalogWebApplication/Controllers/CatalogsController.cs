using CatalogWebApplication.Controllers.Model;
using CatalogWebApplication.Service;
using Microsoft.AspNetCore.Mvc;

namespace CatalogWebApplication.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CatalogsController : ControllerBase
    {
        private readonly CatalogService _service;

        public CatalogsController(CatalogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Guid.TryParse(id, out _))
                return BadRequest("Invalid ID format. Must be a GUID.");

            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound($"Category with ID '{id}' not found.");

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto request)
        {
            if (!Guid.TryParse(request.ParentId, out _))
                return BadRequest("Invalid ID format. Must be a GUID.");

            var createdCategory = await _service.CreateAsync(request.Name, request.ParentId);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            if (!Guid.TryParse(id, out _))
                return BadRequest("Invalid ID format. Must be a GUID.");

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }

}
