// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.HashedGraphFederatedProviderData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class HashedGraphFederatedProviderData
  {
    public SubjectDescriptor SubjectDescriptor { get; }

    public string ProviderName { get; }

    public byte[] AccessTokenHash { get; }

    public HashedGraphFederatedProviderData(
      GraphFederatedProviderData providerData,
      IVssRequestContext requestContext)
    {
      this.SubjectDescriptor = providerData.SubjectDescriptor;
      this.ProviderName = providerData.ProviderName;
      this.AccessTokenHash = TokenHasher.Hash(requestContext, providerData.AccessToken);
    }
  }
}
