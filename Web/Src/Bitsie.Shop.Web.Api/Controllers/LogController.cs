using System.Web;
using System.Web.Http;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class LogController : BaseApiController
    {
        #region Fields

        private readonly IMapperService _mapper;

        #endregion

        #region Constructor

        public LogController(IUserService userService,
            ILogService logService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a single log by ID
        /// </summary>
        /// <param name="id">Log's ID</param>
        /// <returns>Log data</returns>
        [HttpGet, RequiresApiAuth, ApiRequiresRole(Role.Administrator)]
        public LogViewModel GetOne(int id)
        {
            var log = LogService.GetLogById(id);
            if (log == null)
            {
                throw new HttpException(404, "Log not found.");
            }

            return new LogViewModel(log);
        }

        /// <summary>
        /// Retrieve a list of logs based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of logs</returns>
        [HttpGet, RequiresApiAuth, ApiRequiresRole(Role.Administrator)]
        public LogListViewModel Get([FromUri]LogListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new LogListInputModel();

            var filter = new LogFilter();
            _mapper.Map(inputModel, filter);

            var logs = LogService.GetLogs(filter, inputModel.CurrentPage, inputModel.NumPerPage);

            if (inputModel.Export)
            {
                Export("Bitsie_Logs", logs.AllItems);
                return null;
            }

            return new LogListViewModel(logs);
        }

        #endregion

        #region Private Helper Methods


        #endregion
    }

}
