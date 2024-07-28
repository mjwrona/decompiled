// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.HostDomainClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class HostDomainClient : IHostDomainClient
  {
    private readonly IHostDomainHttpClient client;

    public HostDomainClient(IHostDomainHttpClient client) => this.client = client;

    public async Task<MultiDomainInfo> GetDefaultDomainAsync(CancellationToken cancellationToken) => (await this.client.GetDomainsAsync(cancellationToken)).Single<MultiDomainInfo>((Func<MultiDomainInfo, bool>) (domain => domain.IsDefault));

    public async Task<IEnumerable<MultiDomainInfo>> GetDomainsAsync(
      CancellationToken cancellationToken)
    {
      return await this.client.GetDomainsAsync(cancellationToken);
    }

    public async Task<bool> IsValidDomainAsync(string domainId, CancellationToken cancellationToken) => await this.client.GetDomainAsync(domainId, cancellationToken) != null;
  }
}
