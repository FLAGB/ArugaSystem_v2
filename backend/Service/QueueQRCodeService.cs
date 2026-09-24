using AndroidWebAPI.Models;
using AndroidWebAPI.Repositories;

namespace AndroidWebAPI.Services
{
    public class QueueQRCodeService : IQueueQRCodeService
    {
        private readonly IQueueQRCodeRepository _qrRepository;
        private readonly IQueueQRSettingRepository _settingRepository;
        private readonly IClinicOperatingScheduleRepository _scheduleRepository;

        public QueueQRCodeService(
            IQueueQRCodeRepository qrRepository,
            IQueueQRSettingRepository settingRepository,
            IClinicOperatingScheduleRepository scheduleRepository)
        {
            _qrRepository = qrRepository;
            _settingRepository = settingRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<QueueQRCode?> GenerateTodayAsync()
        {
            var now = DateTime.Now;
            var today = now.Date;

            // 1. Check development setting
            var setting = await _settingRepository.GetAsync();

            if (setting == null || !setting.IsEnabled)
            {
                return null;
            }

            // 2. Check if today's QR already exists
            var existingQRCode =
                await _qrRepository.GetTodayAsync(today);

            if (existingQRCode != null)
            {
                return existingQRCode;
            }

            // 3. Check for today's special schedule exception
            var exception =
                await _scheduleRepository.GetExceptionAsync(today);

            TimeSpan openingTime;
            TimeSpan closingTime;
            TimeSpan cutoffTime;
            bool isOpen;

            if (exception != null)
            {
                isOpen = exception.IsOpen;
                openingTime = exception.OpeningTime;
                closingTime = exception.ClosingTime;
                cutoffTime = exception.QueueCutoffTime;
            }
            else
            {
                // 4. Get normal operating schedule
                var dayOfWeek = (int)today.DayOfWeek;

                var schedule =
                    await _scheduleRepository.GetByDayAsync(dayOfWeek);

                if (schedule == null)
                {
                    return null;
                }

                isOpen = schedule.IsOpen;
                openingTime = schedule.OpeningTime;
                closingTime = schedule.ClosingTime;
                cutoffTime = schedule.QueueCutoffTime;
            }

            // 5. Clinic must be open
            if (!isOpen)
            {
                return null;
            }

            // 6. Current time must be within clinic hours
            var currentTime = now.TimeOfDay;

            if (currentTime < openingTime ||
                currentTime > closingTime)
            {
                return null;
            }

            // 7. Generate QR token
            var token = Guid.NewGuid().ToString("N");

            var qrCode = new QueueQRCode
            {
                QRCodeID = Guid.NewGuid(),

                Token = token,

                QRDate = today,

                ValidFrom = today.Add(openingTime),

                ValidUntil = today.Add(cutoffTime),

                IsActive = true,

                CreatedAt = now
            };

            // 8. Deactivate previous active QR codes
            await _qrRepository.DeactivateAllAsync();

            // 9. Save today's QR
            return await _qrRepository.CreateAsync(qrCode);
        }

        public async Task<QueueQRCode?> GetActiveQRCodeAsync()
        {
            return await _qrRepository.GetActiveAsync();
        }
    }
}