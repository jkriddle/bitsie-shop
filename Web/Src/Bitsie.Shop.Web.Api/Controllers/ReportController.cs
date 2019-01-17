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
    public class ReportController : BaseApiController
    {
        #region Fields
        
        private readonly IMapperService _mapper;
        private readonly IReportService _reportService;

        #endregion

        #region Constructor

        public ReportController(IUserService userService,
            IReportService reportService,
            ILogService logService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
            _reportService = reportService;
        }

        #endregion

        /// <summary>
        /// Retrieve a list of logs based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of logs</returns>
        [HttpGet, RequiresApiAuth, ApiRequiresRole(Role.Merchant)]
        public DashboardReportViewModel Dashboard()
        {
            var report = _reportService.GetDashboardReport(CurrentUser.Id);
            return new DashboardReportViewModel(CurrentUser, report);
        }
    }

}
