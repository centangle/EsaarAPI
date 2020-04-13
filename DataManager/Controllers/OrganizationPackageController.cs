using BusinessLogic;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class OrganizationPackageController : BaseController
    {
        [HttpPost]
        public async Task<int> Create(ItemModel model, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            model.Organization = new BaseBriefModel { Id = organizationId };
            return await _logic.CreateItem(model);
        }
        [HttpPut]
        public async Task<bool> Update(ItemModel model, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            model.Organization = new BaseBriefModel { Id = organizationId };
            return await _logic.UpdateItem(model);
        }
        [HttpPost]
        public async Task<bool> CreateSinglePackageWithChildrens(ItemModel model, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleItemWithChildrens(model, organizationId);
        }
        [HttpPut]
        public async Task<bool> UpdateSinglePackageWithChildren(ItemModel model, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleItemWithChildren(model, organizationId);
        }
        [HttpPost]
        public async Task<bool> CreateMultiplePackagesWithChildrens(List<ItemModel> items, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleItemsWithChildrens(items, organizationId);
        }
        [HttpPut]
        public async Task<bool> UpdateMultiplePackagesWithChildrens(List<ItemModel> items, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleItemsWithChildrens(items, organizationId);
        }

        [HttpDelete]
        public async Task<bool> DeletePackageWithChildren(int id, int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteItem(id, organizationId);
        }
    }
}
