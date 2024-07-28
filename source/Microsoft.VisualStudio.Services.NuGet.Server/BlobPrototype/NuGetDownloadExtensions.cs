// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetDownloadExtensions
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public static class NuGetDownloadExtensions
  {
    public static void AddNuspecDownloadTelemetry(
      this ICache<string, object> requestContextItems,
      IMetadataEntry metadata)
    {
      string str = metadata.IsLocal ? "local" : "upstreamCachedMetadata";
      requestContextItems.Set("Packaging.Properties.PackageSource", metadata.IsFromUpstream ? (object) "upstreamSaved" : (object) str);
      ICache<string, object> cache = requestContextItems;
      IEnumerable<UpstreamSourceInfo> sourceChain = metadata.SourceChain;
      // ISSUE: variable of a boxed type
      __Boxed<Guid?> val = (ValueType) (sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>()?.Id : new Guid?());
      cache.Set("Packaging.Properties.DirectUpstreamSourceId", (object) val);
    }
  }
}
