// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.IUpdateUpstreamMetadataOperationData`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public interface IUpdateUpstreamMetadataOperationData<out TPackageName, TPackageVersion> : 
    IUpdateUpstreamMetadataOperationData,
    IPackageOperationData,
    ICommitOperationData
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
  {
    TPackageName PackageName { get; }

    string UpstreamsConfigurationHash { get; }

    DateTime UpstreamsLastRefreshedUtc { get; }

    object PackageNameMetadata { get; }

    ImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>> NewUpstreamVersionListsToCache { get; }

    DateTime MinimumUpstreamVersionListDate { get; }

    Dictionary<TPackageVersion, TerrapinIngestionValidationReason> EntriesToUpdateWithTerrapinData { get; }
  }
}
