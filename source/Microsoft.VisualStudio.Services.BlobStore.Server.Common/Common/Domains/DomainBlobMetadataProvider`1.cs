// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainBlobMetadataProvider`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class DomainBlobMetadataProvider<TMetadataProvider> : 
    IDomainBlobMetadataProvider<TMetadataProvider>,
    IDisposable
    where TMetadataProvider : IBlobMetadataProvider
  {
    private readonly Dictionary<IDomainId, TMetadataProvider> domainTable;
    private bool disposedValue;

    public DomainBlobMetadataProvider(
      IEnumerable<(IDomainId id, TMetadataProvider provider)> domainMetadataProviders)
    {
      this.domainTable = new Dictionary<IDomainId, TMetadataProvider>();
      foreach ((IDomainId id, TMetadataProvider provider) metadataProvider in domainMetadataProviders)
        this.domainTable.Add(metadataProvider.id, metadataProvider.provider);
    }

    public TMetadataProvider GetMetadataProvider(ISecuredDomainRequest domainRequest)
    {
      if (domainRequest == null)
        throw new InvalidDomainRequestException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.DomainRequestNullError());
      TMetadataProvider metadataProvider;
      if (!this.domainTable.TryGetValue(domainRequest.DomainId, out metadataProvider))
        throw new DomainNotFoundException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.DomainNotFound((object) domainRequest.DomainId.Serialize()));
      return metadataProvider;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        foreach (KeyValuePair<IDomainId, TMetadataProvider> keyValuePair in this.domainTable)
          keyValuePair.Value.Dispose();
      }
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
