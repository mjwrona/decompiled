// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsDeletedDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsDeletedDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileOrganizationsDeleted";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Guid hostId;
      providerContext.Properties.TryGetValue<Guid>("accountId", out hostId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vssRequestContext.GetUserIdentity();
      if (!UserAccountMappingMigrationHelper.QueryAccountIds(vssRequestContext, userIdentity, UserRole.Member, includeDeletedAccounts: true).Except<Guid>((IEnumerable<Guid>) UserAccountMappingMigrationHelper.QueryAccountIds(vssRequestContext, userIdentity, UserRole.Member)).Contains<Guid>(hostId))
        return (object) null;
      OrganizationHttpClient orgClient = ProfileOrganizationsDeletedDataProvider.GetOrgClient(vssRequestContext.Elevate(), hostId);
      List<string> propertyNames = new List<string>();
      propertyNames.Add("SystemProperty.PreviousName");
      propertyNames.Add("SystemProperty.ViolatedTerms");
      CancellationToken cancellationToken = new CancellationToken();
      Collection collection = orgClient.GetCollectionAsync("Me", (IEnumerable<string>) propertyNames, cancellationToken: cancellationToken).SyncResult<Collection>();
      if (collection.Properties.GetValue<bool>("SystemProperty.ViolatedTerms", false))
        return (object) null;
      return (object) new DeletedOrganizationData()
      {
        Id = hostId,
        Name = collection.Properties.GetValue<string>("SystemProperty.PreviousName", string.Empty)
      };
    }

    private static OrganizationHttpClient GetOrgClient(IVssRequestContext context, Guid hostId)
    {
      ICreateClient clientProvider = context.ClientProvider as ICreateClient;
      Uri uri = new Uri(LocationServiceHelper.GetRootLocationServiceUrl(context, hostId));
      IVssRequestContext requestContext = context;
      Uri baseUri = uri;
      Guid targetServicePrincipal = InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.SPS);
      return clientProvider.CreateClient<OrganizationHttpClient>(requestContext, baseUri, "ProfileOrganizationDataProvider", (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal);
    }
  }
}
