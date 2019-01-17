using System;
using System.Collections.Generic;
using System.Linq;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;

namespace Bitsie.Shop.Services
{
    public class LogService : ILogService
    {
        #region Fields

        private readonly ILogRepository _logRepository;
        private readonly LogLevel _minLevel;

        #endregion

        #region Constructor

        public LogService(ILogRepository logRepository, LogLevel minLevel)
        {
            _logRepository = logRepository;
            _minLevel = minLevel;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Add a new log to the system
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(Log log)
        {
            if (log.Level >= _minLevel)
            {
                _logRepository.Save(log);
            }
        }

        /// <summary>
        /// Retrieve a single log
        /// </summary>
        /// <param name="id">Log's unique ID</param>
        /// <returns>Matching log</returns>
        public Log GetLogById(int id)
        {
            return _logRepository.FindOne(id);
        }

        /// <summary>
        /// Retrieve a paged list of logs
        /// </summary>
        /// <param name="filter">Query to filter logs</param>
        /// <param name="currentPage">Current page number</param>
        /// <param name="numPerPage">Number of items per page</param>
        /// <returns></returns>
        public IPagedList<Log> GetLogs(LogFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<Log> logs = _logRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<Log>(logs, currentPage, numPerPage, totalRecords);
        }
        

        #endregion


    }
}
