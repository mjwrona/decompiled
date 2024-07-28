// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.InlineRefreshAsyncInvokerWithRebootstrapping
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class InlineRefreshAsyncInvokerWithRebootstrapping
  {
    public static IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager> Bootstrap(
      IVssRequestContext requestContext)
    {
      IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager> innerInvoker = PackagingServerConstants.OffloadInlineUpstreamRefreshesToTasks.Bootstrap(requestContext).Get() ? (IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>) new OffloadingAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>(requestContext) : (IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>) new InlineAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>(requestContext);
      if (PackagingServerConstants.ConsolidateConcurrentInlineUpstreamRefreshes.Bootstrap(requestContext).Get())
        innerInvoker = (IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>) new ConsolidatingAsyncInvokerWithAggRebootstrappingDecorator<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager>(innerInvoker, InlineRefreshConcurrencyConsolidator.Bootstrap(requestContext));
      return innerInvoker;
    }
  }
}
