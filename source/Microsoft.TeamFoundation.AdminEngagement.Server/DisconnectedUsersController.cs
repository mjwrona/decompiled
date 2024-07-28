// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.AdminEngagement.WebApi.DisconnectedUsersController
// Assembly: Microsoft.TeamFoundation.AdminEngagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DC53F57-597F-449E-A165-8D6CA5E396C1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.AdminEngagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.AdminEngagement.WebApi
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "OrganizationSettings", ResourceName = "DisconnectedUser")]
  public class DisconnectedUsersController : TfsApiController
  {
    private const string orgUserThresholdRegistryPath = "/Configuration/Service/AdminEngagement/TooManyOrgUsersThreshold";

    [HttpGet]
    public DisconnectedUsers GetDisconnectedUsers(Guid? tenantId = null)
    {
      this.TfsRequestContext.TraceEnter(10050052, "DisconnectedUser", "RestApi", nameof (GetDisconnectedUsers));
      DisconnectedUsers disconnectedUsers = new DisconnectedUsers();
      if (!tenantId.HasValue)
      {
        tenantId = new Guid?(DisconnectedUsersController.GetOrgTenantId(this.TfsRequestContext));
        if (!tenantId.HasValue)
          throw new Exception("You must select a tenant.");
      }
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext elevatedDeploymentContext = vssRequestContext.Elevate();
      AadService service1 = elevatedDeploymentContext.GetService<AadService>();
      AadTenant tenant;
      try
      {
        AadService aadService = service1;
        IVssRequestContext context = elevatedDeploymentContext;
        GetTenantRequest request = new GetTenantRequest();
        request.ToTenant = tenantId.Value.ToString();
        tenant = aadService.GetTenant(context, request)?.Tenant;
        if (tenant == null)
          throw new Exception("tenant id is not valid!");
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(10050055, TraceLevel.Error, "DisconnectedUser", "RestApi", ex);
        throw;
      }
      List<Microsoft.VisualStudio.Services.Identity.Identity> list1;
      try
      {
        IdentityService service2 = this.TfsRequestContext.GetService<IdentityService>();
        list1 = service2.ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) service2.ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        }, QueryMembership.Expanded, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>().Members.ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && !identity.IsImported && IdentityHelper.IsUserIdentity(elevatedDeploymentContext, (IReadOnlyVssIdentity) identity))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        disconnectedUsers.OrgUserCount = list1.Count;
        int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/Service/AdminEngagement/TooManyOrgUsersThreshold", 100);
        if (disconnectedUsers.OrgUserCount > num)
        {
          disconnectedUsers.ShowTooManyUsersWarning = true;
          this.TfsRequestContext.Trace(10050054, TraceLevel.Warning, "DisconnectedUser", "RestApi", string.Format("Organization has too many users: {0} users. We're not querying AAD for now.", (object) list1.Count));
          this.TfsRequestContext.TraceLeave(10050052, "DisconnectedUser", "RestApi", nameof (GetDisconnectedUsers));
          return disconnectedUsers;
        }
        this.TfsRequestContext.Trace(10050059, TraceLevel.Info, "DisconnectedUser", "RestApi", string.Format("Organization has {0} users.", (object) list1.Count));
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(10050056, TraceLevel.Error, "DisconnectedUser", "RestApi", ex);
        throw;
      }
      int count = 0;
      while (count < list1.Count)
      {
        try
        {
          IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = list1.Skip<Microsoft.VisualStudio.Services.Identity.Identity>(count).Take<Microsoft.VisualStudio.Services.Identity.Identity>(10).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsAADServicePrincipal));
          count += 10;
          Dictionary<Microsoft.VisualStudio.Services.Identity.Identity, DisconnectedUsersController.UserUpn> idToUpn = new Dictionary<Microsoft.VisualStudio.Services.Identity.Identity, DisconnectedUsersController.UserUpn>();
          foreach (Microsoft.VisualStudio.Services.Identity.Identity key in source)
          {
            string lowerInvariant = key.GetProperty<string>("Account", string.Empty)?.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(lowerInvariant))
              idToUpn.Add(key, new DisconnectedUsersController.UserUpn(DisconnectedUsersController.BuildForeignUserUpnPrefix(tenant, lowerInvariant), lowerInvariant));
          }
          List<AadUser> userBatchInTenant = new List<AadUser>();
          userBatchInTenant.AddRange(DisconnectedUsersController.GetUsersInTenant(idToUpn.Values.Select<DisconnectedUsersController.UserUpn, string>((Func<DisconnectedUsersController.UserUpn, string>) (val => val.GuestUpn)).ToArray<string>(), tenantId.Value.ToString(), service1, elevatedDeploymentContext, this.TfsRequestContext));
          userBatchInTenant.AddRange(DisconnectedUsersController.GetUsersInTenant(idToUpn.Values.Select<DisconnectedUsersController.UserUpn, string>((Func<DisconnectedUsersController.UserUpn, string>) (val => val.Upn)).ToArray<string>(), tenantId.Value.ToString(), service1, elevatedDeploymentContext, this.TfsRequestContext));
          List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (ib => !userBatchInTenant.Any<AadUser>((Func<AadUser, bool>) (ub => ub.UserPrincipalName.IndexOf(idToUpn[ib].Upn, StringComparison.OrdinalIgnoreCase) >= 0 || ub.UserPrincipalName.IndexOf(idToUpn[ib].GuestUpn, StringComparison.OrdinalIgnoreCase) >= 0)))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          disconnectedUsers.Users.AddRange(list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRefWithEmail>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRefWithEmail>) (ub =>
          {
            return new IdentityRefWithEmail()
            {
              DisplayName = ub.DisplayName,
              Id = ub.Id.ToString(),
              Descriptor = ub.SubjectDescriptor,
              PreferredEmailAddress = ub.Properties.ContainsKey("mail") ? ub.Properties["mail"].ToString() : ""
            };
          })));
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(10050053, TraceLevel.Error, "DisconnectedUser", "RestApi", ex);
        }
      }
      this.TfsRequestContext.TraceLeave(10050052, "DisconnectedUser", "RestApi", nameof (GetDisconnectedUsers));
      return disconnectedUsers;
    }

    private static Guid GetOrgTenantId(IVssRequestContext requestContext)
    {
      Guid orgTenantId = Guid.Empty;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
      if (organization != null && organization.TenantId != Guid.Empty)
        orgTenantId = organization.TenantId;
      return orgTenantId;
    }

    private static IEnumerable<AadUser> GetUsersInTenant(
      string[] userPrincipalNamePrefixes,
      string tenantId,
      AadService aadService,
      IVssRequestContext elevatedDeploymentContext,
      IVssRequestContext requestContext)
    {
      try
      {
        Dictionary<string, AadUser> dictionary = new Dictionary<string, AadUser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        GetUsersRequest getUsersRequest = new GetUsersRequest();
        getUsersRequest.ToTenant = tenantId;
        getUsersRequest.UserPrincipalNamePrefixes = (IEnumerable<string>) userPrincipalNamePrefixes;
        GetUsersRequest request = getUsersRequest;
        GetUsersResponse users1 = aadService.GetUsers(elevatedDeploymentContext, request);
        List<AadUser> usersInTenant;
        if (users1 == null)
        {
          usersInTenant = (List<AadUser>) null;
        }
        else
        {
          IEnumerable<AadUser> users2 = users1.Users;
          usersInTenant = users2 != null ? users2.Where<AadUser>((Func<AadUser, bool>) (x => x != null && x.AccountEnabled)).ToList<AadUser>() : (List<AadUser>) null;
        }
        return (IEnumerable<AadUser>) usersInTenant;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050053, TraceLevel.Error, "DisconnectedUser", "RestApi", ex);
        return Enumerable.Empty<AadUser>();
      }
    }

    private static string BuildForeignUserUpnPrefix(AadTenant aadTenant, string accountName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(accountName, nameof (accountName));
      return accountName.Replace('@', '_') + "#EXT#";
    }

    internal class UserUpn
    {
      public string GuestUpn { get; set; }

      public string Upn { get; set; }

      public UserUpn(string guestUpn, string upn)
      {
        this.GuestUpn = guestUpn;
        this.Upn = upn;
      }
    }
  }
}
