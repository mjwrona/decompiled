// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Extensions.IdentityPickerRestApiResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud.Extensions
{
  public sealed class IdentityPickerRestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, CommonIdentityPickerResourceIds.IdentitiesLocationId, "IdentityPicker", "Identities", "{area}/{resource}", VssRestApiVersion.v1_0, defaults: (object) new
      {
      }, routeName: "Identities");
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, CommonIdentityPickerResourceIds.IdentityAvatarLocationId, "IdentityPicker", "Identities", "{area}/{resource}/{objectId}/avatar", VssRestApiVersion.v1_0, defaults: (object) new
      {
        action = "GetAvatar"
      }, routeName: "IdentityAvatar");
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, CommonIdentityPickerResourceIds.IdentityFeatureMruLocationId, "IdentityPicker", "Identities", "{area}/{resource}/{objectId}/mru/{featureId}", VssRestApiVersion.v1_0, defaults: (object) new
      {
      }, routeName: "IdentityFeatureMru");
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, CommonIdentityPickerResourceIds.IdentityConnectionsLocationId, "IdentityPicker", "Identities", "{area}/{resource}/{objectId}/connections", VssRestApiVersion.v1_0, defaults: (object) new
      {
        action = "GetConnections"
      }, routeName: "IdentityConnections");
    }
  }
}
