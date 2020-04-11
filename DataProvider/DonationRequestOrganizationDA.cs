using Catalogs;
using Models;
using Models.BriefModel;
using System.Collections.Generic;

namespace DataProvider
{
    public partial class DataAccess
    {
        private void AddDonationRequestOrganizations(CharityEntities context, List<DonationRequestOrganizationModel> organizations, int donationRequestId)
        {
            foreach (var org in organizations)
            {
                var dbModel = SetDonationRequestOrganization(new DonationRequestOrganization(), org, donationRequestId);
                context.DonationRequestOrganizations.Add(dbModel);
            }
        }
        private DonationRequestOrganization SetDonationRequestOrganization(DonationRequestOrganization dbModel, DonationRequestOrganizationModel model, int donationRequestId)
        {
            dbModel.DonationRequestId = donationRequestId;
            dbModel.OrganizationId = model.Organization.Id;
            if (dbModel.Id == 0)
            {
                dbModel.Status = (int)DonationRequestStatusCatalog.Initiated;
                dbModel.AssignedTo = null;
            }
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
