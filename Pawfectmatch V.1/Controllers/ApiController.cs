using Microsoft.AspNetCore.Mvc;
using Pawfectmatch_V._1.Services;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Helpers;

namespace Pawfectmatch_V._1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly IAdoptionService _adoptionService;

        public ApiController(IPetService petService, IAdoptionService adoptionService)
        {
            _petService = petService;
            _adoptionService = adoptionService;
        }

        // GET: api/pets
        [HttpGet("pets")]
        public async Task<IActionResult> GetPets([FromQuery] string searchTerm = "", 
                                                [FromQuery] string petType = "All", 
                                                [FromQuery] string gender = "All", 
                                                [FromQuery] int page = 1)
        {
            try
            {
                var pets = await _petService.GetAvailablePetsAsync(searchTerm, petType, gender, page, 12);
                
                var result = new
                {
                    Pets = pets.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Breed,
                        p.PetType,
                        p.Age,
                        p.Gender,
                        p.Description,
                        p.ImagePath,
                        p.DatePosted
                    }),
                    pets.PageIndex,
                    pets.TotalPages,
                    pets.TotalCount,
                    pets.HasNextPage,
                    pets.HasPreviousPage
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching pets." });
            }
        }

        // GET: api/pets/{id}
        [HttpGet("pets/{id}")]
        public async Task<IActionResult> GetPet(int id)
        {
            try
            {
                var pet = await _petService.GetPetByIdAsync(id);
                if (pet == null)
                    return NotFound(new { error = "Pet not found." });

                var result = new
                {
                    pet.Id,
                    pet.Name,
                    pet.Breed,
                    pet.PetType,
                    pet.Age,
                    pet.Gender,
                    pet.Description,
                    pet.ImagePath,
                    pet.Status,
                    pet.MicrochipNumber,
                    pet.DatePosted,
                    pet.DateOfRelease
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching the pet." });
            }
        }

        // GET: api/pets/search
        [HttpGet("pets/search")]
        public async Task<IActionResult> SearchPets([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                    return Ok(new List<object>());

                var pets = await _petService.GetAvailablePetsAsync(term, "All", "All", 1, 10);
                
                var result = pets.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Breed,
                    p.PetType,
                    p.Age,
                    p.Gender,
                    p.ImagePath
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while searching pets." });
            }
        }

        // GET: api/pets/types
        [HttpGet("pets/types")]
        public async Task<IActionResult> GetPetTypes()
        {
            try
            {
                var types = await _petService.GetPetTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching pet types." });
            }
        }

        // GET: api/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var petStats = await _petService.GetPetStatisticsAsync();
                var appStats = await _adoptionService.GetApplicationStatisticsAsync();

                var result = new
                {
                    Pets = petStats,
                    Applications = appStats
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching statistics." });
            }
        }

        // POST: api/applications
        [HttpPost("applications")]
        public async Task<IActionResult> CreateApplication([FromBody] AdoptionApplication application)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { errors });
                }

                var success = await _adoptionService.CreateApplicationAsync(application);
                if (!success)
                {
                    return BadRequest(new { error = "Failed to create application. Please check your input." });
                }

                return Ok(new { message = "Application submitted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while submitting the application." });
            }
        }

        // GET: api/applications
        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications([FromQuery] string status = "All")
        {
            try
            {
                var applications = await _adoptionService.GetApplicationsAsync(status);
                
                var result = applications.Select(a => new
                {
                    a.Id,
                    a.ApplicantName,
                    a.Email,
                    a.Address,
                    a.Status,
                    a.SubmittedAt,
                    Pet = a.Pet != null ? new
                    {
                        a.Pet.Id,
                        a.Pet.Name,
                        a.Pet.Breed,
                        a.Pet.PetType
                    } : null,
                    Admin = a.Admin != null ? new
                    {
                        a.Admin.UserName,
                        a.Admin.Email
                    } : null
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching applications." });
            }
        }

        // PUT: api/applications/{id}/status
        [HttpPut("applications/{id}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new { error = "Status is required." });
                }

                var success = await _adoptionService.UpdateApplicationStatusAsync(id, request.Status, request.AdminId ?? "");
                if (!success)
                {
                    return NotFound(new { error = "Application not found or update failed." });
                }

                return Ok(new { message = "Application status updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the application status." });
            }
        }

        // GET: api/health
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? AdminId { get; set; }
    }
} 