using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;

namespace Bitsie.Shop.Services
{
    
    public interface ILogService
    {
        Log GetLogById(int id);
        IPagedList<Log> GetLogs(LogFilter filter, int currentPage, int numPerPage);
        void CreateLog(Log log);
    }
}
