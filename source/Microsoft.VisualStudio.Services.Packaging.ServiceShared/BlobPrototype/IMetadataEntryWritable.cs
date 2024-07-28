// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IMetadataEntryWritable
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
  public interface IMetadataEntryWritable : IMetadataEntry, IPackageFiles
  {
    new PackagingCommitId CommitId { get; set; }

    new Guid CreatedBy { get; set; }

    new DateTime CreatedDate { get; set; }

    new Guid ModifiedBy { get; set; }

    new DateTime ModifiedDate { get; set; }

    new IStorageId PackageStorageId { get; set; }

    new long PackageSize { get; set; }

    new IEnumerable<Guid> Views { get; set; }

    new DateTime? DeletedDate { get; set; }

    new DateTime? ScheduledPermanentDeleteDate { get; set; }

    new DateTime? PermanentDeletedDate { get; set; }

    new DateTime? QuarantineUntilDate { get; set; }

    new IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }
  }
}
