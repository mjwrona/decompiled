// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectExtensions
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphSubjectExtensions
  {
    internal static void FillSerializeInternalsField(
      this GraphSubject graphSubject,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (graphSubject == null || !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
        return;
      graphSubject.ShoudSerializeInternals = true;
    }
  }
}
