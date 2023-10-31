using Microsoft.AspNetCore.Mvc;
using Signer.Services;

namespace Signer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignerController : ControllerBase
    {
        readonly ILogger<SignerController> logger;
        public SignerController(ILogger<SignerController> logger)
        {
            this.logger = logger;
        }
        
        [HttpPost("Sign")]
        public IActionResult SignFile(
            [FromBody] SignFileCmd cmd,
            [FromServices] UnsignedDocuments documents,
            [FromServices] ISignerService signerService)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var document = documents.Document(cmd.FileName);
                if (!document.SourceExists)
                {
                    return NotFound(document.Path);
                }
                if (document.SignExists)
                {
                    return Ok(document.SignPath);
                }
                var signingResult = signerService.SignDocument(new UnsignedDocument[] { document });
                if (!signingResult.HasErrors)
                    return Ok(document.SignPath);
                else
                    throw new Exception(signingResult.GetAllErrors());
            }
            catch(Exception ex)
            {
                return GetErrorResponse(ex);
            }            
        }

        [HttpGet("Crc32/{fileName}")]
        public IActionResult Crc32(
            string fileName,
            [FromServices] UnsignedDocuments documents)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var document = documents.Document(fileName); 
                if (!document.SourceExists)
                {
                    return NotFound("File for Crc32 calculation hasn't been found");
                }
                return Ok(document.Crc32());
            } 
            catch(Exception ex)
            {
                return GetErrorResponse(ex);
            }
        }                   
        private static IActionResult GetErrorResponse(Exception ex)
        {
            var error = new ProblemDetails
            {
                Title = "An error occured",
                Detail = ex.Message,
                Status = 500,
                Type = "https://httpstatuses.com/500"
            };
            return new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}