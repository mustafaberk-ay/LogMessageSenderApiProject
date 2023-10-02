using LogMessageSenderApiProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LogMessageSenderApiProject.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class ActionsController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpPost]
        public async Task<IActionResult> RespondOk()
        {
            var logMessage = CreateLogMessage("OK");
            await SendLogMessage(logMessage);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RandomQuote()
        {
            var quote = FetchRandomQuote().Result;
            var logMessage = CreateLogMessage(quote);
            await SendLogMessage(logMessage);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Divide(DivisionRequest request)
        {
            if (request.Divisor == 0)
            {
                var errorMessage = CreateLogMessage("Division by zero is not allowed");
                await SendLogMessage(errorMessage);
                return BadRequest(errorMessage.Message);
            }

            var result = request.Dividend / request.Divisor;
            var resultMessage = CreateLogMessage($"Division Result: {result}");
            await SendLogMessage(resultMessage);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Initialize()
        {
            var logMessage = CreateLogMessage("App Started");
            await SendLogMessage(logMessage);
            return Ok();
        }

        [HttpPost]
        public IActionResult GetLogs([FromBody] List<LogMessage> logs)
        {
            Console.WriteLine("Getting All Logs");
            foreach (LogMessage logMessage in logs)
            {
                Console.WriteLine($"Log Message: {logMessage.Message} ----- Timestamp: {logMessage.Timestamp}");
            }
            return Ok();
        }

        private async Task<string> FetchRandomQuote()
        {
            try
            {
                string randomQuoteUrl = "https://api.quotable.io/quotes/random";
                string errorQuoteUrl = "https://api.quotable.io/quotes/random?minLength=200";
                var randomQuoteResponse = await client.GetAsync(randomQuoteUrl);
                randomQuoteResponse.EnsureSuccessStatusCode();
                var content = await randomQuoteResponse.Content.ReadAsStringAsync();
                var quoteList = JsonConvert.DeserializeObject<List<Quote>>(content);
                return quoteList.FirstOrDefault().Content;
            }
            catch (Exception e)
            {
                var errorMessage = CreateLogMessage(e.Message);
                await SendLogMessage(errorMessage);
                throw;
            }

        }

        private LogMessage CreateLogMessage(string logMessage)
        {
            return new LogMessage
            {
                Message = logMessage,
                Timestamp = DateTime.Now
            };
        }

        private async Task<IActionResult> SendLogMessage(LogMessage payload)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(payload);
                var response = await client.PostAsync("http://localhost:5007/receive", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                return Ok();
            }
            catch (Exception e)
            {
                var errorMessage = CreateLogMessage(e.Message);
                await SendLogMessage(errorMessage);
                return BadRequest(e.Message);
            }
        }
    }
}
