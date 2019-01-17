using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Api.Models
{
    public class LogListViewModel : PagedViewModel<Log>
    {
        #region Constructor

        public LogListViewModel(IPagedList<Log> logs)
            : base(logs)
        {
            Logs = new List<LogViewModel>();
            foreach (var log in logs.Items)
            {
                Logs.Add(new LogViewModel(log));
            }
        }

        #endregion

        #region Public Properties

        public IList<LogViewModel> Logs { get; set; }

        #endregion

    }
}