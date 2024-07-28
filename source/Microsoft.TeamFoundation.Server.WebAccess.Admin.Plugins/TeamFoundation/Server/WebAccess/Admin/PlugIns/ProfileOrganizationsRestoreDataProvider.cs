// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsRestoreDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.HostManagement.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsRestoreDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileOrganizationsRestore";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        Guid accountId;
        providerContext.Properties.TryGetValue<Guid>("accountId", out accountId);
        string newName;
        providerContext.Properties.TryGetValue<string>("newName", out newName);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (!UserAccountMappingMigrationHelper.QueryAccountIds(vssRequestContext, userIdentity, UserRole.Member, includeDeletedAccounts: true).Except<Guid>((IEnumerable<Guid>) UserAccountMappingMigrationHelper.QueryAccountIds(vssRequestContext, userIdentity, UserRole.Member)).Contains<Guid>(accountId))
          throw new Exception(string.Format("Requested account {0} not found ", (object) accountId));
        return (object) ProfileOrganizationsRestoreDataProvider.RestoreAndRename(vssRequestContext, accountId, newName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050095, "ProfileSettings", "Service", ex);
        throw;
      }
    }

    private static bool RestoreAndRename(
      IVssRequestContext deploymentContext,
      Guid accountId,
      string newName)
    {
      ServiceHostProperties serviceHostProperties = deploymentContext.GetService<IHostManagementService>().GetServiceHostProperties(deploymentContext, accountId);
      if (serviceHostProperties == null)
        throw new HostDoesNotExistException(accountId);
      ICreateClient clientProvider = deploymentContext.ClientProvider as ICreateClient;
      Uri hostUri = ProfileOrganizationsRestoreDataProvider.GetHostUri(deploymentContext, serviceHostProperties.ParentHostId);
      IVssRequestContext requestContext = deploymentContext;
      Uri baseUri = hostUri;
      Guid targetServicePrincipal = new Guid();
      using (OrganizationHttpClient client = clientProvider.CreateClient<OrganizationHttpClient>(requestContext, baseUri, "ProfileOrganizationDataProvider", (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal))
        return client.RestoreCollectionAsync(accountId, newName).SyncResult<bool>();
    }

    private static Uri GetHostUri(IVssRequestContext requestContext, Guid hostId) => new UriBuilder(requestContext.GetService<IPartitioningService>().QueryPartition<Guid>(requestContext, hostId, ServiceInstanceTypes.SPS).Container.Address)
    {
      Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A{0}", (object) hostId.ToString("D"))
    }.Uri;
  }
}
