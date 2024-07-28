// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.InlineRefreshConcurrencyConsolidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class InlineRefreshConcurrencyConsolidator
  {
    private static readonly Guid Key = new Guid("1EBAE252-3171-4709-BC1F-4402D10B0894");

    public static IConcurrencyConsolidator<InlineRefreshKey, RefreshPackageResult> Bootstrap(
      IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return (IConcurrencyConsolidator<InlineRefreshKey, RefreshPackageResult>) ServiceHostLevelSharedObjectHolder.Get<ConcurrencyConsolidator<InlineRefreshKey, RefreshPackageResult>>(requestContext, InlineRefreshConcurrencyConsolidator.Key, (Func<Guid, ConcurrencyConsolidator<InlineRefreshKey, RefreshPackageResult>>) (_ => new ConcurrencyConsolidator<InlineRefreshKey, RefreshPackageResult>(false, 1)));
    }
  }
}
