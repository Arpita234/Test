using Microsoft.AspNetCore.Mvc;
using ConfigFileManagementApp.Interface;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ConfigFileManagementApp.Controllers
{
    [Route("api/ConfigFiles")]
    [ApiController]
    public class ConfigFileOperationsController : ControllerBase
    {

        private readonly IFileUploadService _fileUploadService;
        private readonly string configDirectory = "ConfigFiles";

        public ConfigFileOperationsController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        // GET: api/ConfigFiles/List
        [HttpGet("List")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Get()
        {
            try
            {
                var filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), configDirectory);
                var configFiles = Directory.GetFiles(filesDirectory).Select(file => Path.GetFileName(file)).ToList();
                return Ok(configFiles);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/ConfigFiles/UploadFile
        [HttpPost("UploadFile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> Post(IFormFile file)
        {
            try
            {
                if (file is null || file.Length == 0)
                {
                    return BadRequest("Not a valid file");
                }

                var fileName = await _fileUploadService.UploadFileAsync(file);

                return Created(fileName, new {FileName = fileName});
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,$"{ex.Message} {ex.InnerException}");
            }
        }


        // DELETE api/ConfigFiles/DeleteFile
        [HttpDelete("DeleteFile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult Delete([Required] string fileName)
        {
            try
            {
                if (!fileName.EndsWith(".json"))
                {
                    fileName = fileName + ".json";
                }

                var filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), configDirectory);

                var configFilesNames = Directory.GetFiles(filesDirectory).
                                  Where(file => (Path.GetFileName(file)).Substring(Path.GetFileName(file).IndexOf("_") + 1) == fileName).
                                  Select(file => Path.GetFileName(file)).ToList();

                bool fileDelted = false;
                foreach (var file in configFilesNames)
                {
                    var filePath = Path.Combine(filesDirectory, file);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        fileDelted = true;
                    }
                }

                if (fileDelted)
                {
                    return Ok("Config file deleted successfully");
                }              
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message} {ex.InnerException}");

            }
        }

    }
}
