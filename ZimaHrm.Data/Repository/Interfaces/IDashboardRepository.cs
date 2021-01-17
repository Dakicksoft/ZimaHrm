using System.Collections.Generic;
using System.Threading.Tasks;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IDashboardRepository 
    {
        Task<int> TotalEmplooyeeAsync();
        Task<int> TotalDepartmentAsync();
        Task<int> TotalPresentAsync();
        Task<int> TotalAbsentAsync();
        Task<IEnumerable<Notice>> LastFiveNotificationsAsync();
        Task<IEnumerable<Holiday>> LastFiveHolidaysAsync();
    }
}
