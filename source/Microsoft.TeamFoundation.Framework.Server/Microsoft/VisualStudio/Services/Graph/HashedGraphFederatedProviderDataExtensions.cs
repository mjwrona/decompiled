// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.HashedGraphFederatedProviderDataExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class HashedGraphFederatedProviderDataExtensions
  {
    public static HashedGraphFederatedProviderData Hashed(
      this GraphFederatedProviderData providerData,
      IVssRequestContext requestContext)
    {
      return providerData == null ? (HashedGraphFederatedProviderData) null : new HashedGraphFederatedProviderData(providerData, requestContext);
    }
  }
}
