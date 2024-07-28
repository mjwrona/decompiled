// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.CrossCollectionLocationDataFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class CrossCollectionLocationDataFacade : ICrossCollectionLocationDataService
  {
    private readonly IVssRequestContext requestContext;

    public CrossCollectionLocationDataFacade(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.requestContext = requestContext;
    }

    public async Task<ApiResourceLocationCollection> GetLocationsForBaseAddressAsync(
      Uri baseAddress,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.requestContext.GetService<Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData.ICrossCollectionLocationDataService>().GetLocationsForBaseAddressAsync(this.requestContext, baseAddress, cancellationToken);
    }
  }
}
