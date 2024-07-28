// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ServerCoreApiExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ServerCoreApiExtensions
  {
    public static IdentityRef ToIdentityRef(
      this TeamFoundationIdentity identity,
      IVssRequestContext requestContext)
    {
      requestContext.CheckPermissionToReadPublicIdentityInfo();
      if (identity == null)
        return (IdentityRef) null;
      IdentityRef identityRef = new IdentityRef()
      {
        Id = identity.TeamFoundationId.ToString(),
        Descriptor = identity.GetSubjectDescriptor(requestContext),
        DisplayName = identity.DisplayName,
        UniqueName = identity.UniqueName,
        IsContainer = identity.IsContainer,
        Url = IdentityHelper.GetIdentityResourceUriString(requestContext, identity.TeamFoundationId),
        ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, identity.TeamFoundationId)
      };
      ReferenceLinks referenceLinks = new ReferenceLinks();
      string graphMemberAvatarUrl = GraphProfileUrlHelper.GetGraphMemberAvatarUrl(requestContext, identityRef.Descriptor, identity.TeamFoundationId);
      referenceLinks.AddLink("avatar", graphMemberAvatarUrl, (ISecuredObject) identityRef);
      identityRef.Links = referenceLinks;
      return identityRef;
    }

    public static string GetCoreResourceUriString(
      IVssRequestContext requestContext,
      Guid locationId,
      object routeValues)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "core", locationId, routeValues).ToString();
    }

    internal static void VerifyCollectionRequest(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }
  }
}
