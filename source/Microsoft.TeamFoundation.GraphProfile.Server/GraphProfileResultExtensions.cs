// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.Server.GraphProfileResultExtensions
// Assembly: Microsoft.TeamFoundation.GraphProfile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E08BE30A-4AE3-40A5-BE5B-3FCDC59E061E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.GraphProfile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.GraphProfile.Server
{
  public static class GraphProfileResultExtensions
  {
    public static GraphMemberAvatar CreateGraphMemberAvatar(
      IVssRequestContext requestContext,
      SubjectDescriptor memberDescriptor,
      Guid storageKey,
      byte[] imageData,
      string imageType,
      GraphMemberAvatarSize size)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      GraphMemberAvatar memberAvatar = new GraphMemberAvatar(imageData, imageType, size, new ReferenceLinks());
      GraphProfileResultExtensions.PopulateGraphMemberAvatarLinks(requestContext, memberDescriptor, storageKey, memberAvatar);
      return memberAvatar;
    }

    private static void PopulateGraphMemberAvatarLinks(
      IVssRequestContext context,
      SubjectDescriptor memberDescriptor,
      Guid storageKey,
      GraphMemberAvatar memberAvatar)
    {
      string graphMemberAvatarUrl = GraphProfileUrlHelper.GetGraphMemberAvatarUrl(context, memberDescriptor, storageKey);
      string graphSubjectUrl = GraphUrlHelper.GetGraphSubjectUrl(context, memberDescriptor);
      memberAvatar.Links.AddLink("self", graphMemberAvatarUrl, (ISecuredObject) memberAvatar);
      memberAvatar.Links.AddLink("subject", graphSubjectUrl, (ISecuredObject) memberAvatar);
    }
  }
}
