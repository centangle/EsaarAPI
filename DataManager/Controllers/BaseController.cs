using Catalogs;
using Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class BaseController : ApiController
    {
        protected int CurrentPersonId
        {
            get
            {
                try
                {
                    var currentPersonId = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "PersonId").FirstOrDefault();
                    if (currentPersonId != null)
                    {
                        return int.Parse(currentPersonId.Value);
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
