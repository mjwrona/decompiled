// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PackageIngestionUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class PackageIngestionUtils
  {
    public static IngestionDirection GetIngestionDirection(
      IEnumerable<UpstreamSourceInfo>? sourceChain)
    {
      bool flag;
      switch (sourceChain)
      {
        case null:
          flag = false;
          break;
        case IReadOnlyCollection<UpstreamSourceInfo> upstreamSourceInfos1:
          flag = upstreamSourceInfos1.Count > 0;
          break;
        case ICollection<UpstreamSourceInfo> upstreamSourceInfos2:
          flag = upstreamSourceInfos2.Count > 0;
          break;
        default:
          flag = sourceChain.Any<UpstreamSourceInfo>();
          break;
      }
      return !flag ? IngestionDirection.DirectPush : IngestionDirection.PullFromUpstream;
    }

    public static IngestionDirection GetIngestionDirection(this IAddOperationData addOp) => PackageIngestionUtils.GetIngestionDirection(addOp.SourceChain);

    public static FeedPermissionConstants GetRequiredAddPackagePermission(
      IngestionDirection direction)
    {
      if (direction == IngestionDirection.DirectPush)
        return FeedPermissionConstants.AddPackage;
      if (direction == IngestionDirection.PullFromUpstream)
        return FeedPermissionConstants.AddUpstreamPackage;
      throw new ArgumentOutOfRangeException(nameof (direction));
    }

    public static void ValidateMaxPackageSize(
      IVssRequestContext requestContext,
      string protocolType,
      Stream stream)
    {
      PackageIngestionUtils.ValidateMaxPackageSize(requestContext, protocolType, stream.Length);
    }

    public static void ValidateMaxPackageSize(
      IVssRequestContext requestContext,
      string protocolType,
      long fileSize)
    {
      new PackageSizeValidatingHandler((IRegistryService) new RegistryServiceFacade(requestContext), protocolType).Handle(fileSize);
    }
  }
}
