
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicalRecords.Domain.Contracts;
using MedicalRecords.Domain.Models;
using MedicalRecords.Middlewares.Custom_Exceptions;
using Microsoft.AspNetCore.Authorization;


namespace MedicalRecords.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController: ControllerBase
{
    private readonly IPatientService _service;
    private readonly ILogger<PatientsController> _logger;
    public PatientsController(IPatientService service,ILogger<PatientsController> logger)
    
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var patients = await _service.GetAllAsync();
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching patients.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }

        
    }
   
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        
        var (data, total) = await _service.GetPagedAsync(page, size);
        return Ok(new { total, data });
    }
   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching patient.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
       
    }

   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _service.CreateAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient. .");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

 
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Patient patient)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if ( await _service.UpdateAsync(id, patient))
            {
                return NoContent();
            }
            return NotFound(new { message = $"Patient with ID {id} not found." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating driver with ID {id}.");
            return StatusCode(500, new { message = "An error occurred while updating the patient." });
        }
        
    }
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        
        try
        {
            if( await _service.DeleteAsync(id))
                return NoContent();
            return NotFound(new { message = $"Driver with ID {id} not found." });  
        }
        
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting patient with ID {id}.");
            return StatusCode(500, new { message = "An error occurred while deleting the patient." });
        }
    }
}

