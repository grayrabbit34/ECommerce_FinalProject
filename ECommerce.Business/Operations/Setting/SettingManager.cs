using System;
using System.Threading.Tasks;                 // Task için gerekli
using ECommerce.Data.Entities;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWork;

namespace ECommerce.Business.Operations.Setting
{
    /// <summary>
    /// Settings tablosunu yönetir (tekil kayıt: Id = 1).
    /// </summary>
    public class SettingManager : ISettingService
    {
        private readonly IRepository<SettingEntity> _settingRepo;
        private readonly IUnitOfWork _uow;

        public SettingManager(IRepository<SettingEntity> settingRepo, IUnitOfWork uow)
        {
            _settingRepo = settingRepo;
            _uow = uow;
        }

        /// <summary>
        /// Mevcut bakım modu durumunu döner.
        /// </summary>
        public Task<bool> GetMaintenanceState()
        {
            var s = _settingRepo.Get(x => x.Id == 1);
            return Task.FromResult(s?.MaintenanceMode ?? false);
        }

        /// <summary>
        /// Bakım modunu tersine çevirir ve yeni durumu döner.
        /// </summary>
        public async Task<bool> ToggleMaintenance()
        {
            var s = _settingRepo.Get(x => x.Id == 1) ?? new SettingEntity { Id = 1 };

            s.MaintenanceMode = !s.MaintenanceMode;

            if (s.CreatedDate == default)
            {
                s.CreatedDate = DateTime.UtcNow;
                _settingRepo.Add(s);
            }
            else
            {
                s.ModifiedDate = DateTime.UtcNow;
                _settingRepo.Update(s);
            }

            await _uow.SaveChangesAsync();
            return s.MaintenanceMode;
        }
    }
}
