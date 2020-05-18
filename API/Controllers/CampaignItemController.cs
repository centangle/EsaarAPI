using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.ViewModels.Campaign;
using BusinessLogic;
using Catalogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.BriefModel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignItemController : BaseController
    {
        private readonly Logic _logic;

        public CampaignItemController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int campaignId, SearchItemTypeCatalog itemType, string itemName = null, string orderByColumn = null, bool calculateTotal = true)
        {

            OrganizationItemSearchModel filters = new OrganizationItemSearchModel();
            filters.ItemName = itemName;
            filters.CampaignId = campaignId;
            filters.Type = itemType;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetCampaignItems(filters);
        }

        [HttpPost]
        [Route("Modify")]
        public async Task<bool> Modify(CampaignItemViewModel model)
        {
            List<OrganizationItemModel> campaignItems = new List<OrganizationItemModel>();
            foreach (var item in model.Items)
            {
                OrganizationItemModel campaignItem = new OrganizationItemModel
                {
                    Id = item.Id,
                    Item = new ItemBriefModel
                    {
                        Id = item.Item.Id,
                    },
                    CampaignItemUOM = new UOMBriefModel
                    {
                        Id = item.CampaignItemUOM.Id,
                    },
                    CampaignItemTarget = item.CampaignItemTarget

                };
                campaignItems.Add(campaignItem);

            }
            return await _logic.ModifyCampaignItems(model.CampaignId, campaignItems);
        }
    }
}