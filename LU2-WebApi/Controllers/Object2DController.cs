using LU2_WebApi.Models;
using LU2_WebApi.Repositorys;
using Microsoft.AspNetCore.Mvc;

namespace LU2_WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _repository;

        public Object2DController(IObject2DRepository repository)
        {
            _repository = repository;
        }

        // GET: /Object2D
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object2D>>> GetAll()
        {
            var objects = await _repository.GetAllAsync();
            return Ok(objects);
        }

        // GET: /Object2D/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Object2D>> GetById(int id)
        {
            var object2D = await _repository.GetByIdAsync(id);
            if (object2D == null)
                return NotFound("Object2D not found.");

            return Ok(object2D);
        }

        // POST: /Object2D
        [HttpPost]
        public async Task<ActionResult<Object2D>> Create([FromBody] Object2D object2D)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdObject = await _repository.AddAsync(object2D);
            return CreatedAtAction(nameof(GetById), new { id = createdObject.Id }, createdObject);
        }

        // PUT: /Object2D/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Object2D updatedObject2D)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _repository.UpdateAsync(id, updatedObject2D);
            if (!success)
                return NotFound("Object2D not found.");

            return NoContent();
        }

        // DELETE: /Object2D/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
                return NotFound("Object2D not found.");

            return NoContent();
        }
    }
}
