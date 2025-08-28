using System.Threading.Tasks;
using ECommerce.Business.Operations.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    /// <summary>
    /// Ayarlar: Bakım modu aç/kapat ve durumunu öğren.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;
        public SettingsController(ISettingService settingService) => _settingService = settingService;

        // Bakım modunu tersine çevir ve yeni durumu döndür
        [HttpPatch("maintenance-toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleMaintenance()
        {
            var state = await _settingService.ToggleMaintenance();
            return Ok(new { maintenance = state });
        }

        // Mevcut bakım modu durumunu döndür (anonim erişime açık)
        [HttpGet("maintenance-state")]
        [AllowAnonymous]
        public async Task<IActionResult> State()
        {
            var state = await _settingService.GetMaintenanceState();
            return Ok(new { maintenance = state });
        }
    }
}


