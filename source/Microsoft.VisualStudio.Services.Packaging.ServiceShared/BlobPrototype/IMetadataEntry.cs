// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IMetadataEntry : IPackageFiles
  {
    IPackageIdentity PackageIdentity { get; }

    PackagingCommitId CommitId { get; }

    Guid CreatedBy { get; }

    DateTime CreatedDate { get; }

    Guid ModifiedBy { get; }

    DateTime ModifiedDate { get; }

    DateTime? PermanentDeletedDate { get; }

    DateTime? QuarantineUntilDate { get; }

    IStorageId PackageStorageId { get; }

    long PackageSize { get; }

    IEnumerable<Guid> Views { get; }

    DateTime? DeletedDate { get; }

    DateTime? ScheduledPermanentDeleteDate { get; }

    IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    bool IsLocal { get; }

    bool IsFromUpstream { get; }

    bool IsUpstreamCached { get; }
  }
}
