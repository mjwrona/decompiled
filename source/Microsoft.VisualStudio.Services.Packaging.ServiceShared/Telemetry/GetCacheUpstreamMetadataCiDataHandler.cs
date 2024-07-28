// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.GetCacheUpstreamMetadataCiDataHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class GetCacheUpstreamMetadataCiDataHandler : 
    IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>,
    IHaveInputType<FeedRequest<RefreshPackageResult>>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public GetCacheUpstreamMetadataCiDataHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(FeedRequest<RefreshPackageResult> request) => Task.FromResult<ICiData>((ICiData) new CacheUpstreamMetadataCiData(this.requestContext, request.Protocol, request.Feed, request.AdditionalData));
  }
}
