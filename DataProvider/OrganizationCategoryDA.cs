using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<List<ItemBriefModel>> GetOrganizationCategories(int organizationId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var categoriesIds = await (from i in context.Items
                                           join oi in context.OrganizationItems on i.Id equals oi.ItemId
                                           where
                                           oi.OrganizationId == organizationId
                                           && i.IsDeleted == false
                                           && oi.IsDeleted == false
                                           && i.IsPeripheral == true
                                           && i.RootId != null
                                           select i).GroupBy(x => x.RootId).Select(x => x.Key).ToListAsync();
                return await (from i in context.Items
                              where categoriesIds.Contains(i.Id)
                              select new ItemBriefModel
                              {
                                  Id = i.Id,
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  ImageUrl = i.ImageUrl,
                              }).ToListAsync();
            }

        }
    }
}
