using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.Api.Data;
using SkillForge.Api.DTOs.Exchange;
using SkillForge.Api.Models;
using SkillForge.Api.Services;
using System.Security.Claims;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExchangesController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExchangesController> _logger;

        public ExchangesController(IExchangeService exchangeService, INotificationService notificationService, ApplicationDbContext context, ILogger<ExchangesController> logger)
        {
            _exchangeService = exchangeService;
            _notificationService = notificationService;
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ExchangeDto>> CreateExchange([FromBody] CreateExchangeDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.CreateExchangeAsync(userId.Value, dto);
                if (exchange == null)
                {
                    return BadRequest(new { message = "Failed to create exchange. Please check that the offerer exists and offers this skill." });
                }

                // Send real-time notification to the offerer
                var fullExchange = await GetFullExchangeModelAsync(exchange.Id);
                if (fullExchange != null)
                {
                    await _notificationService.SendExchangeRequestNotificationAsync(fullExchange);
                }

                _logger.LogInformation($"User {userId} created exchange request with user {dto.OffererId} for skill {dto.SkillId}");
                return CreatedAtAction(nameof(GetExchange), new { id = exchange.Id }, exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exchange");
                return StatusCode(500, new { message = "An error occurred while creating the exchange." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExchangeDto>> GetExchange(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.GetExchangeByIdAsync(id, userId.Value);
                if (exchange == null)
                {
                    return NotFound();
                }

                return Ok(exchange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting exchange {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the exchange." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeDto>>> GetMyExchanges([FromQuery] ExchangeStatus? status = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchanges = await _exchangeService.GetUserExchangesAsync(userId.Value, status);
                return Ok(exchanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user exchanges");
                return StatusCode(500, new { message = "An error occurred while retrieving exchanges." });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ExchangeDto>> UpdateExchange(int id, [FromBody] UpdateExchangeDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.UpdateExchangeAsync(id, userId.Value, dto);
                if (exchange == null)
                {
                    return NotFound();
                }

                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating exchange {id}");
                return StatusCode(500, new { message = "An error occurred while updating the exchange." });
            }
        }

        [HttpPost("{id}/accept")]
        public async Task<ActionResult<ExchangeDto>> AcceptExchange(int id, [FromBody] ExchangeActionDto? dto = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.AcceptExchangeAsync(id, userId.Value, dto?.Notes);
                if (exchange == null)
                {
                    return NotFound();
                }

                // Send real-time notification about acceptance
                var fullExchange = await GetFullExchangeModelAsync(exchange.Id);
                if (fullExchange != null)
                {
                    await _notificationService.SendExchangeStatusUpdateNotificationAsync(fullExchange, ExchangeStatus.Pending);
                }

                _logger.LogInformation($"Exchange {id} accepted by user {userId}");
                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error accepting exchange {id}");
                return StatusCode(500, new { message = "An error occurred while accepting the exchange." });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ExchangeDto>> RejectExchange(int id, [FromBody] ExchangeActionDto? dto = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.RejectExchangeAsync(id, userId.Value, dto?.Notes);
                if (exchange == null)
                {
                    return NotFound();
                }

                // Send real-time notification about rejection
                var fullExchange = await GetFullExchangeModelAsync(exchange.Id);
                if (fullExchange != null)
                {
                    await _notificationService.SendExchangeStatusUpdateNotificationAsync(fullExchange, ExchangeStatus.Pending);
                }

                _logger.LogInformation($"Exchange {id} rejected by user {userId}");
                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting exchange {id}");
                return StatusCode(500, new { message = "An error occurred while rejecting the exchange." });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ExchangeDto>> CancelExchange(int id, [FromBody] ExchangeActionDto? dto = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.CancelExchangeAsync(id, userId.Value, dto?.Notes);
                if (exchange == null)
                {
                    return NotFound();
                }

                // Send real-time notification about cancellation
                var fullExchange = await GetFullExchangeModelAsync(exchange.Id);
                if (fullExchange != null)
                {
                    await _notificationService.SendExchangeStatusUpdateNotificationAsync(fullExchange, ExchangeStatus.Accepted);
                }

                _logger.LogInformation($"Exchange {id} cancelled by user {userId}");
                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling exchange {id}");
                return StatusCode(500, new { message = "An error occurred while cancelling the exchange." });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<ExchangeDto>> CompleteExchange(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.CompleteExchangeAsync(id, userId.Value);
                if (exchange == null)
                {
                    return NotFound();
                }

                // Send real-time notification about completion
                var fullExchange = await GetFullExchangeModelAsync(exchange.Id);
                if (fullExchange != null)
                {
                    await _notificationService.SendExchangeStatusUpdateNotificationAsync(fullExchange, ExchangeStatus.Accepted);
                }

                _logger.LogInformation($"Exchange {id} completed by user {userId}");
                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing exchange {id}");
                return StatusCode(500, new { message = "An error occurred while completing the exchange." });
            }
        }

        [HttpPost("{id}/no-show")]
        public async Task<ActionResult<ExchangeDto>> MarkAsNoShow(int id, [FromBody] ExchangeActionDto? dto = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var exchange = await _exchangeService.MarkAsNoShowAsync(id, userId.Value, dto?.Notes);
                if (exchange == null)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Exchange {id} marked as no-show by user {userId}");
                return Ok(exchange);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking exchange {id} as no-show");
                return StatusCode(500, new { message = "An error occurred while marking the exchange as no-show." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }
            return userId;
        }

        private async Task<SkillExchange?> GetFullExchangeModelAsync(int exchangeId)
        {
            return await _context.SkillExchanges
                .Include(e => e.Offerer)
                .Include(e => e.Learner)
                .Include(e => e.Skill)
                .FirstOrDefaultAsync(e => e.Id == exchangeId);
        }
    }
}