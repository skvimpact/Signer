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
            [FromServices] ISignerService signer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var document = documents.Document(cmd.FileName);
                //logger.LogError($"A_{document.Path}-{document.SourceExists}"); // zz
                if (!document.SourceExists)
                {
                    return NotFound(document.Path);
                }
                if (document.SignExists)
                {
                    //logger.LogError($"B_{document.SignPath}-{document.SignExists}"); // zz
                    return Ok(document.SignPath);
                }
                var findResult = signer.FindSignature();
                if (findResult.HasErrors)
                {
                    foreach(var error in findResult.Errors)
                    {
                        logger.LogError(error.ToString());
                    }
                }
                if (signer.SignIsFound())
                {           
                    var signResult = signer.SignDocument(document.Path, document.SignPath);
                    return Ok(document.SignPath);
                }
                else
                {
                    return NotFound("Signature hasn't been found");
                }
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