using Microsoft.AspNetCore.Mvc;

using TheLibrary.Common.DTOs;
using TheLibrary_BusinessLayer.Services;

namespace TheLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryBranchController : ControllerBase
    {
        private readonly ILibraryBranchService _branchService;

        public LibraryBranchController(ILibraryBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? libraryId = null)
        {
            return Ok(await _branchService.GetAllAsync(libraryId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _branchService.GetByIdAsync(id));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LibraryBranchCreateDTO dto)
        {
            try
            {
                var id = await _branchService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] LibraryBranchCreateDTO dto)
        {
            try
            {
                await _branchService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _branchService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
