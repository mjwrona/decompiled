// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMemberExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphMemberExtensions
  {
    public static Guid GetStorageKey(
      this GraphMember graphMember,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<GraphMember>(graphMember, nameof (graphMember));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(requestContext, graphMember.Descriptor);
    }

    public static AadGraphMember ToAadGraphMemberClient(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (identity == null)
        return (AadGraphMember) null;
      if (identity.IsAADServicePrincipal)
        return (AadGraphMember) identity.ToGraphServicePrincipalClient(context);
      return !identity.IsContainer ? (AadGraphMember) identity.ToGraphUserClient(context) : throw new ArgumentException(FrameworkResources.RequiresNonContainerIdentity((object) identity.SubjectDescriptor));
    }
  }
}
