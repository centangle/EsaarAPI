using Catalogs;
using Helpers;
using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        private async Task AddDonationRequestOrganizations(CharityEntities context, List<DonationRequestOrganizationModel> organizations, int donationRequestId)
        {
            foreach (var org in organizations)
            {
                var dbModel = SetDonationRequestOrganization(new DonationRequestOrganization(), org, donationRequestId);
                context.DonationRequestOrganizations.Add(dbModel);
                await context.SaveChangesAsync();
                org.Id = dbModel.Id;
            }
        }
        private DonationRequestOrganization SetDonationRequestOrganization(DonationRequestOrganization dbModel, DonationRequestOrganizationModel model, int donationRequestId)
        {
            dbModel.DonationRequestId = donationRequestId;
            dbModel.OrganizationId = model.Organization.Id;
            if (dbModel.Id == 0)
            {
                dbModel.Status = (int)StatusCatalog.Initiated;
                dbModel.ModeratorId = null;
            }
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        private List<DonationRequestOrganizationModel> GetDonationRequestOrganization(DonationRequestModel model)
        {
            if (model.OrganizationId < 1)
            {
                throw new KnownException("Organization is required");
            }
            else
            {
                return new List<DonationRequestOrganizationModel> {new DonationRequestOrganizationModel
                {
                    Organization=new BaseBriefModel
                    {
                        Id=model.OrganizationId
                    }
                }};
            }
        }
        private RequestThreadModel GetDonationRequestThreadModel(int entityId)
        {
            RequestThreadModel requestThreadModel = new RequestThreadModel();
            requestThreadModel.Entity.Id = entityId;
            requestThreadModel.EntityType = RequestThreadEntityTypeCatalog.Donation;
            requestThreadModel.Status = StatusCatalog.Initiated;
            requestThreadModel.Type = RequestThreadTypeCatalog.General;
            requestThreadModel.IsSystemGenerated = true;
            return requestThreadModel;
        }
        private async Task<bool> IsDonationRequestThreadAccessible(CharityEntities context, RequestThreadModel requestModel)
        {
            var orgRequest = await context.DonationRequestOrganizations.Where(x => x.Id == requestModel.Entity.Id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (orgRequest != null)
            {
                if (orgRequest.CreatedBy == _loggedInMemberId)
                {
                    return true;
                }
                else
                {
                    if (orgRequest.ModeratorId == _loggedInMemberId)
                    {
                        return true;
                    }
                    else
                    {
                        var organizationMember = (await GetMemberRoleForOrganization(context, orgRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                        if (IsOrganizationMemberOwner(organizationMember))
                        {
                            return true;
                        }
                    }
                }
                throw new KnownException("You are not authorized to perform this action");
            }
            throw new KnownException("Request has been deleted");
        }
        private async Task<bool> ChangeDonationRequestStatus(CharityEntities context, RequestThreadModel model)
        {
            var donationOrganizationRequest = await context.DonationRequestOrganizations.Where(x => x.Id == model.Entity.Id).FirstOrDefaultAsync();
            if (donationOrganizationRequest != null)
            {
                var organizationMember = (await GetMemberRoleForOrganization(context, donationOrganizationRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(organizationMember))
                {
                    if (donationOrganizationRequest.IsDeleted == true)
                    {
                        return false;
                    }
                    donationOrganizationRequest.Status = (int)model.Status;
                    return true;
                }
                else
                {
                    throw new KnownException("You are not authorized to change status.");
                }
            }
            else
            {
                return false;
            }
        }
    }
}
