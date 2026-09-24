using AndroidWebAPI.Repositories;
using AndroidWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueQRCodeController : ControllerBase
    {
        private readonly IQueueQRSettingRepository _settingRepository;
        private readonly IQueueQRCodeService _qrService;

        public QueueQRCodeController(
            IQueueQRSettingRepository settingRepository,
            IQueueQRCodeService qrService)
        {
            _settingRepository = settingRepository;
            _qrService = qrService;
        }

        // GET: api/QueueQRCode/settings
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var setting = await _settingRepository.GetAsync();

            if (setting == null)
            {
                return Ok(new
                {
                    isEnabled = false
                });
            }

            return Ok(new
            {
                settingID = setting.SettingID,
                isEnabled = setting.IsEnabled,
                updatedAt = setting.UpdatedAt
            });
        }

        // PUT: api/QueueQRCode/settings
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(
            [FromBody] UpdateQueueQRSettingRequest request)
        {
            var setting =
                await _settingRepository.UpdateAsync(
                    request.IsEnabled);

            return Ok(new
            {
                message = request.IsEnabled
                    ? "Queue QR code generation has been enabled."
                    : "Queue QR code generation has been disabled.",

                settingID = setting.SettingID,
                isEnabled = setting.IsEnabled,
                updatedAt = setting.UpdatedAt
            });
        }

        // POST: api/QueueQRCode/generate
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQRCode()
        {
            var qrCode =
                await _qrService.GenerateTodayAsync();

            if (qrCode == null)
            {
                return BadRequest(new
                {
                    message =
                        "QR code cannot be generated. " +
                        "Make sure queue QR generation is enabled " +
                        "and the clinic is currently open."
                });
            }

            return Ok(new
            {
                message = "Queue QR code generated successfully.",

                qrCodeID = qrCode.QRCodeID,

                token = qrCode.Token,

                qrDate = qrCode.QRDate,

                validFrom = qrCode.ValidFrom,

                validUntil = qrCode.ValidUntil,

                isActive = qrCode.IsActive
            });
        }

        // GET: api/QueueQRCode/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveQRCode()
        {
            var qrCode =
                await _qrService.GetActiveQRCodeAsync();

            if (qrCode == null)
            {
                return NotFound(new
                {
                    message = "There is no active queue QR code."
                });
            }

            return Ok(new
            {
                qrCodeID = qrCode.QRCodeID,

                token = qrCode.Token,

                qrDate = qrCode.QRDate,

                validFrom = qrCode.ValidFrom,

                validUntil = qrCode.ValidUntil,

                isActive = qrCode.IsActive
            });
        }
    }

    public class UpdateQueueQRSettingRequest
    {
        public bool IsEnabled { get; set; }
    }
}