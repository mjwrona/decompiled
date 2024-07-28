// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public static class DownloadExtensions
  {
    public static void AddDownloadTelemetry(
      this ICache<string, object> requestContextItems,
      ICachablePackageMetadata metadata)
    {
      IReadOnlyList<UpstreamSourceInfo> sourceChain1 = metadata.SourceChain;
      UpstreamSourceInfo source = sourceChain1 != null ? sourceChain1.FirstOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
      IReadOnlyList<UpstreamSourceInfo> sourceChain2 = metadata.SourceChain;
      UpstreamSourceInfo upstreamSourceInfo = sourceChain2 != null ? sourceChain2.LastOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
      requestContextItems.Set("Packaging.Properties.PackageSource", (object) DownloadExtensions.GetPackageSource(source, metadata.StorageId));
      requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) source?.Id);
      requestContextItems.Set("Packaging.Properties.OriginUpstreamSourceType", (object) upstreamSourceInfo?.SourceType);
      requestContextItems.Set("Packaging.Properties.PackageStorageType", (object) metadata.StorageId.ToTelemetryStorageType());
    }

    public static void AddDownloadTelemetry(
      this ICache<string, object> requestContextItems,
      IMetadataEntry metadata)
    {
      IEnumerable<UpstreamSourceInfo> sourceChain1 = metadata.SourceChain;
      UpstreamSourceInfo source = sourceChain1 != null ? sourceChain1.FirstOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
      IEnumerable<UpstreamSourceInfo> sourceChain2 = metadata.SourceChain;
      UpstreamSourceInfo upstreamSourceInfo = sourceChain2 != null ? sourceChain2.LastOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
      requestContextItems.Set("Packaging.Properties.PackageSource", (object) DownloadExtensions.GetPackageSource(source, metadata.PackageStorageId));
      requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) source?.Id);
      requestContextItems.Set("Packaging.Properties.OriginUpstreamSourceType", (object) upstreamSourceInfo?.SourceType);
      requestContextItems.Set("Packaging.Properties.PackageStorageType", (object) metadata.PackageStorageId.ToTelemetryStorageType());
    }

    private static string GetPackageSource(UpstreamSourceInfo source, IStorageId storageId)
    {
      if (storageId is TryAllUpstreamsStorageId)
        return "upstreamUnknown";
      return source != null ? "upstreamSaved" : "local";
    }
  }
}
