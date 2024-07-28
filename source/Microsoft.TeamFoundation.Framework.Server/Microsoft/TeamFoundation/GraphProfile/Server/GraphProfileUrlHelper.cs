// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.Server.GraphProfileUrlHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Profile;
using System;

namespace Microsoft.TeamFoundation.GraphProfile.Server
{
  internal static class GraphProfileUrlHelper
  {
    private const string Area = "GraphProfile";
    private const string Layer = "GraphProfileUrlHelper";

    public static string GetGraphMemberAvatarUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor memberDescriptor,
      Guid storageKey)
    {
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return GraphProfileUrlHelper.GetGraphMemberAvatarCollectionOrEnterpriseUrl(requestContext, memberDescriptor);
        return memberDescriptor.IsCuidBased() ? GraphProfileUrlHelper.GetProfileLocationDataProvider(requestContext).GetResourceUri(requestContext, "Profile", ProfileResourceIds.AvatarLocationid, (object) new
        {
          storageKey = storageKey
        }).AbsoluteUri : string.Empty;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008201, "GraphProfile", nameof (GraphProfileUrlHelper), ex);
        return string.Empty;
      }
    }

    private static string GetGraphMemberAvatarCollectionOrEnterpriseUrl(
      IVssRequestContext collectionOrEnterpriseContext,
      SubjectDescriptor memberDescriptor)
    {
      collectionOrEnterpriseContext.CheckProjectCollectionOrOrganizationRequestContext();
      return GraphProfileUrlHelper.GetGraphProfileLocationDataProvider(collectionOrEnterpriseContext).GetResourceUri(collectionOrEnterpriseContext, "GraphProfile", GraphProfileResourceIds.MemberAvatars.MemberAvatarsLocationId, (object) new
      {
        memberDescriptor = memberDescriptor
      }).AbsoluteUri;
    }

    private static ILocationDataProvider GetGraphProfileLocationDataProvider(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ILocationService>().GetLocationData(requestContext, GraphProfileResourceIds.AreaIdGuid);
    }

    private static ILocationDataProvider GetProfileLocationDataProvider(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ILocationService>().GetLocationData(requestContext, ProfileResourceIds.AreaIdGuid);
    }
  }
}
