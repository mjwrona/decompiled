// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsLeaveDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.MemberEntitlementManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsLeaveDataProvider : IExtensionDataProvider
  {
    private static readonly Guid AexServiceIdentity = new Guid("00000041-0000-8888-8000-000000000000");

    public string Name => "Admin.ProfileOrganizationsLeave";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Guid hostId;
      providerContext.Properties.TryGetValue<Guid>("accountId", out hostId);
      try
      {
        ProfileOrganizationsLeaveDataProvider.GetMemberEntitlementClient(requestContext.To(TeamFoundationHostType.Deployment), hostId).DeleteUserEntitlementAsync(requestContext.GetUserId(), (object) null, new CancellationToken()).SyncResult();
        return (object) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050093, "ProfileSettings", "Service", ex);
        throw;
      }
    }

    private static MemberEntitlementManagementHttpClient GetMemberEntitlementClient(
      IVssRequestContext context,
      Guid hostId)
    {
      Uri hostUri = context.GetService<IUrlHostResolutionService>().GetHostUri(context, hostId, ProfileOrganizationsLeaveDataProvider.AexServiceIdentity);
      return (context.ClientProvider as ICreateClient).CreateClient<MemberEntitlementManagementHttpClient>(context, hostUri, "ProfileOrganizationDataProvider", (ApiResourceLocationCollection) null);
    }
  }
}
