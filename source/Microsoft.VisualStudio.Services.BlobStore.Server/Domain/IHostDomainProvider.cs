// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.IHostDomainProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  public interface IHostDomainProvider : IDisposable
  {
    Task<bool> InitializeAsync(IVssRequestContext requestContext);

    Task<IMultiDomainInfo> GetDomainAsync(IVssRequestContext requestContext, IDomainId domainId);

    Task<IMultiDomainInfo> GetDefaultDomainAsync(IVssRequestContext requestContext);

    Task<IEnumerable<IMultiDomainInfo>> GetDomainsAsync(IVssRequestContext requestContext);

    Task<IMultiDomainInfo> CreateProjectDomainsForAdminAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ByteDomainId physicalDomainId,
      bool isDelete,
      bool forceDelete);
  }
}
