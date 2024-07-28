// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CollectionCatalogRules
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CollectionCatalogRules
  {
    internal static void ValidateCollectionExists(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogSerivce,
      CatalogRuleValidationUtility utility,
      CatalogResource resource)
    {
      TeamFoundationHostManagementService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
      Guid collectionId = new Guid(resource.Properties["InstanceId"]);
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      Guid hostId = collectionId;
      if (service.QueryServiceHostProperties(requestContext1, hostId, ServiceHostFilterFlags.IncludeActiveServicingDetails) == null)
        throw new CollectionDoesNotExistException(collectionId);
    }
  }
}
