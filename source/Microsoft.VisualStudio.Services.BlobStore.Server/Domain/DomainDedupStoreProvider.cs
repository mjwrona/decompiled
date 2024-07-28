// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.DomainDedupStoreProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  public class DomainDedupStoreProvider
  {
    private readonly IDomainId defaultDomainId;
    private readonly Dictionary<IDomainId, IDedupProvider> dedupStoreProviders;

    public DomainDedupStoreProvider(IDomainId defaultDomainId)
    {
      this.defaultDomainId = defaultDomainId;
      this.dedupStoreProviders = new Dictionary<IDomainId, IDedupProvider>();
    }

    public void AddDedupProvider(IDomainId domainId, IDedupProvider dedupProvider) => this.dedupStoreProviders.TryAdd<IDomainId, IDedupProvider>(domainId, dedupProvider);

    public virtual IDedupProvider GetProvider(ISecuredDomainRequest domainRequest)
    {
      IDedupProvider provider;
      if (!this.dedupStoreProviders.TryGetValue(domainRequest.DomainId, out provider))
        throw new InvalidOperationException("Provider backing domain id: " + domainRequest.DomainId.Serialize() + " was not found.");
      return provider;
    }

    public IDedupProvider GetDefaultProvider()
    {
      IDedupProvider defaultProvider;
      if (!this.dedupStoreProviders.TryGetValue(this.defaultDomainId, out defaultProvider))
        throw new InvalidOperationException("Provider backing domain id: " + this.defaultDomainId.Serialize() + " was not found.");
      return defaultProvider;
    }

    public Dictionary<IDomainId, IDedupProvider> GetProviders() => this.dedupStoreProviders;
  }
}
