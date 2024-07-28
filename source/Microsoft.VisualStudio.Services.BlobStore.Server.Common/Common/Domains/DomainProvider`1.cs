// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainProvider`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class DomainProvider<TDomain> : IDomainProvider<TDomain> where TDomain : IDomain
  {
    private readonly IDomainId defaultDomainId;
    private readonly Dictionary<IDomainId, TDomain> domainTable;

    public DomainProvider(IDomainId defaultDomainId, IEnumerable<TDomain> physicalDomains)
    {
      this.defaultDomainId = defaultDomainId ?? throw new ArgumentNullException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.DefaultDomainNullError());
      this.domainTable = new Dictionary<IDomainId, TDomain>();
      foreach (TDomain physicalDomain in physicalDomains)
        this.domainTable.Add(physicalDomain.Id, physicalDomain);
      if (!this.domainTable.ContainsKey(defaultDomainId))
        throw new ArgumentException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.ValidDefaultDomainRequired((object) defaultDomainId.Serialize()));
    }

    public IDomainId GetDefaultDomainId() => this.defaultDomainId;

    public TDomain GetDomain(ISecuredDomainRequest domainRequest)
    {
      if (domainRequest == null)
        throw new InvalidDomainRequestException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.DomainRequestNullError());
      TDomain domain;
      if (!this.domainTable.TryGetValue(domainRequest.DomainId, out domain))
        throw new DomainNotFoundException(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.DomainNotFound((object) domainRequest.DomainId.Serialize()));
      return domain;
    }

    public IEnumerable<IDomainId> ListDomains() => (IEnumerable<IDomainId>) this.domainTable.Keys;
  }
}
