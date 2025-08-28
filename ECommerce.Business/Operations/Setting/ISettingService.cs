using System;
namespace ECommerce.Business.Operations.Setting
{
 
    public interface ISettingService
    {
        // Bakım modunu tersine çevirir, yeni durumu döner.
        Task<bool> ToggleMaintenance();

        // Mevcut bakım modu durumunu döner.
        Task<bool> GetMaintenanceState();
    }

}

