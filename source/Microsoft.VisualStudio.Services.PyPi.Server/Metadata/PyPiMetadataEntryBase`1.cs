// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataEntryBase`1
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public abstract class PyPiMetadataEntryBase<TPyPiPackageFile> : 
    MetadataEntryWithFiles<PyPiPackageIdentity, TPyPiPackageFile>
    where TPyPiPackageFile : PyPiPackageFile
  {
    protected PyPiMetadataEntryBase(PyPiPackageIdentity packageIdentity) => this.PackageIdentity = packageIdentity ?? throw new ArgumentNullException(nameof (packageIdentity));

    protected PyPiMetadataEntryBase(
      PyPiPackageIdentity packageIdentity,
      TPyPiPackageFile initialPackageFile,
      VersionConstraintList requiresPython,
      PackagingCommitId commitId,
      Guid createdBy,
      DateTime createdDate,
      Guid modifiedBy,
      DateTime modifiedDate,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, sourceChain, (IEnumerable<TPyPiPackageFile>) ImmutableList.Create<TPyPiPackageFile>(initialPackageFile))
    {
      this.PackageIdentity = packageIdentity ?? throw new ArgumentNullException(nameof (packageIdentity));
      this.RequiresPython = requiresPython;
    }

    public override IEqualityComparer<string> PathEqualityComparer => (IEqualityComparer<string>) StringComparer.Ordinal;

    public VersionConstraintList RequiresPython { get; set; }

    public override PyPiPackageIdentity PackageIdentity { get; }

    protected void CopyFrom(PyPiMetadataEntryBase<TPyPiPackageFile> source)
    {
      this.CopyFrom((MetadataEntryWithFiles<PyPiPackageIdentity, TPyPiPackageFile>) source);
      this.RequiresPython = source.RequiresPython;
    }
  }
}
