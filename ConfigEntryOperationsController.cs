using Microsoft.AspNetCore.Mvc;
using ConfigFileManagementApp.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ConfigFileManagementApp.Controllers
{
    [Route("api/ConfigEntry")]
    [ApiController]
    public class ConfigEntryOperationsController : ControllerBase
    {
        private readonly ConfigEntryService _configEntryService;

        public ConfigEntryOperationsController(ConfigEntryService configEntryService)
        {
            _configEntryService = configEntryService;
        }

        // GET: api/ConfigEntry/List
        [HttpGet("List")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Get([Required] string fileName, string? sectionName = null)
        {
            try
            {
                if (!fileName.EndsWith(".json"))
                {
                    fileName = fileName + ".json";
                }
                
                dynamic configEntries = _configEntryService.GetConfigEntries(fileName, sectionName);

                return Ok(configEntries);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }       
     
        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Post([Required] string fileName, [Required] string sectionName, Dictionary<string, object> configItems)
        {
            try
            {              
                if (!fileName.EndsWith(".json"))
                {
                    fileName = fileName + ".json";
                }

                _configEntryService.AddConfigEntry(fileName, sectionName, configItems);
                 
                return Created(sectionName, new {sectionName = configItems});
            }
            catch(FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/ConfigEntry/Update
        [HttpPut("Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Put([Required] string fileName, [Required] string sectionName, Dictionary<string, object> configItems)
        {
            try
            {
                if (!fileName.EndsWith(".json"))
                {
                    fileName = fileName + ".json";
                }

                _configEntryService.UpdateConfigEntry(fileName, sectionName, configItems);
                return Ok("Config entry updated successfully");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE api/ConfigEntry/Delete
        [HttpDelete("Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Delete([Required] string fileName, [Required] string sectionName, [Required]string configEntryKey)
        {
            try
            {
                if (!fileName.EndsWith(".json"))
                {
                    fileName = fileName + ".json";
                }

                _configEntryService.DeleteConfigEntry(fileName, sectionName, configEntryKey);
                return Ok("Config entry deleted successfully");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
