// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsRestoreAvailableDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsRestoreAvailableDataProvider : IExtensionDataProvider
  {
    private const string c_throwWhenAccountNotFoundInRemoteSpsInstanceFF = "Sps.Web.Account.ThrowWhenDeletedAccountIsNotFoundInRemoteSpsInstance";

    public string Name => "Admin.ProfileOrganizationsRestoreAvailable";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Guid guid;
      providerContext.Properties.TryGetValue<Guid>("accountId", out guid);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string collectionName;
      try
      {
        IVssRequestContext requestContext1 = vssRequestContext.Elevate();
        collectionName = HttpClientHelper.CreateSpsClient<OrganizationHttpClient>(requestContext1, guid, InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.SPS)).GetCollectionAsync("Me", (IEnumerable<string>) new string[1]
        {
          "SystemProperty.PreviousName"
        }, cancellationToken: requestContext1.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>().Properties.GetValue<string>("SystemProperty.PreviousName", string.Empty);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050094, "ProfileSettings", "Service", ex);
        if (vssRequestContext.IsFeatureEnabled("Sps.Web.Account.ThrowWhenDeletedAccountIsNotFoundInRemoteSpsInstance"))
          throw new Exception("Requested account " + guid.ToString() + " not found ");
        return (object) false;
      }
      if (string.IsNullOrEmpty(collectionName))
        return (object) false;
      try
      {
        ((NameAvailabilityHelper) new NameReservationHelper()).CheckCollectionNameReservation(vssRequestContext, collectionName, guid, vssRequestContext.UseDevOpsDomainUrls());
      }
      catch (Exception ex)
      {
        return (object) false;
      }
      return (object) true;
    }
  }
}
