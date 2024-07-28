// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileOrganizationsView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      (IList<Guid> guidList1, IList<Guid> deleted1) = ProfileOrganizationsViewDataProvider.GetAccounts(context, userIdentity, UserRole.Owner);
      (IList<Guid> guidList2, IList<Guid> deleted2) = ProfileOrganizationsViewDataProvider.GetAccounts(context, userIdentity, UserRole.Member);
      IList<Guid> list = (IList<Guid>) guidList2.Except<Guid>((IEnumerable<Guid>) guidList1).ToList<Guid>();
      ProfileOrganizationsViewDataProvider.GetOrganizationData(context, guidList1);
      return (object) new OrganizationsViewData()
      {
        OwnerAccounts = ProfileOrganizationsViewDataProvider.GetOrganizationData(context, guidList1),
        MemberAccounts = ProfileOrganizationsViewDataProvider.GetOrganizationData(context, list),
        OwnerDeletedAccountIds = deleted1,
        MemberDeletedAccountIds = deleted2,
        IsAadUser = AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) userIdentity)
      };
    }

    private static IList<OrganizationData> GetOrganizationData(
      IVssRequestContext context,
      IList<Guid> accountIds)
    {
      IList<HostProperties> hostPropertiesList = context.GetService<TeamFoundationHostManagementService>().QueryServiceHostPropertiesBatch(context, (ICollection<Guid>) (accountIds as List<Guid>));
      List<OrganizationData> organizationData = new List<OrganizationData>();
      foreach (HostProperties hostProperties in (IEnumerable<HostProperties>) hostPropertiesList)
        organizationData.Add(new OrganizationData()
        {
          Id = hostProperties.Id,
          Name = hostProperties.Name,
          Description = hostProperties.Description,
          LastUserAccess = hostProperties.LastUserAccess
        });
      return (IList<OrganizationData>) organizationData;
    }

    private static (IList<Guid> active, IList<Guid> deleted) GetAccounts(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      UserRole role)
    {
      IList<Guid> second = UserAccountMappingMigrationHelper.QueryAccountIds(context, identity, role, role == UserRole.Member);
      IList<Guid> first = UserAccountMappingMigrationHelper.QueryAccountIds(context, identity, role, role == UserRole.Member, true);
      return (second, (IList<Guid>) first.Except<Guid>((IEnumerable<Guid>) second).ToList<Guid>());
    }
  }
}
