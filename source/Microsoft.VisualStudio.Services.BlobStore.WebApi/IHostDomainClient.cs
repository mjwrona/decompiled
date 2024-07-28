// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IHostDomainClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public interface IHostDomainClient
  {
    Task<IEnumerable<MultiDomainInfo>> GetDomainsAsync(CancellationToken cancellationToken);

    Task<bool> IsValidDomainAsync(string domainID, CancellationToken cancellationToken);

    Task<MultiDomainInfo> GetDefaultDomainAsync(CancellationToken cancellationToken);
  }
}
