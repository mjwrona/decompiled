// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.IPyPiMetadataEntryWritableCore`1
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public interface IPyPiMetadataEntryWritableCore<TPackageFile> : 
    IMetadataEntryWriteable<PyPiPackageIdentity>,
    IMetadataEntry<PyPiPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IMetadataEntryWritable,
    IPyPiMetadataEntryCore<TPackageFile>,
    IPackageFiles<TPackageFile>
    where TPackageFile : PyPiPackageFile
  {
    bool AddPackageFile(TPackageFile packageFile);

    new VersionConstraintList RequiresPython { get; set; }
  }
}
