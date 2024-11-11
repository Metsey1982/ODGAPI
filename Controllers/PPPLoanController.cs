using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ODGAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PPPLoanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PPPLoanController> _logger; 
        private readonly HttpClient _httpClient;
        public PPPLoanController(IConfiguration configuration, ILogger<PPPLoanController> logger, HttpClient httpClient) 
        { 
            _configuration = configuration; 
            _logger = logger;
            _httpClient = httpClient;
        }
        [HttpGet("test")]
        public IActionResult GetTestMessage()
        {
            return Ok("Hello from MyNewController!");
        }

        // Add more actions (methods) as needed

		[HttpGet(Name = "GetPPPLoan")]
		public async Task<IActionResult> GetPPPLoanDataAll()
		{
            try 
            {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoanURL"];
                var pppresourceId = _configuration["PPPLoanResourceId"];

                var url = $"{ppploanurl}{pppresourceId}";
                _logger.LogInformation("url is " + url);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsondata = await response.Content.ReadAsStringAsync();
                return Ok(jsondata);
           }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred" + ex.Message.ToString());
           }   
 
		}

        [HttpGet("paginated/{page}/{pageSize}")]
        public async Task<IActionResult> GetPPPLoanPaginatedData(int page, int pageSize)
        { 
            try 
            {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoanURL"];
                var pppresourceId = _configuration["PPPLoanResourceId"];
                var url = $"{ppploanurl}{pppresourceId}?$limit={pageSize}&$offset={page * pageSize}&$order=cd,JobsRetained DESC";
                _logger.LogInformation("url is " + url);
                //var url = $"https://data.nj.gov/resource/riep-z5cp.json?$limit={pageSize}&$offset={page * pageSize}&$order=naicscode";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); 
                var jsondata = await response.Content.ReadAsStringAsync();
            
                return Ok(jsondata);
            }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred");
           }   
        }
    }
}
