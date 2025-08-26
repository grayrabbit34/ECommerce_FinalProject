namespace ECommerce.Data.Entities;

public class SettingEntity : BaseEntity
{

    //proje bakımda mı değil mi? true/false
    public bool MaintenanceMode { get; set; } = false;
}
