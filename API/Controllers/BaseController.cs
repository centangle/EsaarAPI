using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Base;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int LoggedInMemberId
        {
            get
            {
                try
                {
                    var loggedInMemberId = User.Claims.Where(c => c.Type == "MemberId").FirstOrDefault();/* ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "MemberId").FirstOrDefault();*/
                    if (loggedInMemberId != null)
                    {
                        return int.Parse(loggedInMemberId.Value);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        protected void SetPaginationProperties(BaseSearchModel searchModel, int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, string orderByColumn, bool disablePagination, bool calculateTotal)
        {
            searchModel.RecordsPerPage = recordsPerPage;
            searchModel.CurrentPage = currentPage;
            searchModel.OrderDir = orderDir;
            if (!string.IsNullOrEmpty(orderByColumn))
                searchModel.OrderByColumn = orderByColumn;
            searchModel.DisablePagination = disablePagination;
            searchModel.CalculateTotal = calculateTotal;
        }
    }
}