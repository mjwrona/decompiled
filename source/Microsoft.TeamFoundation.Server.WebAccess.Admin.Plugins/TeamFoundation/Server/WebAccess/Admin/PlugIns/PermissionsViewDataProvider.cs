// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.PermissionsView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      PermissionsViewData permissionsViewData = new PermissionsViewData();
      IClientLocationProviderService service = requestContext.GetService<IClientLocationProviderService>();
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      TeamFoundationIdentity[] foundationIdentityArray = TfsAdminIdentityHelper.ListScopedApplicationGroupsForProject(requestContext);
      return (object) TfsAdminIdentityHelper.JsonFromFilteredIdentitiesList(requestContext, new TeamFoundationFilteredIdentitiesList()
      {
        HasMoreItems = false,
        Items = foundationIdentityArray
      });
    }
  }
}
